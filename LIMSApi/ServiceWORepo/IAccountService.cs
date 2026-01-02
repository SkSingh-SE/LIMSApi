using LIMSApi.Dtos;

namespace LIMSApi.ServiceWORepo
{
    public interface IAccountService
    {
        Task<AccountDashboardDto> GetDashboardAsync();
        Task<PagedResponse<object>> GetCaseAccountListAsync(PageFilter filter);
        Task<CaseAccountSummaryDto> GetCaseAccountSummaryAsync(long inwardId);

        Task<PagedResponse<object>> GetCasePaymentListAsync(long inwardId, PageFilter filter);
        Task<long> GenerateInvoiceAsync(long inwardId);
        Task SendInvoiceAsync(long invoiceId, bool sendEmail, bool sendWhatsApp);
    }

}
