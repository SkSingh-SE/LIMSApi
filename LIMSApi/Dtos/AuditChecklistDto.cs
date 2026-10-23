namespace LIMSApi.Dtos
{
    public class AuditChecklistDto
    {
        public long AuditPlanId { get; set; }
        public long ScheduleItemId { get; set; }
        public string? AuditPlanNo { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long? AuditorId { get; set; }
        public string? AuditorName { get; set; }
        public long? AuditeeId { get; set; }

        public string? AuditeeName { get; set; }
        public string? PlanNo{ get; set; }

        public DateTime? ScheduleDate { get; set; }

        public List<AuditChecklistIsoClauseDto> IsoClauses { get; set; } = new();
    }
    public class AuditChecklistIsoClauseDto
    {
        public int? ClauseId { get; set; }

        public string? ClauseName { get; set; }
    }
}
