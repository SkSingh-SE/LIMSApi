using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class SpecificationHeader : AuditProperty
{
    [Key]
    public long ID { get; set; }
    [StringLength(100),Required]
    public required string AliasName { get; set; }

    public long? StandardOrganizationID { get; set; }


    public string? Standard { get; set; } // Not in use 

    public string? Part { get; set; }

    public string? StandardYear { get; set; }
    public bool IsCustom { get; set; }

    // MS1: material specification number (e.g. "1234")
    [StringLength(100)]
    public string? SpecificationNo { get; set; }

    // Version label. A new version = a new SpecificationHeader entry (no version table).
    [StringLength(50)]
    public string? Version { get; set; }

    // Auto-generated display title, e.g. "IS 1234 : v1"
    [StringLength(300)]
    public string? DisplayTitle { get; set; }

    // MS5: configurable caption / specification title
    [StringLength(300)]
    public string? Title { get; set; }

    // MS-B: which grade-identifier boxes are enabled for all grades (UNS No / Steel No / EN Grade /
    // Alloy No + up to 3 custom). JSON array of { key, label, isCustom, order }.
    public string? IdentifierConfigJson { get; set; }


    [ForeignKey("StandardOrganizationID")]
    public virtual StandardOrganizationMaster? StandardOrganization { get; set; }

    public virtual ICollection<SpecificationGrade> Grades { get; set; } = new List<SpecificationGrade>();

    // MS-A: header-level parameter template (copied into grade tabs on Add Grade)
    public virtual ICollection<SpecificationHeaderParameter> HeaderParameters { get; set; } = new List<SpecificationHeaderParameter>();

}