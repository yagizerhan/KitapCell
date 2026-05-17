using System;

namespace KitapCell.Models
{
    /// <summary>
    /// Tracks a user's reading progress for a specific book.
    /// A record is created the first time a user opens a digital book in the reader,
    /// and is updated each time they advance their position.
    /// This enables the "resume reading" feature — the reader reopens the book
    /// at the last saved page or EPUB CFI location.
    /// </summary>
    public class ReadingHistory
    {
        /// <summary>Primary key — auto-incremented by EF Core.</summary>
        public int Id { get; set; }

        /// <summary>Foreign key referencing the reading <see cref="User"/>.</summary>
        public int UserId { get; set; }

        /// <summary>Foreign key referencing the <see cref="Book"/> being read.</summary>
        public int BookId { get; set; }

        /// <summary>Date and time the user first opened this book in the reader.</summary>
        public DateTime StartDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Date and time the user marked the book as finished.
        /// Null while the book is still in progress.
        /// </summary>
        public DateTime? FinishDate { get; set; }

        /// <summary>
        /// Current page number for PDF files.
        /// Updated by the in-app reader on every page turn.
        /// </summary>
        public int CurrentPage { get; set; } = 0;

        /// <summary>
        /// EPUB Canonical Fragment Identifier (CFI) that precisely encodes
        /// the user's last reading position inside an EPUB document.
        /// Used by the Bibi reader to restore scroll position.
        /// Null for PDF files (which use <see cref="CurrentPage"/> instead).
        /// </summary>
        [System.ComponentModel.DataAnnotations.MaxLength(255)]
        public string? LastLocationCfi { get; set; }

        /// <summary>
        /// True when the user has read through the entire book.
        /// Can also be manually set from the profile page.
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        /// <summary>Timestamp of the most recent reading session, used for sorting recently-read lists.</summary>
        public DateTime LastReadDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Cumulative seconds the user has spent reading this book.
        /// Reserved for future analytics / reading-speed features.
        /// </summary>
        public int TotalReadSeconds { get; set; } = 0;

        // Navigation Properties
        /// <summary>The user whose reading progress this record tracks.</summary>
        public User User { get; set; } = null!;

        /// <summary>The book being tracked.</summary>
        public Book Book { get; set; } = null!;
    }
}
