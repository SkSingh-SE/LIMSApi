using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ICourierRepository
    {
        Task AddCourier(CourierMaster model);
        Task UpdateCourier(CourierMaster model);
        Task DeleteCourier(CourierMaster model);
        Task<CourierMaster> GetCourierById(long id);
        Task<PagedResponse<object>> GetAllCouriers(PageFilter filter);

        Task<List<DropdwonSelector>> GetCourierDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
