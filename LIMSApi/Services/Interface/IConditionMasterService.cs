using LIMSApi.Dtos;
using LIMSApi.Helpers;

namespace LIMSApi.Services.Interface
{
    public interface IConditionMasterService
    {
        Task CreateConditionMaster(ConditionMasterCreateDto dto);
        Task ModifyConditionMaster(ConditionMasterUpdateDto dto);
        Task<bool> ToggleConditionMasterStatus(long id);
        Task DeleteConditionMaster(long id);
        Task<ConditionMasterDto?> GetConditionMasterDetails(long id);
        Task<PagedResponse<object>> FetchConditionMasterList(PageFilter filter);
        Task<List<ConditionMasterDropdownDto>> GetConditionMasterDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
