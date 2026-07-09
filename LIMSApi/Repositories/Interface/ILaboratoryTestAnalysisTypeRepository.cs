using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ILaboratoryTestAnalysisTypeRepository
    {
        Task Add(LaboratoryTestAnalysisType model);
        Task Update(LaboratoryTestAnalysisType model);
        Task<LaboratoryTestAnalysisType?> GetById(long id);
        Task<List<LaboratoryTestAnalysisType>> GetBySubGroupId(long subGroupId);
        Task<List<DropdwonSelector>> GetDropdown(long subGroupId);
        Task<bool> ExistsByNamePerSubGroup(string name, long subGroupId, long? excludeId);
        Task<PagedResponse<object>> GetAll(PageFilter filter);
    }
}
