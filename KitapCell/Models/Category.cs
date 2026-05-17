using System.Collections.Generic;

namespace KitapCell.Models
{
    /// <summary>
    /// Represents a book category (genre) in the library system.
    /// Each book belongs to exactly one category.
    /// </summary>
    public class Category
    {
        /// <summary>Primary key — auto-incremented by EF Core.</summary>
        public int Id { get; set; }

        /// <summary>Human-readable category name (e.g. "Roman", "Bilim Kurgu").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional emoji icon displayed next to the category name in the UI
        /// (e.g. "📖", "🚀"). Makes visual scanning of category lists faster.
        /// </summary>
        public string? IconEmoji { get; set; }

        // Navigation Property
        /// <summary>All books that belong to this category.</summary>
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
