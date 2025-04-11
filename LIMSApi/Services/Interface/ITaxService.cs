using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ITaxService
    {
        Task CreateTax(TaxMaster model);
        Task ModifyTax(TaxMaster model);
        Task RemoveTax(long id);
        Task<TaxMaster> GetTaxDetails(long id);
        Task<PagedResponse<object>> FetchTaxList(PageFilter filter);

        Task<List<DropdwonSelector>> GetTaxDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
