namespace LIMSApi.Dtos
{
    public class LedgerPeriodSummaryDto
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal ClosingBalance { get; set; }
        public List<LedgerEntryDto> Entries { get; set; } = new();
    }

    public class LedgerEntryDto
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Debit" or "Credit"
        public decimal Amount { get; set; }
        public decimal RunningBalance { get; set; }
    }
}
