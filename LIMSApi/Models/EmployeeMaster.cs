using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class EmployeeMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [StringLength(5)]
    public string? BloodGroup { get; set; }

    [StringLength(255)]
    public string? ResidentialAddressLine1 { get; set; }

    [StringLength(255)]
    public string? ResidentialAddressLine2 { get; set; }

    public required string ResidentialPinCode { get; set; }
    public required long ResidentialAreaID { get; set; }

    [StringLength(255)]
    public string? PermanentAddressLine1 { get; set; }

    public string? PermanentAddressLine2 { get; set; }

    public required string PermanentPinCode { get; set; }
    public required long PermanentAreaID { get; set; }

    [StringLength(15)]
    public string? MobileNo { get; set; }

    [StringLength(15)]
    public string? Gender { get; set; }

    [StringLength(15)]
    public string? EmergencyMobileNo { get; set; }

    [StringLength(100)]
    [EmailAddress]
    public required string EmailId { get; set; }

    public bool? IsMarried { get; set; }

    [StringLength(100)]
    public string? SpouseName { get; set; }

    [StringLength(100)]
    public string? FatherName { get; set; }

    [StringLength(100)]
    public string? MotherName { get; set; }

    public long? DesignationID { get; set; }

    public DateTime? DateOfJoin { get; set; }

    public int? RelevantExperienceYears { get; set; }

    [StringLength(15)]
    public string? PANNumber { get; set; }

    [StringLength(100)]
    public string? BankName { get; set; }

    [StringLength(100)]
    public string? Branch { get; set; }

    [StringLength(100)]
    public string? AccountHolderName { get; set; }
    [StringLength(20)]
    public string? AccountNumber { get; set; }

    [StringLength(15)]
    public string? IFSCCode { get; set; }

    public long? DepartmentID { get; set; }

    public long? ReportingTo { get; set; }

    public long? UserID { get; set; }

    public bool? IsTeamHead { get; set; }

    public string? DigitalSignature { get; set; }
    public string? EmployeeStatus { get; set; }

    // Foreign Key Relations
    [ForeignKey("DepartmentID")]
    public virtual DepartmentMaster? Department { get; set; }

    [ForeignKey("DesignationID")]
    public virtual DesignationMaster? Designation { get; set; }

    [ForeignKey("ReportingTo")]
    public virtual EmployeeMaster? ReportingManager { get; set; }

    [ForeignKey("UserID")]
    public virtual UserMaster? User { get; set; }

    public virtual ICollection<EmployeeQualification> Qualifications { get; set; } = new List<EmployeeQualification>();
    public virtual ICollection<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();


}

