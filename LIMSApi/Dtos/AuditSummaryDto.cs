namespace LIMSApi.Dtos
{
    public class AuditSummaryDto
    {
        // ==========================================
        // II. Audit Plan Details
        // ==========================================

        public long AuditPlanId { get; set; }

        public string? AuditPlanNo { get; set; }

        public string? AuditType { get; set; }

        public int? PlanningYear { get; set; }

        public string? LeadAuditor { get; set; }

        public DateTime? AuditFrom { get; set; }

        public DateTime? AuditTo { get; set; }

        public string? AuditCriteria { get; set; }

        public string? ScopeOfAudit { get; set; }

        public string? AuditObjective { get; set; }

        public string? OverallAuditStatus { get; set; }


        // ==========================================
        // III. Audit Execution Summary
        // ==========================================

        public int TotalAudits { get; set; }

        public int Completed { get; set; }

        public int InProgress { get; set; }

        public int Scheduled { get; set; }


        // ==========================================
        // IV. Audit Findings Summary
        // ==========================================

        public int TotalNcrs { get; set; }

        public int MajorNcrs { get; set; }

        public int MinorNcrs { get; set; }

        public int Observations { get; set; }

        public int ClosedNcrs { get; set; }

        public int PendingNcrs { get; set; }


        // ==========================================
        // V. Department-wise Summary
        // ==========================================

        public List<AuditDepartmentSummaryDto> DepartmentSummary { get; set; } = new();
    }
}
