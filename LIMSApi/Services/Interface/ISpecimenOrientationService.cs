using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ISpecimenOrientationService
    {
        Task CreateSpecimenOrientation(SpecimenOrientationMaster model);
        Task ModifySpecimenOrientation(SpecimenOrientationMaster model);
        Task RemoveSpecimenOrientation(long id);
        Task<SpecimenOrientationMaster> GetSpecimenOrientationDetails(long id);
        Task<PagedResponse<object>> FetchSpecimenOrientationList(PageFilter filter);

        Task<List<DropdwonSelector>> GetSpecimenOrientationDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
