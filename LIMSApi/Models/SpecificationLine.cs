using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class SpecificationLine
{
    [Key]
    public long ID { get; set; }

    public long? GradeID { get; set; }

    public bool? ManualSelection { get; set; }

    public long? ParameterID { get; set; }

    public decimal? MinValue { get; set; }

    public decimal? MaxValue { get; set; }

    public string? Notes { get; set; }
    public string? Equation { get; set; }

    public long? ParameterUnitID { get; set; }
    public decimal? MinValueEquation { get; set; }
    public decimal? MaxValueEquation { get; set; } 
    public decimal? MinTolerance { get; set; }
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

    public ICollection<SpecificationLineLaboratoryTest> LaboratoryTests { get; set; } = new List<SpecificationLineLaboratoryTest>();
}
