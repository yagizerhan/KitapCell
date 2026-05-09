using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace KitapCell.Services
{
    public class FetchedBookData
    {
        public string Title { get; set; } = "";
        public List<string> Authors { get; set; } = new List<string>();
        public string Publisher { get; set; } = "";
        public string PublishYear { get; set; } = "";
        public int PageCount { get; set; } = 0;
        public string CoverUrl { get; set; } = "";
    }

    public static class BookFetchService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<FetchedBookData?> FetchBookByIsbnAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return null;

            isbn = isbn.Replace("-", "").Trim();

            // 1. OpenLibrary API'sini dene
            var openLibraryData = await TryFetchFromOpenLibrary(isbn);
            if (openLibraryData != null && !string.IsNullOrWhiteSpace(openLibraryData.Title))
            {
                return openLibraryData;
            }

            // 2. Yukarıdan sonuç gelmezse Google Books API'sini dene
            var googleData = await TryFetchFromGoogleBooks(isbn);
            if (googleData != null && !string.IsNullOrWhiteSpace(googleData.Title))
            {
                return googleData;
            }

            return null;
        }

        private static async Task<FetchedBookData?> TryFetchFromOpenLibrary(string isbn)
        {
            try
            {
                string url = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                string jsonContent = await response.Content.ReadAsStringAsync();
                
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
                return null;
            }
        }

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
                return null;
            }
        }
    }
}
