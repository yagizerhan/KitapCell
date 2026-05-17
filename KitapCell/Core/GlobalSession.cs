using KitapCell.Models;

namespace KitapCell.Core
{
    /// <summary>
    /// Holds the currently authenticated user for the lifetime of the desktop application session.
    /// This is a simple static in-memory session — data is lost when the application closes.
    /// The web interface uses its own cookie-based session managed by <see cref="Web.SessionHelper"/>.
    /// </summary>
    public static class GlobalSession
    {
        /// <summary>
        /// The user who is currently logged in to the desktop application.
        /// Null when no user is authenticated (e.g. before the login form is shown).
        /// Set by <c>LoginForm</c> on successful authentication.
        /// </summary>
        public static User? CurrentUser { get; set; }

        /// <summary>True when a user is actively logged in; false otherwise.</summary>
        public static bool IsLoggedIn => CurrentUser != null;

        /// <summary>True when the current user holds the <see cref="UserRole.Admin"/> role.</summary>
        public static bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
    }
}
