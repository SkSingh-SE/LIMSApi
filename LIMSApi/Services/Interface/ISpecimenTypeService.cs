using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ISpecimenTypeService
    {
        Task CreateSpecimenType(SpecimenTypeMaster model);
        Task ModifySpecimenType(SpecimenTypeMaster model);
        Task<SpecimenTypeMaster> RemoveSpecimenType(long id);
        Task<SpecimenTypeMaster> GetSpecimenTypeDetails(long id);
        Task<PagedResponse<object>> FetchSpecimenTypeList(PageFilter filter);

        Task<List<DropdwonSelector>> GetSpecimenTypeDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
