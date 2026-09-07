using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IConditionMasterRepository
    {
        Task AddConditionMaster(ConditionMaster model);
        Task UpdateConditionMaster(ConditionMaster model);
        Task DeleteConditionMaster(ConditionMaster model);
        Task<ConditionMaster?> GetConditionMasterById(long id);
        Task<ConditionMaster?> GetConditionMasterByCode(string code);
        Task<PagedResponse<object>> GetAllConditionMasters(PageFilter filter);
        Task<List<ConditionMasterDropdownDto>> GetConditionMasterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByCode(string code);
        Task<bool> ExistsByCodeAndNotId(string code, long id);
        Task<int> GetReferenceCount(long conditionMasterId);
    }
}
