namespace LIMSApi.Models
{
    public class NablQualityControlPlanActivity
    {
        public long ID { get; set; }

        // FK of Main QC Plan
        public long QualityControlPlanId { get; set; }
        public string? ActivityName { get; set; }
        public long? DepartmentID { get; set; }
        public long? TestMethodId { get; set; }
        public string? ReferenceType { get; set; }
        // CRM / Equipment selected ID
        public long? ReferenceId { get; set; }
        // CRM/Equipment display name OR manual textbox value
        public string? ReferenceName { get; set; }
        public string? FrequencyType { get; set; }
        // Example:
        // Daily = 2026-06-23
        // Weekly = 2026-W26
        // Monthly = 2026-06
        // Quarterly = Q1-2026
        // Half-Yearly = H1-2026
        // Yearly = 2026
        public string? FrequencyName { get; set; }
        public long? EmployeeId { get; set; }
        public string? AcceptanceCriteria { get; set; }
        public string? ResultStatus { get; set; }
        public string? Remarks { get; set; }
        public string? DepartmentName { get; set; }
        public string? EmployeeName { get; set; }
        public string? TestMethod { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? NextDueDate { get; set; }
        public bool IsActive { get; set; } = true;
        // Navigation
        public NablQualityControlPlan? QualityControlPlan { get; set; }
    }
}
