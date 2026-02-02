using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class UserMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    public string? UserCode { get; set; }
    [StringLength(100)]
    public required string UserName { get; set; }

    public long? EmployeeID { get; set; }

    public string? EmailId { get; set; }

    public string? Password { get; set; }

    public long? RoleID { get; set; }
    public string? RoleName { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool IsLoginEnabled { get; set; } = true;

    //Access Policy


    public bool RemoteLogin { get; set; } = false;
    [StringLength(500)]
    public string? IpRestriction { get; set; } // CSV or CIDR

    [StringLength(100)]
    public string? WorkingHours { get; set; } // e.g. 09:00-18:00

    public string AccountStatus { get; set; } = "Active";
    public DateTime? LastLoginDate { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;

    public int SessionTimeout { get; set; } = 30;
    public bool ForcePasswordChange { get; set; }

    public bool TwoFactorEnabled { get; set; } = false;
    public string? TwoFactorMethod { get; set; }
    public string? TwoFactorOtp { get; set; }
    public DateTime? TwoFactorOtpExpiry { get; set; }
    public DateTime? TwoFactorLastSentAt { get; set; }
    public int TwoFactorSendCount { get; set; } = 0;

    public int AutoLockAfterAttempts { get; set; } = 5;
    public string UnlockMethod { get; set; } = "Admin";
    [ForeignKey("RoleID")]
    public virtual RoleMaster? Role {  get; set; }
    [ForeignKey("EmployeeID")]
    public virtual EmployeeMaster? Employee { get; set; }

}
