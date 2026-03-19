using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.ServiceWORepo
{
    public interface IAccountService
    {
        Task<AccountDashboardDto> GetDashboardAsync();
        Task<PagedResponse<object>> GetCaseAccountListAsync(PageFilter filter);
        Task<CaseAccountSummaryDto> GetCaseAccountSummaryAsync(long inwardId);

        Task<PagedResponse<object>> GetCasePaymentListAsync(long inwardId, PageFilter filter);
        Task CreatePriceSnapshotAsync(long inwardId);
        Task<long> GenerateInvoiceAsync(long inwardId);
        Task SendInvoiceAsync(long invoiceId, bool sendEmail, bool sendWhatsApp);
        Task<long> GenerateProformaInvoiceAsync(long inwardId);

        // Ledger period-based summary
        Task<LedgerPeriodSummaryDto> GetLedgerPeriodSummaryAsync(long customerId, DateTime periodStart, DateTime periodEnd);

        // Invoice Line Items (ad-hoc charges)
        Task<List<InvoiceLineItem>> GetLineItemsAsync(long proformaInvoiceHeaderId);
        Task<InvoiceLineItem> CreateLineItemAsync(InvoiceLineItemDto dto);
        Task<InvoiceLineItem> UpdateLineItemAsync(long id, InvoiceLineItemDto dto);
        Task DeleteLineItemAsync(long id);
    }

}
