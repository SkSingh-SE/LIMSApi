using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IInvoiceCaseConfigurationRepository
    {
        Task AddInvoiceCaseConfiguration(InvoiceCaseConfiguration model);
        Task UpdateInvoiceCaseConfiguration(InvoiceCaseConfiguration model);
        Task<InvoiceCaseConfiguration> GetInvoiceCaseConfigurationById(long id);
        Task<PagedResponse<object>> GetAllInvoiceCaseConfigurations(PageFilter filter);

        Task<List<DropdwonSelector>> GetInvoiceCaseConfigurationDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
