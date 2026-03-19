using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISpecimenOrientationCategoryRepository
    {
        Task AddSpecimenOrientationCategory(SpecimenOrientationCategoryMaster model);
        Task UpdateSpecimenOrientationCategory(SpecimenOrientationCategoryMaster model);
        Task DeleteSpecimenOrientationCategory(long id);
        Task<SpecimenOrientationCategoryMaster> GetSpecimenOrientationCategoryById(long id);
        Task<PagedResponse<object>> GetAllSpecimenOrientationCategorys(PageFilter filter);
        Task<List<DropdwonSelector>> GetSpecimenOrientationCategoryDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
