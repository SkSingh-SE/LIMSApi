using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IDimensionalFactorRepository
    {
        Task AddDimensionalFactor(DimensionalFactorMaster model);
        Task UpdateDimensionalFactor(DimensionalFactorMaster model);
        Task DeleteDimensionalFactor(long id);
        Task<DimensionalFactorMaster> GetDimensionalFactorById(long id);
        Task<PagedResponse<object>> GetAllDimensionalFactors(PageFilter filter);

        Task<List<DropdwonSelector>> GetDimensionalFactorDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
