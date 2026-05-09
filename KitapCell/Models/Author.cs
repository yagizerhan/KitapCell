using System.Collections.Generic;

namespace KitapCell.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public string? Nationality { get; set; }

        // Navigation Property
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
