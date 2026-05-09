using KitapCell.Models;

namespace KitapCell.Core
{
    public static class GlobalSession
    {
        public static User? CurrentUser { get; set; }
        
        public static bool IsLoggedIn => CurrentUser != null;
        public static bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
    }
}
