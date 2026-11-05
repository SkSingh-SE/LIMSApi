using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

/// <summary>
/// Dimensional Factor Master entity.
/// NOTE (NABL Precision Standard): Decimal places and rounding precision are auto-inherited 
/// from the bound Parameter Master in Material Specification (not stored on DimensionalFactorMaster directly).
/// </summary>
public partial class DimensionalFactorMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(20)]
    public string? Code { get; set; }

    public long? ParameterUnitID { get; set; }

    public long? ParameterUnitEquivalentID { get; set; }

    [StringLength(100)]
    public string? Instrument { get; set; }

    [StringLength(100)]
    public string? ToleranceType { get; set; }

    public long? DefaultTestMethodID { get; set; }

    [ForeignKey("ParameterUnitID")]
    public virtual ParameterUnitMaster? ParameterUnit { get; set; }

    [ForeignKey(nameof(ParameterUnitEquivalentID))]
    public virtual ParameterUnitEquivalent? ParameterUnitEquivalent { get; set; }

    [ForeignKey("DefaultTestMethodID")]
    public virtual TestMethodSpecification? DefaultTestMethod { get; set; }

    [StringLength(50)]
    public string? FactorType { get; set; } = "Measured";

    public long? DefaultEquipmentTypeID { get; set; }

    [ForeignKey(nameof(DefaultEquipmentTypeID))]
    public virtual EquipmentTypeMaster? DefaultEquipmentType { get; set; }

    public virtual ICollection<DimensionalFactorProductForm> ApplicableForms { get; set; } = new List<DimensionalFactorProductForm>();

    public virtual ICollection<DimensionalFactorShapeFormula> ShapeFormulas { get; set; } = new List<DimensionalFactorShapeFormula>();
}
