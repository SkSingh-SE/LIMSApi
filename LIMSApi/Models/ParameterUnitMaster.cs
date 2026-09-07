using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class ParameterUnitMaster : AuditProperty
{
    public long ID { get; set; }

    [Required, StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public required string Name { get; set; }

    [Required, StringLength(50)]
    public string Symbol { get; set; } = string.Empty;

    [StringLength(50)]
    public string? QuantityType { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? ConversionFactor { get; set; } = 1.0m;

    // Normalized equivalents (replaces inline SimilarUnit1-7; inline kept until cleanup phase).
    public virtual ICollection<ParameterUnitEquivalent> Equivalents { get; set; } = new List<ParameterUnitEquivalent>();
}
