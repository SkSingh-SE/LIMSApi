using Microsoft.Extensions.Caching.Memory;

namespace LIMSApi.Helpers
{
    /// <summary>
    /// Custom rate limiter for login endpoint.
    /// Tracks failed attempts per IP. Resets counter on successful login.
    ///
    /// Why custom instead of built-in ASP.NET Core rate limiter?
    /// The built-in FixedWindowLimiter cannot reset a specific partition's
    /// counter on demand. We need "reset on success" behavior.
    /// </summary>
    public class LoginRateLimiter
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<LoginRateLimiter> _logger;

        // Configuration
        private const int MaxAttempts = 10;
        private static readonly TimeSpan Window = TimeSpan.FromMinutes(5);

        public LoginRateLimiter(IMemoryCache cache, ILogger<LoginRateLimiter> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Check if the IP is allowed to attempt login.
        /// Returns (allowed, remainingAttempts, retryAfterSeconds).
        /// </summary>
        public (bool Allowed, int Remaining, int RetryAfterSeconds) Check(string ipAddress)
        {
            var key = $"login_attempts:{ipAddress}";

            if (_cache.TryGetValue(key, out LoginAttemptRecord? record) && record != null)
            {
                if (record.Count >= MaxAttempts)
                {
                    var retryAfter = (int)(record.WindowExpires - DateTimeOffset.UtcNow).TotalSeconds;
                    if (retryAfter <= 0)
                    {
                        // Window expired — reset
                        _cache.Remove(key);
                        return (true, MaxAttempts, 0);
                    }

                    _logger.LogWarning("Login rate limit exceeded for IP {IP}. Retry after {Seconds}s", ipAddress, retryAfter);
                    return (false, 0, retryAfter);
                }

                return (true, MaxAttempts - record.Count, 0);
            }

            return (true, MaxAttempts, 0);
        }

        /// <summary>
        /// Record a failed login attempt for this IP.
        /// </summary>
        public void RecordFailure(string ipAddress)
        {
            var key = $"login_attempts:{ipAddress}";

            var record = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow.Add(Window);
                return new LoginAttemptRecord
                {
                    Count = 0,
                    WindowExpires = DateTimeOffset.UtcNow.Add(Window)
                };
            })!;

            record.Count++;

            // Update cache with new count (keep same expiration)
            _cache.Set(key, record, record.WindowExpires);

            _logger.LogInformation(
                "Login failed for IP {IP}. Attempt {Count}/{Max}",
                ipAddress, record.Count, MaxAttempts);
        }

        /// <summary>
        /// Reset the counter for this IP after successful login.
        /// </summary>
        public void ResetOnSuccess(string ipAddress)
        {
            var key = $"login_attempts:{ipAddress}";
            _cache.Remove(key);
        }

        private class LoginAttemptRecord
        {
            public int Count { get; set; }
            public DateTimeOffset WindowExpires { get; set; }
        }
    }
}
