using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class SpecificationGrade
{
    [Key]
    public long ID { get; set; }

    [Required]
    public long SpecificationHeaderID { get; set; }

    [StringLength(100), Required]
    public required string Grade { get; set; }

    // Presentation / administrative notes only (does NOT affect compliance/planning/execution)
    [StringLength(500)]
    public string? Remarks { get; set; }

    // MS-B: per-grade values for the header's enabled identifiers. Optional legacy string only.
    public string? IdentifierValuesJson { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public long? ModifiedBy { get; set; }

    [ForeignKey("SpecificationHeaderID")]
    public virtual SpecificationHeader? SpecificationHeader { get; set; }

    public virtual ICollection<SpecificationLine> SpecificationLines { get; set; } = new List<SpecificationLine>();
}