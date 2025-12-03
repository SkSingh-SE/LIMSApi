namespace LIMSApi.Dtos
{
    public class WorkflowActionRequestDto
    {
        public long Id { get; set; }
        public string Action { get; set; }   // Next, Cancel, Reject
        public string? Name { get; set; }   // Approve, Reject, etc.
        public string? Remarks { get; set; }
    }
}
