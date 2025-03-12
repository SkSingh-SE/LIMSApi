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

    public long? UOMID { get; set; }

    public long? SpecimenOrientationID { get; set; }

    public long? DimensionalFactorID { get; set; }

    public string? LowerLimit { get; set; }

    public decimal? LowerLimitValue { get; set; }

    public string? UpperLimit { get; set; }

    public decimal? UpperLimitValue { get; set; }

    public long? HeatTreatmentID { get; set; }

    public long? ProductConditionID1 { get; set; }

    public long? ProductConditionID2 { get; set; }

    [ForeignKey("SpecificationHeaderID")]
    public virtual SpecificationHeader? SpecificationHeader { get; set; }

    [ForeignKey("ParameterID")]
    public virtual ParameterMaster? Parameter { get; set; }
    [ForeignKey("UOMID")]
    public virtual UOMMaster? UOM { get; set; }

    [ForeignKey("SpecimenOrientationID")]
    public virtual SpecimenOrientationMaster? SpecimenOrientation { get; set; }
    [ForeignKey("DimensionalFactorID")]
    public virtual DimensionalFactorMaster? DimensionalFactor { get; set; }
    
    [ForeignKey("HeatTreatmentID")]
    public virtual HeatTreatmentMaster? HeatTreatment { get; set; }
    
    [ForeignKey("ProductConditionID1")]
    public virtual ProductConditionMaster? ProductCondition { get; set; }
}
