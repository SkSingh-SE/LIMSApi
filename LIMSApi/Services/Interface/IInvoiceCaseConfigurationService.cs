using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IInvoiceCaseConfigurationService
    {
        Task CreateInvoiceCaseConfiguration(List<InvoiceCaseConfiguration> configurations);
        Task ModifyInvoiceCaseConfiguration(List<InvoiceCaseConfiguration> configurations);
        Task RemoveInvoiceCaseConfiguration(long id);
        Task<InvoiceCaseConfiguration> GetInvoiceCaseConfigurationDetails(long id);
        Task<PagedResponse<object>> FetchInvoiceCaseConfigurationList(PageFilter filter);

        Task<List<DropdwonSelector>> GetInvoiceCaseConfigurationDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
