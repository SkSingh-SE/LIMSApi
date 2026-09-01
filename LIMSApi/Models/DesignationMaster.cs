using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class DesignationMaster :AuditProperty
{
    [Key]
    public long ID { get; set; }

    [StringLength(150)]
    public required string Name { get; set; }

    [MaxLength(3000)]
    public string? Description { get; set; }

    /// <summary>
    /// Role assigned to this designation. Employees inherit their role through their Designation.
    /// </summary>
    public long? RoleID { get; set; }

    [ForeignKey("RoleID")]
    public virtual RoleMaster? Role { get; set; }

    [MaxLength(500)]
    public string? Qualification { get; set; }

    public int? MinExperience { get; set; }  // in years

    [MaxLength(3000)]
    public string? PersonalityTraits { get; set; }

    [MaxLength(3000)]
    public string? RolesAndResponsibilities { get; set; }
}
