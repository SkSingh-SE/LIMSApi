using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ILaboratoryTestSubGroupService
    {
        Task Create(LaboratoryTestSubGroup model);
        Task Modify(LaboratoryTestSubGroup model);
        Task Remove(long id);
        Task<LaboratoryTestSubGroup> GetDetails(long id);
        Task<PagedResponse<object>> FetchList(PageFilter filter);
        Task<List<DropdwonSelector>> GetDropdown(long labTestId);
        Task<List<LaboratoryTestSubGroup>> GetByLabTestId(long labTestId);
        Task<List<DropdwonSelector>> GetTestMethodSpecificationBySubGroupId(long subGroupId);
    }
}
