using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models;

public partial class DepartmentMaster:AuditProperty
{
    [Key]
    public long ID { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// True for chemical-analysis departments (Chemical Lab, Spectro Lab).
    /// Used to separate General vs Chemical test dropdowns in the plan form.
    /// </summary>
    public bool IsChemical { get; set; } = false;
}
