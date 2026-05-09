using System;

namespace KitapCell.Models
{
    public class UserRating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        
        public int Score { get; set; } // 1-5
        public string? Review { get; set; }
        public DateTime RatingDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public User User { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}
