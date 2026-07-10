namespace LIMSApi.Dtos
{
    public class RetestingQcPlanDetailsDto
    {
        public long QCPlanId { get; set; }
        public long QCPlanActivityId { get; set; }

        public string? PlanNo { get; set; }
        public int? PlanYear { get; set; }
        public string? Discipline { get; set; }
        public string? MaterialProductGroup { get; set; }
        public string? LabIncharge { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? QCActivity { get; set; }
        public string? DepartmentName { get; set; }
        public string? TestMethodName { get; set; }
        public string? ReferenceType { get; set; }
        public string? ReferenceName { get; set; }
        public string? FrequencyType { get; set; }
        public string? FrequencyName { get; set; }
        public string? ResponsibleEmployee { get; set; }
        public string? AcceptanceCriteria { get; set; }
        public DateTime? NextDueDate { get; set; }
    }
}
