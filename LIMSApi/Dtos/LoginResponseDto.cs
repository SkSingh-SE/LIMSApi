namespace LIMSApi.Dtos
{
    public class UserBranchItemDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanCreate { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        public bool CanExecute { get; set; } = true;
        public bool CanApprove { get; set; } = false;
        public bool CanDelete { get; set; } = false;
    }

    public sealed class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public long UserId { get; set; }
        public long? EmployeeId { get; set; }

        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        public long? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public long? DefaultBranchId { get; set; }
        public string? DefaultBranchName { get; set; }
        public bool CanViewAllBranches { get; set; }
        public List<UserBranchItemDto> Branches { get; set; } = new();

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
