using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace KitapCell.Services
{
    /// <summary>
    /// Data transfer object (DTO) returned by <see cref="BookFetchService"/>.
    /// Contains all bibliographic fields that can be automatically filled in
    /// the AddBook form after a successful ISBN lookup.
    /// </summary>
    public class FetchedBookData
    {
        /// <summary>Book title returned by the API.</summary>
        public string Title { get; set; } = "";

        /// <summary>List of author names associated with the book.</summary>
        public List<string> Authors { get; set; } = new List<string>();

        /// <summary>Name of the publishing house.</summary>
        public string Publisher { get; set; } = "";

        /// <summary>Publication year as a string (e.g. "2003" or "March 2003").</summary>
        public string PublishYear { get; set; } = "";

        /// <summary>Total number of pages in the book. Zero if not provided by the API.</summary>
        public int PageCount { get; set; } = 0;

        /// <summary>Direct URL to the book cover image returned by the API. Empty if unavailable.</summary>
        public string CoverUrl { get; set; } = "";
    }

    /// <summary>
    /// Fetches book metadata from external APIs using an ISBN number.
    /// Tries <b>OpenLibrary</b> first; falls back to <b>Google Books</b> if no result is found.
    /// Both APIs are free and require no authentication key.
    /// Called by <c>AddBookForm.BtnSearchIsbn_Click</c> and triggered by
    /// the barcode scanner Enter event (<c>TxtISBN_KeyDown</c>).
    /// </summary>
    public static class BookFetchService
    {
        /// <summary>Shared HTTP client for all outbound API requests (reuse to avoid socket exhaustion).</summary>
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Main entry point. Strips dashes from the ISBN, then queries OpenLibrary.
        /// If OpenLibrary returns no usable data, retries with Google Books.
        /// </summary>
        /// <param name="isbn">Raw ISBN-10 or ISBN-13 string (dashes are stripped automatically).</param>
        /// <returns>
        /// A <see cref="FetchedBookData"/> object populated with the best available data,
        /// or <c>null</c> if neither API returned a result.
        /// </returns>
        public static async Task<FetchedBookData?> FetchBookByIsbnAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return null;

            // Normalise: remove dashes and trim whitespace (e.g. "978-3-16-148410-0" → "9783161484100")
            isbn = isbn.Replace("-", "").Trim();

            // 1. Try OpenLibrary first (richer metadata for older books)
            var openLibraryData = await TryFetchFromOpenLibrary(isbn);
            if (openLibraryData != null && !string.IsNullOrWhiteSpace(openLibraryData.Title))
            {
                return openLibraryData;
            }

            // 2. Fall back to Google Books (better coverage for recent publications)
            var googleData = await TryFetchFromGoogleBooks(isbn);
            if (googleData != null && !string.IsNullOrWhiteSpace(googleData.Title))
            {
                return googleData;
            }

            return null;
        }

        /// <summary>
        /// Queries the OpenLibrary Books API for the given ISBN.
        /// Endpoint: https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&amp;format=json&amp;jscmd=data
        /// Returns null on any network error or if the book is not found.
        /// </summary>
        private static async Task<FetchedBookData?> TryFetchFromOpenLibrary(string isbn)
        {
            try
            {
                string url = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                string jsonContent = await response.Content.ReadAsStringAsync();

                // OpenLibrary returns "{}" (empty object) when the ISBN is not in their catalog
                if (string.IsNullOrWhiteSpace(jsonContent) || jsonContent == "{}")
                    return null;

                using var doc = JsonDocument.Parse(jsonContent);
                var root = doc.RootElement;
                string key = $"ISBN:{isbn}";

                if (!root.TryGetProperty(key, out var bookElement))
                    return null;

                var result = new FetchedBookData();

                if (bookElement.TryGetProperty("title", out var titleElement))
                    result.Title = titleElement.GetString() ?? "";

                if (bookElement.TryGetProperty("number_of_pages", out var pagesElement))
                    result.PageCount = pagesElement.ValueKind == JsonValueKind.Number ? pagesElement.GetInt32() : 0;

                if (bookElement.TryGetProperty("publish_date", out var yearElement))
                    result.PublishYear = yearElement.GetString() ?? "";

                if (bookElement.TryGetProperty("publishers", out var publishersElement) && publishersElement.ValueKind == JsonValueKind.Array)
                {
                    if (publishersElement.GetArrayLength() > 0 && publishersElement[0].TryGetProperty("name", out var pubName))
                    {
                        result.Publisher = pubName.GetString() ?? "";
                    }
                }

                if (bookElement.TryGetProperty("authors", out var authorsElement) && authorsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var authorNode in authorsElement.EnumerateArray())
                    {
                        if (authorNode.TryGetProperty("name", out var authorName))
                        {
                            string aname = authorName.GetString() ?? "";
                            if (!string.IsNullOrWhiteSpace(aname))
                                result.Authors.Add(aname);
                        }
                    }
                }

                // Prefer the largest available cover image
                if (bookElement.TryGetProperty("cover", out var coverElement))
                {
                    if (coverElement.TryGetProperty("large", out var largeElement))
                        result.CoverUrl = largeElement.GetString() ?? "";
                    else if (coverElement.TryGetProperty("medium", out var mediumElement))
                        result.CoverUrl = mediumElement.GetString() ?? "";
                }

                return result;
            }
            catch
            {
                // Swallow all network and parse errors — the caller will try Google Books next
                return null;
            }
        }

        /// <summary>
        /// Queries the Google Books API for the given ISBN.
        /// Endpoint: https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}
        /// Returns null on any network error or if the book is not found.
        /// </summary>
        private static async Task<FetchedBookData?> TryFetchFromGoogleBooks(string isbn)
        {
            try
            {
                string url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                string jsonContent = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonContent);
                var root = doc.RootElement;

                // Google Books returns totalItems=0 when no match is found
                if (!root.TryGetProperty("items", out var itemsElement) || itemsElement.ValueKind != JsonValueKind.Array || itemsElement.GetArrayLength() == 0)
                    return null;

                var firstBook = itemsElement[0];
                if (!firstBook.TryGetProperty("volumeInfo", out var volumeInfo))
                    return null;

                var result = new FetchedBookData();

                if (volumeInfo.TryGetProperty("title", out var titleElement))
                    result.Title = titleElement.GetString() ?? "";

                if (volumeInfo.TryGetProperty("publisher", out var pubElement))
                    result.Publisher = pubElement.GetString() ?? "";

                if (volumeInfo.TryGetProperty("publishedDate", out var dateElement))
                {
                    // Google Books returns dates as "YYYY", "YYYY-MM", or "YYYY-MM-DD"
                    string date = dateElement.GetString() ?? "";
                    if (date.Length >= 4)
                        result.PublishYear = date.Substring(0, 4);
                }

                if (volumeInfo.TryGetProperty("pageCount", out var pagesElement))
                    result.PageCount = pagesElement.ValueKind == JsonValueKind.Number ? pagesElement.GetInt32() : 0;

                if (volumeInfo.TryGetProperty("authors", out var authorsElement) && authorsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var authorNode in authorsElement.EnumerateArray())
                    {
                        string aname = authorNode.GetString() ?? "";
                        if (!string.IsNullOrWhiteSpace(aname))
                            result.Authors.Add(aname);
                    }
                }

                if (volumeInfo.TryGetProperty("imageLinks", out var imageLinks))
                {
                    if (imageLinks.TryGetProperty("thumbnail", out var thumbElement))
                        result.CoverUrl = thumbElement.GetString() ?? "";
                }

                return result;
            }
            catch
            {
                // Swallow all network and parse errors — the UI will prompt for manual entry
                return null;
            }
        }
    }
}
