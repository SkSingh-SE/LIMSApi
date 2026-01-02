namespace LIMSApi.Dtos
{
    public class CaseAccountSummaryDto
    {
        public long InwardID { get; set; }
        public string CaseNo { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string CustomerType { get; set; } = null!;

        public string PIStatus { get; set; } = null!;
        public string InvoiceStatus { get; set; } = null!;
        public bool HasPendingPayment { get; set; }
    }

}
