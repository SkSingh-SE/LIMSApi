using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models;

public partial class DimensionalFactorShapeFormula : AuditProperty
{
    [Key]
    public long ID { get; set; }

    public long DimensionalFactorID { get; set; }
    [ForeignKey(nameof(DimensionalFactorID)), JsonIgnore]
    public virtual DimensionalFactorMaster? DimensionalFactor { get; set; }

    public long? SpecimenTypeID { get; set; }
    [ForeignKey(nameof(SpecimenTypeID))]
    public virtual SpecimenTypeMaster? SpecimenType { get; set; }

    [StringLength(100)]
    public required string SpecimenShape { get; set; }

    [StringLength(200)]
    public required string FormulaExpression { get; set; }

    [StringLength(100)]
    public string? Variables { get; set; }
}
