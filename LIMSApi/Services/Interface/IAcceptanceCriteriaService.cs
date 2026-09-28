using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IAcceptanceCriteriaService
    {
        Task CreateAcceptanceCriteria(AcceptanceCriteriaMasterCreateDto dto);
        Task ModifyAcceptanceCriteria(AcceptanceCriteriaMasterUpdateDto dto);
        Task RemoveAcceptanceCriteria(long id);
        Task<AcceptanceCriteriaMasterDto> GetAcceptanceCriteriaDetails(long id);
        Task<PagedResponse<object>> FetchAcceptanceCriteriaList(PageFilter filter);
        Task<bool> ToggleAcceptanceCriteriaStatus(long id);
        Task<List<DropdwonSelector>> GetAcceptanceCriteriaDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
