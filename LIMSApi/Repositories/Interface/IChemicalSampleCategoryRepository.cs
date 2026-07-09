using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IChemicalSampleCategoryRepository
    {
        Task Add(ChemicalSampleCategory model);
        Task Update(ChemicalSampleCategory model);
        Task Delete(ChemicalSampleCategory model);
        Task<ChemicalSampleCategory?> GetById(long id);
        Task<PagedResponse<object>> GetAll(PageFilter filter);
        Task<List<DropdwonSelector>> GetDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
    }
}
