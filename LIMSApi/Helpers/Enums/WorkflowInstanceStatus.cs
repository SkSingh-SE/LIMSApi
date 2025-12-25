namespace LIMSApi.Helpers.Enums
{
    public enum WorkflowInstanceStatus
    {
        InProgress,
        Completed,
        Rejected,
        Cancelled
    }
    public enum WorkFlowEntityType
    {
        Report_Review,
        Request_Review,
        TestResult
    }
    public static class WorkFlowEntityTypeExtensions
    {
        public static string GetEntityType(this WorkFlowEntityType type)
        {
            return type switch
            {
                WorkFlowEntityType.Report_Review => "Report Review",
                WorkFlowEntityType.Request_Review => "Request Review",
                WorkFlowEntityType.TestResult => "TestResult",
                _ => "Unknown"
            };
        }
    }
}
