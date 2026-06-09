using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class ParameterUnitMaster : AuditProperty
{
    public long ID { get; set; }
    [StringLength(100)]
    public required string Name { get; set; }
    [Required,StringLength(10)]
    public required string ConversaionFactor { get; set; }

    [MaxLength(50)]
    public string? SimilarUnit1 { get; set; }
    [Column(TypeName = "decimal(18,6)")]
    public decimal? ConversionFactor1 { get; set; }

    [MaxLength(50)]
    public string? SimilarUnit2 { get; set; }
    [Column(TypeName = "decimal(18,6)")]
    public decimal? ConversionFactor2 { get; set; }

    [MaxLength(50)]
    public string? SimilarUnit3 { get; set; }
    [Column(TypeName = "decimal(18,6)")]
    public decimal? ConversionFactor3 { get; set; }

    [MaxLength(50)]
    public string? SimilarUnit4 { get; set; }
    [Column(TypeName = "decimal(18,6)")]
    public decimal? ConversionFactor4 { get; set; }

    [MaxLength(50)]
    public string? SimilarUnit5 { get; set; }
    [Column(TypeName = "decimal(18,6)")]
    public decimal? ConversionFactor5 { get; set; }

    [MaxLength(50)]
    public string? SimilarUnit6 { get; set; }
    [Column(TypeName = "decimal(18,6)")]
    public decimal? ConversionFactor6 { get; set; }

    [MaxLength(50)]
    public string? SimilarUnit7 { get; set; }
    [Column(TypeName = "decimal(18,6)")]
    public decimal? ConversionFactor7 { get; set; }

    // Normalized equivalents (replaces inline SimilarUnit1-7; inline kept until cleanup phase).
    public virtual ICollection<ParameterUnitEquivalent> Equivalents { get; set; } = new List<ParameterUnitEquivalent>();
}
