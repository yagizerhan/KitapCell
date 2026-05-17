using System;
using System.Collections.Concurrent;

namespace KitapCell.Web
{
    /// <summary>
    /// Manages lightweight in-memory cookie sessions for the embedded Kestrel web server.
    /// Each session is identified by a random GUID token stored in the browser as a cookie.
    /// Sessions are lost when the web server (or the application) is restarted — this is
    /// intentional; the library web interface does not require persistent sessions.
    /// Also enforces brute-force protection via IP-based login rate limiting.
    /// </summary>
    public static class SessionHelper
    {
        /// <summary>Internal record that binds a session token to a user ID and expiry timestamp.</summary>
        private record SessionData(int UserId, DateTime Expiry);

        /// <summary>Thread-safe dictionary of active sessions, keyed by the session token string.</summary>
        private static readonly ConcurrentDictionary<string, SessionData> _sessions = new();

        // ─── Rate Limiting ──────────────────────────────────────────────────────
        // After 5 consecutive failed login attempts from the same IP, that IP is
        // locked out for 15 minutes to prevent brute-force password attacks.

        /// <summary>Tracks the number of failed login attempts and lock expiry per IP address.</summary>
        private record LoginAttempt(int Count, DateTime LockUntil);

        /// <summary>Thread-safe dictionary mapping IP addresses to their current attempt record.</summary>
        private static readonly ConcurrentDictionary<string, LoginAttempt> _loginAttempts = new();

        /// <summary>Maximum allowed consecutive failed login attempts before a lockout is triggered.</summary>
        private const int MaxFailedAttempts = 5;

        /// <summary>How long an IP address is locked out after exceeding <see cref="MaxFailedAttempts"/>.</summary>
        private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(15);

        /// <summary>
        /// Returns true if the given IP address is currently blocked due to too many failed login attempts.
        /// Expired lockouts are automatically cleared when this method is called.
        /// </summary>
        /// <param name="ip">The remote IP address to check.</param>
        public static bool IsRateLimited(string ip)
        {
            if (_loginAttempts.TryGetValue(ip, out var attempt))
            {
                if (attempt.LockUntil > DateTime.UtcNow)
                    return true; // Still locked

                // Lockout period has expired — remove the stale record
                _loginAttempts.TryRemove(ip, out _);
            }
            return false;
        }

        /// <summary>
        /// Returns the number of minutes remaining on an active lockout for the given IP.
        /// Returns 0 if the IP is not currently locked out.
        /// </summary>
        /// <param name="ip">The remote IP address to query.</param>
        public static int LockRemainingMinutes(string ip)
        {
            if (_loginAttempts.TryGetValue(ip, out var attempt) && attempt.LockUntil > DateTime.UtcNow)
                return (int)Math.Ceiling((attempt.LockUntil - DateTime.UtcNow).TotalMinutes);
            return 0;
        }

        /// <summary>
        /// Records a failed login attempt from the given IP.
        /// On the 5th failure the lockout timer starts and further logins from that
        /// IP are rejected until the lock duration has elapsed.
        /// </summary>
        /// <param name="ip">The remote IP address of the failed attempt.</param>
        public static void RecordFailedAttempt(string ip)
        {
            _loginAttempts.AddOrUpdate(ip,
                _ => new LoginAttempt(1, DateTime.MinValue),
                (_, prev) =>
                {
                    int newCount = prev.Count + 1;
                    DateTime lockUntil = newCount >= MaxFailedAttempts
                        ? DateTime.UtcNow.Add(LockDuration)
                        : prev.LockUntil;
                    return new LoginAttempt(newCount, lockUntil);
                });
        }

        /// <summary>
        /// Clears the failed-attempt counter for the given IP after a successful login.
        /// </summary>
        /// <param name="ip">The remote IP address to clear.</param>
        public static void ClearFailedAttempts(string ip)
            => _loginAttempts.TryRemove(ip, out _);

        // ─── Session Management ───────────────────────────────────────────────────

        /// <summary>
        /// Creates a new session for the given user and returns the session token.
        /// The token should be sent to the browser as an HTTP-only cookie.
        /// </summary>
        /// <param name="userId">ID of the authenticated user.</param>
        /// <param name="expiryHours">Session lifetime in hours. Default: 24 hours.</param>
        /// <returns>A unique GUID-based session token string.</returns>
        public static string CreateSession(int userId, int expiryHours = 24)
        {
            var token = Guid.NewGuid().ToString("N");
            _sessions[token] = new SessionData(userId, DateTime.UtcNow.AddHours(expiryHours));
            return token;
        }

        /// <summary>
        /// Resolves a session token to the corresponding user ID.
        /// Returns null if the token is invalid, missing, or has expired.
        /// Expired sessions are automatically removed from memory.
        /// </summary>
        /// <param name="token">The session token from the browser cookie.</param>
        public static int? GetUserId(string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            if (_sessions.TryGetValue(token, out var session))
            {
                if (session.Expiry > DateTime.UtcNow) return session.UserId;
                _sessions.TryRemove(token, out _); // Session expired — clean up
            }
            return null;
        }

        /// <summary>
        /// Invalidates a specific session, effectively logging that browser out.
        /// Called when the user clicks "Logout" in the web interface.
        /// </summary>
        /// <param name="token">The session token to invalidate.</param>
        public static void RemoveSession(string? token)
        {
            if (!string.IsNullOrWhiteSpace(token))
                _sessions.TryRemove(token, out _);
        }

        /// <summary>
        /// Clears all active sessions. Called when the web server is stopped
        /// to ensure no stale sessions persist if the server is restarted.
        /// </summary>
        public static void ClearAll() => _sessions.Clear();
    }
}
