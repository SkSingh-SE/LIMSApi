using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class DimensionalFactorMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(20)]
    public string? Code { get; set; }

    public long? ParameterUnitID { get; set; }

    [StringLength(100)]
    public string? Instrument { get; set; }

    [StringLength(100)]
    public string? ToleranceType { get; set; }

    public long? DefaultTestMethodID { get; set; }

    [ForeignKey("ParameterUnitID")]
    public virtual ParameterUnitMaster? ParameterUnit { get; set; }

    [ForeignKey("DefaultTestMethodID")]
    public virtual TestMethodSpecification? DefaultTestMethod { get; set; }

    public virtual ICollection<DimensionalFactorProductForm> ApplicableForms { get; set; } = new List<DimensionalFactorProductForm>();
}
