using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablAuditChecklists")]
    public class NablAuditChecklist : NablFormBase
    {
        public long? AuditPlanId { get; set; }

        [ForeignKey("AuditPlanId")]
        public virtual NablAuditPlan? AuditPlan { get; set; }

        public DateTime? AuditDate { get; set; }

        public long? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster? Department { get; set; }

        [MaxLength(200)]
        public string? DepartmentName { get; set; }

        public long? AuditorId { get; set; }

        [ForeignKey("AuditorId")]
        public virtual EmployeeMaster? Auditor { get; set; }

        [MaxLength(200)]
        public string? AuditorName { get; set; }

        public long? AuditeeId { get; set; }

        [ForeignKey("AuditeeId")]
        public virtual EmployeeMaster? Auditee { get; set; }

        [MaxLength(200)]
        public string? AuditeeName { get; set; }

        [MaxLength(200)]
        public string? ISOClause { get; set; }
        public int? NCCount { get; set; }
        public int? ObservationCount { get; set; }
        public string? ChecklistNo { get; set; }
        public string? AuditPlanNo { get; set; }
        public long ScheduleItemId { get; set; }

        [MaxLength(100)]
        public string? ChecklistStatus { get; set; }

        public string? Remarks { get; set; }

        public virtual ICollection<AuditChecklistItem> Items { get; set; } = new List<AuditChecklistItem>();

    }
}
