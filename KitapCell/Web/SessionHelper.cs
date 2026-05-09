using System;
using System.Collections.Concurrent;

namespace KitapCell.Web
{
    /// <summary>
    /// Basit bellek-içi (in-memory) cookie session yönetimi.
    /// Sunucu yeniden başlatılınca tüm sessionlar sıfırlanır — bu kasıtlı bir tasarım kararıdır.
    /// </summary>
    public static class SessionHelper
    {
        private record SessionData(int UserId, DateTime Expiry);

        private static readonly ConcurrentDictionary<string, SessionData> _sessions = new();

        // ─── Rate Limiting ──────────────────────────────────────────────────────
        // Aynı IP'den 5 başarısız giriş → 15 dakika kilit

        private record LoginAttempt(int Count, DateTime LockUntil);
        private static readonly ConcurrentDictionary<string, LoginAttempt> _loginAttempts = new();

        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(15);

        /// <summary>Bu IP rate limit kapsamında mı kontrol eder.</summary>
        public static bool IsRateLimited(string ip)
        {
            if (_loginAttempts.TryGetValue(ip, out var attempt))
            {
                if (attempt.LockUntil > DateTime.UtcNow)
                    return true; // Hâlâ kilitli

                // Kilit süresi geçmiş → temizle
                _loginAttempts.TryRemove(ip, out _);
            }
            return false;
        }

        /// <summary>Kaç dakika kilitli kaldığını döner (0 = kilitli değil).</summary>
        public static int LockRemainingMinutes(string ip)
        {
            if (_loginAttempts.TryGetValue(ip, out var attempt) && attempt.LockUntil > DateTime.UtcNow)
                return (int)Math.Ceiling((attempt.LockUntil - DateTime.UtcNow).TotalMinutes);
            return 0;
        }

        /// <summary>Başarısız giriş denemesini kaydeder. 5. denemede kilitleme başlar.</summary>
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

        /// <summary>Başarılı girişte sayacı sıfırlar.</summary>
        public static void ClearFailedAttempts(string ip)
            => _loginAttempts.TryRemove(ip, out _);

        // ─── Session ─────────────────────────────────────────────────────────────

        public static string CreateSession(int userId, int expiryHours = 24)
        {
            var token = Guid.NewGuid().ToString("N");
            _sessions[token] = new SessionData(userId, DateTime.UtcNow.AddHours(expiryHours));
            return token;
        }

        public static int? GetUserId(string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            if (_sessions.TryGetValue(token, out var session))
            {
                if (session.Expiry > DateTime.UtcNow) return session.UserId;
                _sessions.TryRemove(token, out _); // Süresi dolmuş → temizle
            }
            return null;
        }

        public static void RemoveSession(string? token)
        {
            if (!string.IsNullOrWhiteSpace(token))
                _sessions.TryRemove(token, out _);
        }

        public static void ClearAll() => _sessions.Clear();
    }
}
