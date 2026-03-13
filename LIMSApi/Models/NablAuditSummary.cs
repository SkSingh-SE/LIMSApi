using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablAuditSummaries")]
    public class NablAuditSummary : NablFormBase
    {
        public long? AuditPlanId { get; set; }

        [ForeignKey("AuditPlanId")]
        public virtual NablAuditPlan? AuditPlan { get; set; }

        public DateTime? AuditDateFrom { get; set; }

        public DateTime? AuditDateTo { get; set; }

        public int? TotalAudits { get; set; }

        public int? TotalNCs { get; set; }

        public int? MajorNCs { get; set; }

        public int? MinorNCs { get; set; }

        public int? Observations { get; set; }

        public string? FindingsSummary { get; set; }

        public string? PositiveFindings { get; set; }

        [MaxLength(200)]
        public string? ClosureStatus { get; set; }

        public DateTime? NextAuditDate { get; set; }

        [MaxLength(200)]
        public string? SummaryBy { get; set; }
    }
}
