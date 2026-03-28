namespace LIMSApi.Dtos
{
    public class SuggestedTestDto
    {
        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestName { get; set; } = string.Empty;
        public string SubGroup { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty; // "Specification", "ProductTestGroup"
        public bool IsPerBatch { get; set; }
        public long? TestMethodStandardID { get; set; }
        public string? TestMethodStandardName { get; set; }
        public int Score { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    public class TestAutoSuggestResult
    {
        public List<SuggestedTestDto> SuggestedTests { get; set; } = new();
        public int SpecificationTestCount { get; set; }
        public int ProductTestGroupCount { get; set; }
    }

    public class SmartSuggestRequest
    {
        public long? SpecificationGradeId { get; set; }
        public long? MetalClassificationId { get; set; }
        public long? ProductConditionId { get; set; }
        public long? CustomerId { get; set; }
    }

    public class SmartSuggestResult
    {
        public List<SuggestedTestDto> SuggestedTests { get; set; } = new();
        public int SpecMatchCount { get; set; }
        public int CustomerHistoryCount { get; set; }
        public int GlobalFrequencyCount { get; set; }
        public int TrendingCount { get; set; }
    }
}
