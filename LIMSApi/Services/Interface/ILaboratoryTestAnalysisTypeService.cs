using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ILaboratoryTestAnalysisTypeService
    {
        Task Create(LaboratoryTestAnalysisType model);
        Task Modify(LaboratoryTestAnalysisType model);
        Task Remove(long id);
        Task<LaboratoryTestAnalysisType> GetDetails(long id);
        Task<PagedResponse<object>> FetchList(PageFilter filter);
        Task<List<DropdwonSelector>> GetDropdown(long subGroupId);
        Task<List<LaboratoryTestAnalysisType>> GetBySubGroupId(long subGroupId);
    }
}
