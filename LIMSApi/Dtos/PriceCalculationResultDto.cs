namespace LIMSApi.Dtos
{
    public class PriceCalculationResultDto
    {
        public long InwardId { get; set; }
        public string CaseNo { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<PriceLineResultDto> TestResults { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public bool HasFailures => FailureCount > 0;
    }

    public class PriceLineResultDto
    {
        public long SampleId { get; set; }
        public string SampleNo { get; set; } = "";
        public string TestName { get; set; } = "";
        public string ChargeType { get; set; } = "";
        public bool Success { get; set; }
        public decimal Amount { get; set; }
        public string? FailureReason { get; set; }
    }
}
