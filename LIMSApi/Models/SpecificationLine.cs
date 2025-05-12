using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class SpecificationLine : AuditProperty
{
    [Key]
    public long ID { get; set; }

    public long? SpecificationHeaderID { get; set; }

    public string? PropertyType { get; set; }

    public bool? ManualSelection { get; set; }

    public long? ParameterID { get; set; }

    public decimal? MinValue { get; set; }

    public decimal? MaxValue { get; set; }

    public string? Notes { get; set; }

    public long? ParameterUnitID { get; set; }
    public decimal? MinValueEquation { get; set; }
    public decimal? MaxValueEquation { get; set; } 
    public decimal? MinTolerance { get; set; }
    public decimal? MaxTolerance { get; set; }
    public long? SpecimenOrientationID { get; set; }

    public long? DimensionalFactorID { get; set; }

    public decimal? LowerLimitValue { get; set; }

    public decimal? UpperLimitValue { get; set; }

    public long? HeatTreatmentID { get; set; }

    public long? ProductConditionID1 { get; set; }

    public long? ProductConditionID2 { get; set; }

    public long? LaboratoryTestID1 { get; set; }
    public long? LaboratoryTestID2 { get; set; }

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
    
    [ForeignKey("ProductConditionID1")]
    public virtual ProductConditionMaster? ProductCondition1 { get; set; }

    [ForeignKey("ProductConditionID2")]
    public virtual ProductConditionMaster? ProductCondition2 { get; set; }
}
