using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IInvoiceCaseRepository
    {
        Task AddInvoiceCase(InvoiceCase model);
        Task UpdateInvoiceCase(InvoiceCase model);
        Task<InvoiceCase> GetInvoiceCaseById(long id);
        Task<PagedResponse<object>> GetAllInvoiceCases(PageFilter filter);

        Task<List<DropdwonSelector>> GetInvoiceCaseDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByFinancialAndName(string financialYear, string name);
        Task<bool> ExistsByFinancialYearAndTest(string financialYear, long laboratoryTestId);
        Task<bool> ExistsByFinancialYearAndTestNotId(string financialYear, long laboratoryTestId, long excludeId);
    }
}
