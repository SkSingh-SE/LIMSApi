namespace LIMSApi.Dtos
{
    public class AuditDepartmentSummaryDto
    {
        public long? DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public int TotalAudits { get; set; }

        public int Completed { get; set; }

        public int InProgress { get; set; }

        public int Scheduled { get; set; }

        public int MajorNcrs { get; set; }

        public int MinorNcrs { get; set; }

        public int Observations { get; set; }
    }
}
