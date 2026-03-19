using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ISpecimenOrientationCategoryService
    {
        Task CreateSpecimenOrientationCategory(SpecimenOrientationCategoryMaster model);
        Task ModifySpecimenOrientationCategory(SpecimenOrientationCategoryMaster model);
        Task RemoveSpecimenOrientationCategory(long id);
        Task<SpecimenOrientationCategoryMaster> GetSpecimenOrientationCategoryDetails(long id);
        Task<PagedResponse<object>> FetchSpecimenOrientationCategoryList(PageFilter filter);
        Task<List<DropdwonSelector>> GetSpecimenOrientationCategoryDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
