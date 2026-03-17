namespace LIMSApi.Dtos
{
    public class NablScopeCheckResult
    {
        public long ParameterId { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public decimal? Value { get; set; }
        public decimal? NablLowerLimit { get; set; }
        public decimal? NablUpperLimit { get; set; }
        public string ScopeStatus { get; set; } = string.Empty; // "WithinScope" | "OutsideScope" | "NotAccredited"
        public bool IsUnderISO { get; set; }
        public long? LabScopeSpecParamId { get; set; }
    }

    public class UncertaintyResult
    {
        public long ParameterId { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public decimal? ExpandedUncertainty { get; set; }
        public decimal? CombinedUncertainty { get; set; }
        public decimal? CoverageFactor { get; set; }
        public string? ConfidenceLevel { get; set; }
        public long? NablMeasurementUncertaintyId { get; set; }
    }
}
