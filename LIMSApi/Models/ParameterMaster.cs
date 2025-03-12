using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models;

public partial class ParameterMaster : AuditProperty
{
    public long ID { get; set; }

    public string? ParameterType { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    public string? AliasName { get; set; }

    public int? UOMID { get; set; }

    public string? Notes { get; set; }
}
