using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IMakerService
    {
        Task CreateMaker(MakerMaster model);
        Task ModifyMaker(MakerMaster model);
        Task RemoveMaker(long id);
        Task<MakerMaster> GetMakerDetails(long id);
        Task<PagedResponse<object>> FetchMakerList(PageFilter filter);

        Task<List<DropdwonSelector>> GetMakerDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
