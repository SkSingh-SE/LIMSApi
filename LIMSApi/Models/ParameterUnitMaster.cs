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
}
