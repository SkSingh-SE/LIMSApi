using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISpecimenTypeRepository
    {
        Task<SpecimenTypeMaster> AddSpecimenType(SpecimenTypeMaster model);
        Task<SpecimenTypeMaster> UpdateSpecimenType(SpecimenTypeMaster model);
        Task<SpecimenTypeMaster> DeleteSpecimenType(long id);
        Task<SpecimenTypeMaster> GetSpecimenTypeById(long id);
        Task<PagedResponse<object>> GetAllSpecimenTypes(PageFilter filter);

        Task<List<DropdwonSelector>> GetSpecimenTypeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
