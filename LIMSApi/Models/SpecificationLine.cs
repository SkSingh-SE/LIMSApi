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

    public string? Notes { get; set; }
    public string? Equation { get; set; }

    public long? ParameterUnitID { get; set; }

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

    public string? LowerLimitValue { get; set; }

    public string? UpperLimitValue { get; set; }

    public long? HeatTreatmentID { get; set; }
    public long? ProductConditionID1 { get; set; }
    public long? ProductConditionID2 { get; set; }
    public string Type { get; set; } = "chemical"; // 'chemical' | 'mechanical' | 'other'


    [ForeignKey("ParameterID")]
    public virtual ParameterMaster? Parameter { get; set; }
    [ForeignKey("ParameterUnitID")]
    public virtual ParameterUnitMaster? ParameterUnit { get; set; }

    [ForeignKey("SpecimenOrientationID")]
    public virtual SpecimenOrientationMaster? SpecimenOrientation { get; set; }
    [ForeignKey("DimensionalFactorID")]
    public virtual DimensionalFactorMaster? DimensionalFactor { get; set; }
    
    [ForeignKey("HeatTreatmentID")]
    public virtual HeatTreatmentMaster? HeatTreatment { get; set; }

    [ForeignKey("SpecificationGradeID"),JsonIgnore]
    public virtual SpecificationGrade? SpecificationGrade { get; set; }

}
