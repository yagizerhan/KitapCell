using System;
using System.Collections.Generic;

namespace KitapCell.Models
{
    /// <summary>
    /// Represents a library member or staff account.
    /// Stores authentication credentials, role-based permissions,
    /// and gamification data (reputation score).
    /// </summary>
    public class User
    {
        /// <summary>Primary key — auto-incremented by EF Core.</summary>
        public int Id { get; set; }

        /// <summary>User's first name.</summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>User's last name (surname).</summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// National Identity Number (TC Kimlik No). Stored for administrative records;
        /// not used for authentication.
        /// </summary>
        public string IdentityNumber { get; set; } = string.Empty;

        /// <summary>Optional contact phone number.</summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Email address used as the login username. Must be unique across all users.
        /// Enforced by a unique index in <see cref="Data.LibraryDbContext"/>.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// BCrypt hash of the user's password. Plain-text passwords are never stored.
        /// Hashing is performed by <see cref="Services.PasswordHelper.Hash"/>.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// The user's role which determines their base access level.
        /// Possible values: <see cref="UserRole.Uye"/> (member),
        /// <see cref="UserRole.Admin"/> (administrator).
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Uye;

        // Granular Permissions — override defaults set by Role
        /// <summary>When true, this user can add new books regardless of their role.</summary>
        public bool CanAddBook { get; set; } = false;

        /// <summary>When true, this user can edit existing book records.</summary>
        public bool CanEditBook { get; set; } = false;

        /// <summary>When true, this user can delete book records from the catalog.</summary>
        public bool CanDeleteBook { get; set; } = false;

        /// <summary>
        /// Absolute path to the user's profile photo.
        /// Null means the default avatar placeholder is displayed.
        /// </summary>
        public string? ProfileImagePath { get; set; }

        /// <summary>Date and time the account was first created.</summary>
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gamification score. Increased for on-time returns and reviews;
        /// decreased for late returns. Cannot go below zero.
        /// Managed by <see cref="Services.UserService.UpdateReputationAsync"/>.
        /// </summary>
        public int ReputationScore { get; set; } = 0;

        /// <summary>
        /// When false, the user cannot log in to the desktop or web interface.
        /// Admins can deactivate accounts without deleting their records.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        /// <summary>All loan records created for this user.</summary>
        public ICollection<BookLoan> Loans { get; set; } = new List<BookLoan>();

        /// <summary>All book ratings submitted by this user.</summary>
        public ICollection<UserRating> Ratings { get; set; } = new List<UserRating>();

        /// <summary>All books this user has added to their favorites list.</summary>
        public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();

        /// <summary>Reading history entries tracking this user's progress across books.</summary>
        public ICollection<ReadingHistory> ReadingHistories { get; set; } = new List<ReadingHistory>();
    }
}
