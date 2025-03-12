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

    [StringLength(10)]
    public string? Gender { get; set; }

    public long? DepartmentID { get; set; }

    public long? DesignationID { get; set; }

    public DateTime BirthDate { get; set; }

    public DateTime? JoinDate { get; set; }

    [StringLength(255)]
    public string? ResidentialAddress { get; set; }

    [StringLength(255)]
    public string? PermanentResidentialAddress { get; set; }

    [StringLength(15)]
    public string? MobileNo { get; set; }

    [StringLength(100)]
    [EmailAddress]
    public required string EmailId { get; set; }

    public long? ReportingTo { get; set; }

    public long? UserID { get; set; }

    public bool? IsTeamHead { get; set; }

    public string? DigitalSignature { get; set; }

    public long? TestTypeID { get; set; }

    [StringLength(15)]
    public string? EmergencyMobileNo { get; set; }

    public bool? IsMarried { get; set; }

    [StringLength(100)]
    public string? SpouseName { get; set; }

    [StringLength(100)]
    public string? FatherName { get; set; }

    [StringLength(5)]
    public string? BloodGroup { get; set; }

    [StringLength(100)]
    public string? MotherName { get; set; }

    // Foreign Key Relations (Assuming related entities exist)
    [ForeignKey("DepartmentID")]
    public virtual DepartmentMaster? Department { get; set; }

    [ForeignKey("DesignationID")]
    public virtual DesignationMaster? Designation { get; set; }

    [ForeignKey("ReportingTo")]
    public virtual EmployeeMaster? ReportingManager { get; set; }

    [ForeignKey("UserID")]
    public virtual UserMaster? User { get; set; }

    [ForeignKey("TestTypeID")]
    public virtual TestTypeMaster? TestType { get; set; }
}
