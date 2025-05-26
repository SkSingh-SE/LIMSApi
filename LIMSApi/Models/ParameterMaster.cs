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
    public long ParameterUnitID {  get; set; }
    public string? ElementType { get; set; } = "normal";

    public string? Note { get; set; }
    [ForeignKey("ParameterUnitID")]
    public virtual ParameterUnitMaster? ParameterUnit { get; set; }
}
