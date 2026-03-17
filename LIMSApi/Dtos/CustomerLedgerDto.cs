namespace LIMSApi.Dtos
{
    public class RecordPaymentDto
    {
        public long CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMode { get; set; } = null!;
        public string? ChequeNo { get; set; }
        public string? BankName { get; set; }
        public string? TransactionRef { get; set; }
        public DateTime? ChequeDate { get; set; }
        public List<long>? InvoiceIds { get; set; }
        public long? InwardId { get; set; }
        public string? Remarks { get; set; }
    }

    public class CustomerLedgerEntryDto
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string TransactionType { get; set; } = null!;
        public string? ReferenceNo { get; set; }
        public string? Description { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Balance { get; set; }
        public string? PaymentMode { get; set; }
        public string? Remarks { get; set; }
    }

    public class CustomerStatementDto
    {
        public string CustomerName { get; set; } = null!;
        public long CustomerId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public List<CustomerLedgerEntryDto> Entries { get; set; } = new();
        public decimal ClosingBalance { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
    }

    public class AgingReportDto
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public decimal Current { get; set; }
        public decimal Days30 { get; set; }
        public decimal Days60 { get; set; }
        public decimal Days90 { get; set; }
        public decimal Days90Plus { get; set; }
        public decimal Total { get; set; }
    }

    public class OutstandingReportDto
    {
        public List<CustomerOutstandingDto> Customers { get; set; } = new();
        public decimal TotalOutstanding { get; set; }
    }

    public class CustomerOutstandingDto
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal Outstanding { get; set; }
        public DateTime? LastPaymentDate { get; set; }
    }

    public class CustomerBalanceDto
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public decimal Outstanding { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
    }

    public class PaymentReceiptDto
    {
        public long Id { get; set; }
        public string ReceiptNo { get; set; } = null!;
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMode { get; set; } = null!;
        public string? ChequeNo { get; set; }
        public string? BankName { get; set; }
        public string? TransactionRef { get; set; }
        public string? InvoiceIds { get; set; }
        public string? Remarks { get; set; }
        public string Status { get; set; } = null!;
    }

    public class CreditStatusDto
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerType { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }
        public int CreditLimitDays { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal AvailableCredit { get; set; }
        public bool IsOverLimit { get; set; }
        public bool IsOverdue { get; set; }
        public int? OldestUnpaidDays { get; set; }
    }

    public class CollectionSummaryDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal CashCollection { get; set; }
        public decimal ChequeCollection { get; set; }
        public decimal NEFTCollection { get; set; }
        public decimal UPICollection { get; set; }
        public decimal RazorpayCollection { get; set; }
        public int TotalTransactions { get; set; }
        public List<DailyCollectionDto> DailyBreakdown { get; set; } = new();
    }

    public class DailyCollectionDto
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
    }
}
