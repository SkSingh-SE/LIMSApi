namespace LIMSApi.ServiceWORepo
{
    public interface INablAuditService
    {
        Task<byte[]> GenerateAuditPackage(List<string> formTypes, DateTime? from, DateTime? to);
        Task<List<AuditSummaryItem>> GetAuditSummary();
        Task<NablDashboardDto> GetNablDashboard();
    }

    public class AuditSummaryItem
    {
        public string FormType { get; set; } = string.Empty;
        public string FormCode { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Draft { get; set; }
        public int Submitted { get; set; }
        public int Reviewed { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }

    public class AuditPackageRequest
    {
        public List<string> FormTypes { get; set; } = new();
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }

    public class NablDashboardDto
    {
        public int TotalForms { get; set; }
        public int DraftCount { get; set; }
        public int SubmittedCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public List<NablPendingReviewItem> PendingReviews { get; set; } = new();
        public List<NablUpcomingReviewItem> UpcomingReviews { get; set; } = new();
        public List<NablExpiringDocItem> ExpiringDocuments { get; set; } = new();
        public List<AuditSummaryItem> FormSummary { get; set; } = new();
    }

    public class NablPendingReviewItem
    {
        public long Id { get; set; }
        public string FormType { get; set; } = string.Empty;
        public string FormCode { get; set; } = string.Empty;
        public string? DocumentNo { get; set; }
        public string? Status { get; set; }
        public DateTime Date { get; set; }
        public string? PreparedBy { get; set; }
    }

    public class NablUpcomingReviewItem
    {
        public long Id { get; set; }
        public string FormType { get; set; } = string.Empty;
        public string FormCode { get; set; } = string.Empty;
        public string? DocumentNo { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public int DaysUntilReview { get; set; }
    }

    public class NablExpiringDocItem
    {
        public long Id { get; set; }
        public string FormType { get; set; } = string.Empty;
        public string FormCode { get; set; } = string.Empty;
        public string? DocumentNo { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public bool IsOverdue { get; set; }
    }
}
