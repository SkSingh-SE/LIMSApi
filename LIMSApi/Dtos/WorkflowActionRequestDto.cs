namespace LIMSApi.Dtos
{
    public class WorkflowActionRequestDto
    {
        public long Id { get; set; }
        public string Type { get; set; } = string.Empty;  // Report, Sample, TestResult, etc.
        public string? Action { get; set; } = string.Empty; // Next, Cancel, Reject
        public string? Name { get; set; }   // Approve, Reject, etc.
        public string? Remarks { get; set; }
    }
}
