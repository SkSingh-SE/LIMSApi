using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    [Table("PlanHistories")]
    public class PlanHistory : AuditProperty
    {
        [Key]
        public long Id { get; set; }

        public long PlanId { get; set; }  // FK to SampleTestPlan

        public int Version { get; set; }

        [MaxLength(50)]
        public string ChangeType { get; set; } = "Created";  // Created, Updated, Replanned, Approved, Rejected

        public long ChangedById { get; set; }

        [MaxLength(200)]
        public string? ChangedByName { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public string? PreviousDataJson { get; set; }  // JSON snapshot of plan before change

        public string? NewDataJson { get; set; }  // JSON snapshot after change

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public string? FieldChangesJson { get; set; }  // JSON array of {field, oldValue, newValue}

        [ForeignKey("PlanId"), JsonIgnore]
        public virtual SampleTestPlan? SampleTestPlan { get; set; }
    }
}
