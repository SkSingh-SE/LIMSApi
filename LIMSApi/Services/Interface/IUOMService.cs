using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IUOMService
    {
        Task CreateUOM(UOMMaster model);
        Task ModifyUOM(UOMMaster model);
        Task RemoveUOM(long id);
        Task<UOMMaster> GetUOMDetails(long id);
        Task<PagedResponse<object>> FetchUOMList(PageFilter filter);

        Task<List<DropdwonSelector>> GetUOMDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
