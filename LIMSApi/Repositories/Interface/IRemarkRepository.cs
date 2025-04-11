using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IRemarkRepository
    {
        Task AddRemark(RemarkMaster model);
        Task UpdateRemark(RemarkMaster model);
        Task DeleteRemark(RemarkMaster model);
        Task<RemarkMaster> GetRemarkById(long id);
        Task<PagedResponse<object>> GetAllRemarks(PageFilter filter);

        Task<List<DropdwonSelector>> GetRemarkDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
