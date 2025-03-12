using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IParameterService
    {
        Task CreateParameter(ParameterMaster model);
        Task ModifyParameter(ParameterMaster model);
        Task RemoveParameter(long id);
        Task<ParameterMaster> GetParameterDetails(long id);
        Task<PagedResponse<object>> FetchParameterList(PageFilter filter);

        Task<List<DropdwonSelector>> GetParameterDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
