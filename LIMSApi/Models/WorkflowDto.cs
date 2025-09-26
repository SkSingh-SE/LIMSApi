namespace LIMSApi.Models
{
    public class WorkflowDto
    {
        public long ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<WorkflowStepDto> Steps { get; set; } = new();
    }

    public class WorkflowStepDto
    {
        public long ID { get; set; }
        public int OrderNo { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AssignedToType { get; set; } = string.Empty;
        public string AssignedToValue { get; set; } = string.Empty;
        public List<WorkflowTransitionDto> Transitions { get; set; } = new();
    }

    public class WorkflowTransitionDto
    {
        public long ID { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Alias { get; set; }
        public long? ToStepID { get; set; }   
        public string? ToStepName { get; set; }// <-- use orderNo here, not Id

    }

}
