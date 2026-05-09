using System;

namespace KitapCell.Models
{
    public class ReadingHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? FinishDate { get; set; }
        
        public int CurrentPage { get; set; } = 0;
        
        [System.ComponentModel.DataAnnotations.MaxLength(255)]
        public string? LastLocationCfi { get; set; }
        
        public bool IsCompleted { get; set; } = false;
        public DateTime LastReadDate { get; set; } = DateTime.Now;
        public int TotalReadSeconds { get; set; } = 0;

        // Navigation Properties
        public User User { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}
