using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IMakerRepository
    {
        Task AddMaker(MakerMaster model);
        Task UpdateMaker(MakerMaster model);
        Task DeleteMaker(long id);
        Task<MakerMaster> GetMakerById(long id);
        Task<PagedResponse<object>> GetAllMakers(PageFilter filter);

        Task<List<DropdwonSelector>> GetMakerDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
