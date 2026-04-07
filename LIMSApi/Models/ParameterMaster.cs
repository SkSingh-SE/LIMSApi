using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class ParameterMaster : AuditProperty
{
    public long ID { get; set; }

    public string? ParameterType { get; set; } // Checmical or Mechanical

    [StringLength(100)]
    public required string Name { get; set; }

    public string? AliasName { get; set; }
    public long? ParameterUnitID {  get; set; }
    public string? ElementType { get; set; } = "normal";
    public bool IsCalculated { get; set; } = false;
    public string? Formula { get; set; }

    public string? Note { get; set; }

    [StringLength(20)]
    public string? Code { get; set; }

    [StringLength(20)]
    public string? Symbol { get; set; }

    public int DecimalPrecision { get; set; } = 2;

    [Column(TypeName = "decimal(18,6)")]
    public decimal? MinReportableLimit { get; set; }

    public long? DefaultTestMethodID { get; set; }

    public long? ParameterCategoryID { get; set; }

    [ForeignKey("ParameterUnitID")]
    public virtual ParameterUnitMaster? ParameterUnit { get; set; }

    [ForeignKey("DefaultTestMethodID")]
    public virtual TestMethodStandard? DefaultTestMethod { get; set; }

    [ForeignKey("ParameterCategoryID")]
    public virtual ParameterCategoryMaster? ParameterCategory { get; set; }

    public virtual ICollection<ParameterSpecimenOrientation> AllowedOrientations { get; set; } = new List<ParameterSpecimenOrientation>();
}
