using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class HeatTreatmentMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(20)]
    public string? Code { get; set; }

    public long? HeatTreatmentCategoryID { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? TempRangeMin { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? TempRangeMax { get; set; }

    [StringLength(200)]
    public string? TempRangeDescription { get; set; }

    public long? CoolingMediumID { get; set; }

    [ForeignKey("HeatTreatmentCategoryID")]
    public virtual HeatTreatmentCategoryMaster? HeatTreatmentCategory { get; set; }

    [ForeignKey("CoolingMediumID")]
    public virtual CoolingMediumMaster? CoolingMedium { get; set; }

    public virtual ICollection<HeatTreatmentMetalClassification> ApplicableClassifications { get; set; } = new List<HeatTreatmentMetalClassification>();
}
