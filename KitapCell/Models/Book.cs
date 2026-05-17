using System;
using System.Collections.Generic;

namespace KitapCell.Models
{
    /// <summary>
    /// Represents a physical or digital book in the library catalog.
    /// Contains all bibliographic data, physical stock tracking fields,
    /// and optional paths to a digital copy (PDF or EPUB).
    /// </summary>
    public class Book
    {
        /// <summary>Primary key — auto-incremented by EF Core.</summary>
        public int Id { get; set; }

        /// <summary>
        /// International Standard Book Number. Must be unique across the library.
        /// For digitally-added books without a real ISBN, the system auto-generates
        /// a placeholder in the format "DIGITAL-XXXXXXXX".
        /// </summary>
        public string ISBN { get; set; } = string.Empty;

        /// <summary>Full title of the book as it appears on the cover.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Foreign key referencing the <see cref="Author"/> entity.</summary>
        public int AuthorId { get; set; }

        /// <summary>Foreign key referencing the <see cref="Category"/> entity.</summary>
        public int CategoryId { get; set; }

        /// <summary>Name of the publishing house (e.g. "İş Bankası Yayınları").</summary>
        public string? Publisher { get; set; }

        /// <summary>Year the book was first published (e.g. 1984).</summary>
        public int? PublishYear { get; set; }

        /// <summary>
        /// Total number of physical copies owned by the library.
        /// Defaults to 1 when a new book is added.
        /// </summary>
        public int TotalCopies { get; set; } = 1;

        /// <summary>
        /// Number of copies currently available for lending.
        /// Decremented when a book is lent and incremented on return.
        /// </summary>
        public int AvailableCopies { get; set; } = 1;

        /// <summary>Optional back-cover synopsis or description.</summary>
        public string? Description { get; set; }

        /// <summary>Language the book is written in (default: "Türkçe").</summary>
        public string Language { get; set; } = "Türkçe";

        /// <summary>Total number of pages in the book.</summary>
        public int? PageCount { get; set; }

        /// <summary>
        /// Absolute file-system path to the book's cover image.
        /// Stored in the Assets/Covers/ directory.
        /// If null, a generated placeholder is shown in the UI.
        /// </summary>
        public string? CoverImagePath { get; set; }

        /// <summary>
        /// Absolute file-system path to the book's PDF or EPUB file.
        /// Stored in the Assets/Pdfs/ directory.
        /// Null when the book has no digital copy.
        /// </summary>
        public string? PdfFilePath { get; set; }

        /// <summary>
        /// True when a valid digital file (PDF or EPUB) exists for this book.
        /// Computed automatically when a file is uploaded; controls reader button visibility.
        /// </summary>
        public bool HasDigitalCopy { get; set; } = false;

        /// <summary>
        /// Average star rating calculated from all <see cref="UserRating"/> records.
        /// Recalculated by <c>BookService.AddOrUpdateBookRatingAsync</c> after every review.
        /// Range: 0.0 – 5.0.
        /// </summary>
        public float AverageRating { get; set; } = 0;

        /// <summary>Date and time the book record was added to the library catalog.</summary>
        public DateTime AddedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Optional physical shelf location code (e.g. "A-12", "3rd Floor").
        /// Used by librarians to quickly locate the book.
        /// </summary>
        public string? Location { get; set; }

        // Navigation Properties
        /// <summary>The author of this book.</summary>
        public Author Author { get; set; } = null!;

        /// <summary>The category this book belongs to.</summary>
        public Category Category { get; set; } = null!;

        /// <summary>All loan records associated with this book.</summary>
        public ICollection<BookLoan> Loans { get; set; } = new List<BookLoan>();

        /// <summary>All ratings submitted for this book.</summary>
        public ICollection<UserRating> Ratings { get; set; } = new List<UserRating>();

        /// <summary>All user-favorite entries pointing to this book.</summary>
        public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();

        /// <summary>All reading-history entries for this book across all users.</summary>
        public ICollection<ReadingHistory> ReadingHistories { get; set; } = new List<ReadingHistory>();
    }
}
