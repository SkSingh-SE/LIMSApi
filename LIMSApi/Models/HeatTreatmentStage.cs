using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models;

public partial class HeatTreatmentStage : AuditProperty
{
    [Key]
    public long ID { get; set; }

    public long HeatTreatmentID { get; set; }
    [ForeignKey(nameof(HeatTreatmentID)), JsonIgnore]
    public virtual HeatTreatmentMaster? HeatTreatment { get; set; }

    public int StageNumber { get; set; } = 1;

    [StringLength(100)]
    public required string StageName { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TempRangeMin { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TempRangeMax { get; set; }

    [StringLength(200)]
    public string? SoakingDescription { get; set; }

    public long? CoolingMediumID { get; set; }
    [ForeignKey(nameof(CoolingMediumID))]
    public virtual CoolingMediumMaster? CoolingMedium { get; set; }
}
