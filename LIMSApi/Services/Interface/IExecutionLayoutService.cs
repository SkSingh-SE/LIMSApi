using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IExecutionLayoutService
    {
        Task CreateLayout(ExecutionLayoutCreateDto dto);
        Task ModifyLayout(ExecutionLayoutUpdateDto dto);
        Task<ExecutionLayoutDto> GetLayoutDetails(long id);
        Task<PagedResponse<object>> FetchLayoutList(ExecutionLayoutListRequest request);
        Task<bool> ToggleLayoutStatus(long id);
        Task<List<DropdwonSelector>> GetLayoutDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<ExecutionLayoutDto> ValidateLayout(long id);
        Task<ExecutionLayoutMetadataDto> GetMetadataAsync();
    }
}
