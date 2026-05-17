using System.Collections.Generic;

namespace KitapCell.Models
{
    /// <summary>
    /// Represents a book author in the library system.
    /// A single author can be linked to many books (one-to-many).
    /// </summary>
    public class Author
    {
        /// <summary>Primary key — auto-incremented by EF Core.</summary>
        public int Id { get; set; }

        /// <summary>Full display name of the author (e.g. "Orhan Pamuk").</summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>Optional short biography or description of the author.</summary>
        public string? Biography { get; set; }

        /// <summary>Optional nationality of the author (e.g. "Turkish", "American").</summary>
        public string? Nationality { get; set; }

        // Navigation Property
        /// <summary>All books written by this author.</summary>
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
