namespace LIMSApi.Dtos
{
    public class PricingTemplateRowDto
    {
        public long InvoiceCaseConfigID { get; set; }
        public string ConfigName { get; set; } = string.Empty;
        public string SelectionType { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;    // e.g. ">=3", "OVERRIDE", "FlatRate"
        public string GroupName { get; set; } = string.Empty;       // SubGroup or AnalysisType name
        public string GroupType { get; set; } = string.Empty;       // "SubGroup" | "AnalysisType"
        public bool IsOverride { get; set; }                        // true when ConfigValue == "OVERRIDE"
        public string? OverrideParameterIDs { get; set; }           // null = wildcard, "12,15" = specific
        public string? OverrideParameterNames { get; set; }         // resolved parameter names
    }
}
