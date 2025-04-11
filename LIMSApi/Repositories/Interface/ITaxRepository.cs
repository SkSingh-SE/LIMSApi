using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ITaxRepository
    {
        Task AddTax(TaxMaster model);
        Task UpdateTax(TaxMaster model);
        Task DeleteTax(TaxMaster model);
        Task<TaxMaster> GetTaxById(long id);
        Task<PagedResponse<object>> GetAllTaxs(PageFilter filter);

        Task<List<DropdwonSelector>> GetTaxDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
