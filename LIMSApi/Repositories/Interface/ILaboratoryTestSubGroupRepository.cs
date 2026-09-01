using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ILaboratoryTestSubGroupRepository
    {
        Task Add(LaboratoryTestSubGroup model);
        Task Update(LaboratoryTestSubGroup model);
        Task<LaboratoryTestSubGroup?> GetById(long id);
        Task<List<LaboratoryTestSubGroup>> GetByLabTestId(long labTestId);
        Task<List<DropdwonSelector>> GetDropdown(long labTestId);
        Task<bool> ExistsByNamePerLabTest(string name, long labTestId, long? excludeId);
        Task<PagedResponse<object>> GetAll(PageFilter filter);
    }
}
