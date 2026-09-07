using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class DepartmentMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [Required]
    public long BranchID { get; set; }
    [ForeignKey("BranchID")]
    public virtual Branch? Branch { get; set; }

    public long? DisciplineID { get; set; }
    [ForeignKey("DisciplineID")]
    public virtual DisciplineMaster? Discipline { get; set; }

    [Required, StringLength(100)]
    public required string Name { get; set; }

    [StringLength(20)]
    public string? Code { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// True for chemical-analysis departments (Chemical Lab, Spectro Lab).
    /// Used to separate General vs Chemical test dropdowns in the plan form.
    /// </summary>
    public bool IsChemical { get; set; } = false;
}
