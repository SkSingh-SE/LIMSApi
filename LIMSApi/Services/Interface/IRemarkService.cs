using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IRemarkService
    {
        Task CreateRemark(RemarkMaster model);
        Task ModifyRemark(RemarkMaster model);
        Task RemoveRemark(long id);
        Task<RemarkMaster> GetRemarkDetails(long id);
        Task<PagedResponse<object>> FetchRemarkList(PageFilter filter);

        Task<List<DropdwonSelector>> GetRemarkDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
