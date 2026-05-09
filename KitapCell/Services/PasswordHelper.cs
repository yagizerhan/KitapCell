using System.Security.Cryptography;
using System.Text;

namespace KitapCell.Services
{
    /// <summary>
    /// Provides BCrypt-based password hashing and verification utilities.
    /// </summary>
    public static class PasswordHelper
    {
        // Hashes a plain-text password using BCrypt with work factor 12.
        // A unique salt is generated for every hash, making rainbow-table attacks infeasible.
        public static string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

        // Verifies a plain-text password against a stored BCrypt hash.
        // Returns false instead of throwing if the hash is malformed.
        public static bool Verify(string password, string hash)
        {
            try { return BCrypt.Net.BCrypt.Verify(password, hash); }
            catch { return false; }
        }
    }
}
