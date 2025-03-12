using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISpecimenOrientationRepository
    {
        Task AddSpecimenOrientation(SpecimenOrientationMaster model);
        Task UpdateSpecimenOrientation(SpecimenOrientationMaster model);
        Task DeleteSpecimenOrientation(long id);
        Task<SpecimenOrientationMaster> GetSpecimenOrientationById(long id);
        Task<PagedResponse<object>> GetAllSpecimenOrientations(PageFilter filter);

        Task<List<DropdwonSelector>> GetSpecimenOrientationDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
    }
}
