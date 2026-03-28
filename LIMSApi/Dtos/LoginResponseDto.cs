namespace LIMSApi.Dtos
{
    public sealed class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public long UserId { get; set; }
        public long? EmployeeId { get; set; }

        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        public string AccountStatus { get; set; } = string.Empty;
        public DateTime? LastLoginDate { get; set; }
        public int FailedLoginAttempts { get; set; }

        public int SessionTimeout { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int AutoLockAfterAttempts { get; set; }
        public string UnlockMethod { get; set; } = string.Empty;

        public bool AllowRemoteLogin { get; set; }
        public string? IpRestriction { get; set; }
        public string? WorkingHours { get; set; }

        public int ExpiresInSeconds { get; set; }
        public string? ProfileImagePath { get; set; }
    }

}
