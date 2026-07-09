using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IChemicalSampleCategoryService
    {
        Task Create(ChemicalSampleCategory model);
        Task Modify(ChemicalSampleCategory model);
        Task Remove(long id);
        Task<ChemicalSampleCategory> GetDetails(long id);
        Task<PagedResponse<object>> FetchList(PageFilter filter);
        Task<List<DropdwonSelector>> GetDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
