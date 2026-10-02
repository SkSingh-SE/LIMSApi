namespace LIMSApi.Dtos
{
    public class LifecycleSampleSummaryDto
    {
        public long SampleId { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public string SampleStatus { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? GradeName { get; set; }
        public string? MetalClassification { get; set; }
        public bool PreparationRequired { get; set; }
        public bool MachiningRequired { get; set; }
        public string? PreparationStatus { get; set; }
        public int GeneralTestCount { get; set; }
        public int ChemicalTestCount { get; set; }
        public string? TestResultStatus { get; set; }
        public bool IsTestingCompleted { get; set; }
        public string? ReportStatus { get; set; }
        public long? ReportHeaderId { get; set; }
        public string? ReportNo { get; set; }
        public int UniversalTestCount { get; set; }
        public int UniversalPendingCount { get; set; }
        public int UniversalInProgressCount { get; set; }
        public int UniversalCompletedCount { get; set; }
        public int UniversalVerifiedCount { get; set; }
        public int UniversalApprovedCount { get; set; }
        public string? LatestExecutionStatus { get; set; }
        public long? LatestUniversalTestGroupId { get; set; }
        public long? LatestExecutionId { get; set; }
        public long? LatestResultId { get; set; }
        public string? UniversalResultStatus { get; set; }
        public string? UniversalOverallDecision { get; set; }
        public string? UniversalReportStatus { get; set; }
        public long? UniversalReportId { get; set; }
        public string? UniversalReportNo { get; set; }
        public bool IsCancelled { get; set; }
        public string? CancellationReason { get; set; }
    }

    public class LifecycleSummaryDto
    {
        public long InwardId { get; set; }
        public string CaseNo { get; set; } = string.Empty;
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string InwardStatus { get; set; } = string.Empty;
        public string? ReviewStatus { get; set; }
        public DateTime? CollectionDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public int DaysSinceInward { get; set; }
        public int SampleCount { get; set; }
        public int ActiveSampleCount { get; set; }
        public int CancelledSampleCount { get; set; }

        public int TotalGeneralTests { get; set; }
        public int TotalChemicalTests { get; set; }
        public int TotalTests { get; set; }
        public int TotalUniversalTests { get; set; }
        public int UniversalPendingTests { get; set; }
        public int UniversalInProgressTests { get; set; }
        public int UniversalCompletedTests { get; set; }
        public int UniversalVerifiedTests { get; set; }
        public int UniversalApprovedTests { get; set; }
        public int ReleasedUniversalReports { get; set; }
        public bool HasReleasedUniversalReport { get; set; }

        public bool IsReportStopped { get; set; }
        public string? StopReportReason { get; set; }
        public bool IsClosed { get; set; }

        public bool HasProformaInvoice { get; set; }
        public string? ProformaInvoiceStatus { get; set; }
        public long? ProformaInvoiceId { get; set; }

        public bool HasTaxInvoice { get; set; }
        public string? TaxInvoiceStatus { get; set; }
        public long? TaxInvoiceId { get; set; }
        public decimal TotalBilledAmount { get; set; }
        public decimal TotalReceivedAmount { get; set; }
        public decimal BalanceDueAmount { get; set; }

        public List<LifecycleSampleSummaryDto> Samples { get; set; } = new List<LifecycleSampleSummaryDto>();
    }
}
