namespace LIMSApi.Dtos
{
    public class AuditChecklistNcrDto
    {
        public long ChecklistId { get; set; }
        public long ChecklistItemId { get; set; }
        public string? ChecklistNo { get; set; }
        public long? AuditPlanId { get; set; }
        public long? ScheduleItemId { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long? AuditorId { get; set; }
        public string? AuditorName { get; set; }
        public string? FindingType { get; set; }
        public string? AuditQuestion { get; set; }
        public string? ObjectiveEvidence { get; set; }
    }
}
