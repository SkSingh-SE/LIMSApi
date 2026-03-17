namespace LIMSApi.Dtos
{
    public class MachineDataDto
    {
        public string? MachineId { get; set; }
        public long? TestResultHeaderId { get; set; }
        public long? EquipmentId { get; set; }
        public List<MachineParameterReading> Readings { get; set; } = new();
        public DateTime? Timestamp { get; set; }
        public string? RawDataJson { get; set; }
    }

    public class MachineParameterReading
    {
        public string ParameterName { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string? Unit { get; set; }
    }

    public class MachineDataResultDto
    {
        public int TotalReadings { get; set; }
        public int MatchedCount { get; set; }
        public int UnmatchedCount { get; set; }
        public List<string> MatchedParameters { get; set; } = new();
        public List<string> UnmatchedParameters { get; set; } = new();
        public long? LogId { get; set; }
    }
}
