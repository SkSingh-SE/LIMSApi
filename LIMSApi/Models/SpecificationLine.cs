using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models;

public partial class SpecificationLine
{
    [Key]
    public long ID { get; set; }

    public long? SpecificationGradeID { get; set; }

    public bool? ManualSelection { get; set; }

    public long? ParameterID { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? MinValue { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? MaxValue { get; set; }

    public string? TextValue { get; set; }
    public string? InputType { get; set; }

    public string? Notes { get; set; }
    public string? Equation { get; set; }

    public long? ParameterUnitID { get; set; }

    // Phase 2: chosen equivalent of ParameterUnitID (null = base unit). FK to ParameterUnitEquivalent.
    public long? ParameterUnitEquivalentID { get; set; }

    // Test-method matrix: one Laboratory Test per parameter (the up-to-5 Test Method Specs live in
    // TestMethodMappings as TestMethodSpecificationID rows, DisplayOrder 1-5).
    public long? LaboratoryTestID { get; set; }

    // Formula expressions for equation-driven limits (authoring). Evaluated at test-result time
    // (cross-parameter, e.g. "Mg + 0.4*(C-56)") to produce MinValueEquation / MaxValueEquation.
    public string? MinEquation { get; set; }
    public string? MaxEquation { get; set; }

    // Runtime-computed equation limits (filled by the evaluator phase).
    [Column(TypeName = "decimal(18,6)")]
    public decimal? MinValueEquation { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? MaxValueEquation { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? MinTolerance { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? MaxTolerance { get; set; }
    public long? SpecimenOrientationID { get; set; }

    public long? DimensionalFactorID { get; set; }

    // MS-D: limit = symbol (≤ ≥ < > =) + decimal value. Symbol kept in *Value, number in *DecimalValue.
    public string? LowerLimitValue { get; set; }

    public string? UpperLimitValue { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? LowerLimitDecimalValue { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? UpperLimitDecimalValue { get; set; }

    public long? HeatTreatmentID { get; set; }
    public long? ProductConditionID1 { get; set; }
    public long? ProductConditionID2 { get; set; }

    // MS-D: product size band (→ ProductSizeMaster / MS8) + test condition + test note
    public long? ProductSizeMasterID { get; set; }
    public string? TestCondition { get; set; }
    public string? TestNote { get; set; }

    public string Type { get; set; } = "chemical"; // 'chemical' | 'mechanical' | 'other'


    [ForeignKey("ParameterID")]
    public virtual ParameterMaster? Parameter { get; set; }

    [ForeignKey("ProductSizeMasterID")]
    public virtual ProductSizeMaster? ProductSizeMaster { get; set; }
    [ForeignKey("ParameterUnitID")]
    public virtual ParameterUnitMaster? ParameterUnit { get; set; }
    [ForeignKey("ParameterUnitEquivalentID")]
    public virtual ParameterUnitEquivalent? ParameterUnitEquivalent { get; set; }
    [ForeignKey("LaboratoryTestID")]
    public virtual LaboratoryTest? LaboratoryTest { get; set; }

    [ForeignKey("SpecimenOrientationID")]
    public virtual SpecimenOrientationMaster? SpecimenOrientation { get; set; }
    [ForeignKey("DimensionalFactorID")]
    public virtual DimensionalFactorMaster? DimensionalFactor { get; set; }
    
    [ForeignKey("HeatTreatmentID")]
    public virtual HeatTreatmentMaster? HeatTreatment { get; set; }

    [ForeignKey("SpecificationGradeID"),JsonIgnore]
    public virtual SpecificationGrade? SpecificationGrade { get; set; }

    // MS-E: per-parameter test-method mapping (relational).
    public virtual ICollection<SpecificationLineTestMethod> TestMethodMappings { get; set; } = new List<SpecificationLineTestMethod>();

}
