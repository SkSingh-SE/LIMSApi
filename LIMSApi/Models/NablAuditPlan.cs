using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablAuditPlans")]
    public class NablAuditPlan : NablFormBase
    {
        public int? AuditYear { get; set; }

        // Fields matching frontend form (audit-plan-form.component.ts)
        [MaxLength(100)]
        public string? AuditType { get; set; } // Internal Audit / External Audit

        [MaxLength(200)]
        public string? Period { get; set; } // e.g. "Jan 2026 - Dec 2026"

        [MaxLength(200)]
        public string? AreaDepartment { get; set; }

        [MaxLength(200)]
        public string? AuditorName { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public string? AuditScope { get; set; } // maps to frontend "scope"

        // Extended audit planning fields
        public string? AuditScheduleJson { get; set; } // JSON array of {month, areaAudited, plannedDate, auditorId, auditorName, status}

        public string? AuditObjective { get; set; }

        public string? AuditCriteria { get; set; }

        public long? LeadAuditorId { get; set; }

        [ForeignKey("LeadAuditorId")]
        public virtual EmployeeMaster? LeadAuditor { get; set; }

        [MaxLength(200)]
        public string? LeadAuditorName { get; set; }
    }
}
