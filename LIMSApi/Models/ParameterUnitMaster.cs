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
    [Column(TypeName = "decimal(18,6)")]
    public decimal? ConversionFactor { get; set; }


    // Normalized equivalents (replaces inline SimilarUnit1-7; inline kept until cleanup phase).
    public virtual ICollection<ParameterUnitEquivalent> Equivalents { get; set; } = new List<ParameterUnitEquivalent>();


}
