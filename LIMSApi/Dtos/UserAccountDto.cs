namespace LIMSApi.Dtos
{
    public class UserAccountDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public bool IsLoginEnabled { get; set; } = true;
        public string AccountStatus { get; set; } = "Active";
        public DateTime? LastLoginDate { get; set; }
        public int FailedLoginAttempts { get; set; }

        public string CurrentRole { get; set; } = string.Empty;

        public int SessionTimeout { get; set; }
        public bool ForcePasswordChange { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? TwoFactorMethod { get; set; }

        public int AutoLockAfterAttempts { get; set; }
        public string UnlockMethod { get; set; } = "Admin";

        public bool AllowRemoteLogin { get; set; }
        public string? IpRestriction { get; set; }
        public string? WorkingHours { get; set; }
    }

}
