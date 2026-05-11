using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class MetalClassificationMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(20)]
    public string? Code { get; set; }

    public long? ParentID { get; set; }

    public bool HasChemicalParams { get; set; } = false;
    public bool HasMechanicalParams { get; set; } = false;
    public int SortOrder { get; set; } = 0;

    [StringLength(20)]
    public string? MetalType { get; set; }

    [ForeignKey("ParentID")]
    public virtual MetalClassificationMaster? Parent { get; set; }
    public virtual ICollection<MetalClassificationMaster> Children { get; set; } = new List<MetalClassificationMaster>();
    public virtual ICollection<MetalClassificationParameter> Parameters { get; set; } = new List<MetalClassificationParameter>();
}
