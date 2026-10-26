using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class ProductConditionMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(20)]
    public string? Code { get; set; }

    public long? ProductConditionCategoryID { get; set; }

    public long? LinkedHeatTreatmentID { get; set; }

    public bool CalibrationRequired { get; set; } = false;
    public bool IsDestructive { get; set; } = false;

    [ForeignKey("ProductConditionCategoryID")]
    public virtual ProductConditionCategoryMaster? ProductConditionCategory { get; set; }

    [ForeignKey("LinkedHeatTreatmentID")]
    public virtual HeatTreatmentMaster? LinkedHeatTreatment { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [NotMapped]
    public bool IsHeatTreated => LinkedHeatTreatmentID != null;

    public virtual ICollection<ProductConditionPropertyType> PropertiesCaptured { get; set; } = new List<ProductConditionPropertyType>();
}
