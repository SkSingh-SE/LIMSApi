using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IInvoiceCaseService
    {
        Task CreateInvoiceCase(InvoiceCase model);
        Task ModifyInvoiceCase(InvoiceCase model);
        Task RemoveInvoiceCase(long id);
        Task<InvoiceCase> GetInvoiceCaseDetails(long id);
        Task<PagedResponse<object>> FetchInvoiceCaseList(PageFilter filter);

    }
}
