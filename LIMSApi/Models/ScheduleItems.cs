using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class ScheduleItems
    {
        [Key]
        public long ID { get; set; }

        public long AuditPlanId { get; set; }

        [ForeignKey(nameof(AuditPlanId))]
        [JsonIgnore]
        public virtual NablAuditPlan? AuditPlan { get; set; }

        public long DepartmentId { get; set; }

        [MaxLength(200)]
        public string? DepartmentName { get; set; }

        public string? ISOClausesJson { get; set; }

        [NotMapped]
        public List<AuditScheduleIsoClause>? IsoClauses { get; set; }

        public DateTime ScheduleDate { get; set; }

        public long AuditorId { get; set; }

        [MaxLength(200)]
        public string? AuditorName { get; set; }

        public long AuditeeId { get; set; }

        [MaxLength(200)]
        public string? AuditeeName { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Scheduled";

        public long? ChecklistId { get; set; }
        public bool? IsActive { get; set; }

    }

    [NotMapped]
    public class AuditScheduleIsoClause
    {
        public int? ClauseId { get; set; }

        public string? ClauseName { get; set; }
    }
}
