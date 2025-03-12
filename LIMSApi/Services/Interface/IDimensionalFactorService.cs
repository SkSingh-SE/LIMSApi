using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IDimensionalFactorService
    {
        Task CreateDimensionalFactor(DimensionalFactorMaster model);
        Task ModifyDimensionalFactor(DimensionalFactorMaster model);
        Task RemoveDimensionalFactor(long id);
        Task<DimensionalFactorMaster> GetDimensionalFactorDetails(long id);
        Task<PagedResponse<object>> FetchDimensionalFactorList(PageFilter filter);

        Task<List<DropdwonSelector>> GetDimensionalFactorDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
