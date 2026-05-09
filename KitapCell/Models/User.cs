using System;
using System.Collections.Generic;

namespace KitapCell.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string IdentityNumber { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        
        public UserRole Role { get; set; } = UserRole.Uye;
        
        // Granüler Yetkiler (Granular Permissions)
        public bool CanAddBook { get; set; } = false;
        public bool CanEditBook { get; set; } = false;
        public bool CanDeleteBook { get; set; } = false;

        public string? ProfileImagePath { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        
        public decimal TotalPenalty { get; set; } = 0;
        public int ReputationScore { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<BookLoan> Loans { get; set; } = new List<BookLoan>();
        public ICollection<UserRating> Ratings { get; set; } = new List<UserRating>();
        public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
        public ICollection<ReadingHistory> ReadingHistories { get; set; } = new List<ReadingHistory>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
