using System;

namespace KitapCell.Models
{
    public class UserFavorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        
        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public User User { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}
