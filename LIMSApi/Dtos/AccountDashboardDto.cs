namespace LIMSApi.Dtos
{
    public class AccountDashboardDto
    {
        // Case status counts
        public int PiPendingCount { get; set; }
        public int InvoicePendingCount { get; set; }
        public int PaymentPendingCount { get; set; }
        public int FullySettledCount { get; set; }

        // Financial summary
        public decimal TotalRevenue { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal TotalOverdue { get; set; }
        public decimal TodayCollection { get; set; }

        // Customer type breakdown
        public List<CustomerTypeBreakdownDto> CustomerTypeBreakdown { get; set; } = new();
    }

    public class CustomerTypeBreakdownDto
    {
        public string Type { get; set; } = "";
        public int CaseCount { get; set; }
        public decimal OutstandingAmount { get; set; }
    }
}
