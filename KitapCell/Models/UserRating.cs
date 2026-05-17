using System;

namespace KitapCell.Models
{
    /// <summary>
    /// Stores a user's star rating and optional written review for a book.
    /// Each (user, book) pair can have at most one rating record;
    /// submitting a second rating updates the existing one.
    /// The book's <see cref="Book.AverageRating"/> is recalculated by
    /// <see cref="Services.BookService.AddOrUpdateBookRatingAsync"/> after every save.
    /// </summary>
    public class UserRating
    {
        /// <summary>Primary key — auto-incremented by EF Core.</summary>
        public int Id { get; set; }

        /// <summary>Foreign key referencing the <see cref="User"/> who submitted the rating.</summary>
        public int UserId { get; set; }

        /// <summary>Foreign key referencing the <see cref="Book"/> being rated.</summary>
        public int BookId { get; set; }

        /// <summary>Star score from 1 (lowest) to 5 (highest).</summary>
        public int Score { get; set; } // 1-5

        /// <summary>Optional written review text accompanying the star score.</summary>
        public string? Review { get; set; }

        /// <summary>Date and time the rating was submitted or last updated.</summary>
        public DateTime RatingDate { get; set; } = DateTime.Now;

        // Navigation Properties
        /// <summary>The user who submitted the rating.</summary>
        public User User { get; set; } = null!;

        /// <summary>The book that was rated.</summary>
        public Book Book { get; set; } = null!;
    }
}
