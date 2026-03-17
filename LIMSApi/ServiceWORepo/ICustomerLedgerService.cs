using LIMSApi.Dtos;

namespace LIMSApi.ServiceWORepo
{
    public interface ICustomerLedgerService
    {
        // Ledger Operations
        Task<List<CustomerLedgerEntryDto>> GetCustomerLedger(long customerId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<PagedResponse<CustomerLedgerEntryDto>> GetLedgerEntries(long customerId, PageFilter filter);
        Task<CustomerBalanceDto> GetCustomerBalance(long customerId);
        Task<CustomerStatementDto> GetCustomerStatement(long customerId, DateTime fromDate, DateTime toDate);

        // Ledger Entry Creation
        Task AddDebitEntry(long customerId, decimal amount, string referenceNo, string description, long? inwardId = null, long? invoiceId = null);
        Task AddCreditEntry(long customerId, decimal amount, string referenceNo, string description, string? paymentMode = null, string? chequeNo = null, string? bankName = null, string? transactionRef = null, long? inwardId = null, long? invoiceId = null);

        // Payment Recording
        Task<PaymentReceiptDto> RecordPayment(RecordPaymentDto dto);
        Task<List<PaymentReceiptDto>> GetPaymentReceipts(long customerId);
        Task<PaymentReceiptDto?> GetReceipt(long receiptId);
        Task<PagedResponse<PaymentReceiptDto>> GetReceiptsPaged(long customerId, PageFilter filter);

        // Reports
        Task<List<AgingReportDto>> GetAgingReport(DateTime? asOfDate = null);
        Task<OutstandingReportDto> GetOutstandingReport();
        Task<CollectionSummaryDto> GetCollectionSummary(DateTime from, DateTime to);

        // Credit Management
        Task<CreditStatusDto> GetCreditStatus(long customerId);
        Task<bool> CheckCreditLimit(long customerId, decimal newChargeAmount);
    }
}
