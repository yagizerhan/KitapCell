using System;

namespace KitapCell.Models
{
    /// <summary>
    /// Join entity that represents a user's personal favorites list.
    /// Each record links one user to one book they have marked as a favourite.
    /// The many-to-many relationship between <see cref="User"/> and <see cref="Book"/>
    /// is resolved through this table.
    /// </summary>
    public class UserFavorite
    {
        /// <summary>Primary key — auto-incremented by EF Core.</summary>
        public int Id { get; set; }

        /// <summary>Foreign key referencing the <see cref="User"/> who created the favourite.</summary>
        public int UserId { get; set; }

        /// <summary>Foreign key referencing the <see cref="Book"/> that was favourited.</summary>
        public int BookId { get; set; }

        /// <summary>Date and time the user added this book to their favourites list.</summary>
        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        /// <summary>The user who favourited the book.</summary>
        public User User { get; set; } = null!;

        /// <summary>The book that was favourited.</summary>
        public Book Book { get; set; } = null!;
    }
}
