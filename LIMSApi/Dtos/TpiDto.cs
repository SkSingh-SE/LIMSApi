namespace LIMSApi.Dtos
{
    public class TpiInspectionDto
    {
        public long Id { get; set; }
        public long SampleInwardId { get; set; }
        public string? SampleInwardNumber { get; set; }
        public long? SampleDetailId { get; set; }
        public string? SampleNo { get; set; }
        public long TpiMasterId { get; set; }
        public string? TpiName { get; set; }
        public string Stage { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? ScheduledDate { get; set; }
        public DateTime? InspectionDate { get; set; }
        public string? Remarks { get; set; }
        public string? InspectorComments { get; set; }
        public string? DocumentPath { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class CreateTpiInspectionDto
    {
        public long SampleInwardID { get; set; }
        public long? SampleDetailID { get; set; }
        public long TPIMasterID { get; set; }
        public string Stage { get; set; } = string.Empty;
        public DateTime? ScheduledDate { get; set; }
        public string? Remarks { get; set; }
    }

    public class UpdateTpiStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? InspectorComments { get; set; }
        public string? DocumentPath { get; set; }
    }
}
