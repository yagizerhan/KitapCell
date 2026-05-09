using System;

namespace KitapCell.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.Bilgi;
        
        public bool IsRead { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Property
        public User User { get; set; } = null!;
    }
}
