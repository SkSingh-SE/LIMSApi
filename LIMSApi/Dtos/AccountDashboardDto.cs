namespace LIMSApi.Dtos
{
    public class AccountDashboardDto
    {
        public int PiPendingCount { get; set; }
        public int InvoicePendingCount { get; set; }
        public int PaymentPendingCount { get; set; }
        public int FullySettledCount { get; set; }
    }

}
