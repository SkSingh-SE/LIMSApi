using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ICourierService
    {
        Task CreateCourier(CourierMaster model);
        Task ModifyCourier(CourierMaster model);
        Task RemoveCourier(long id);
        Task<CourierMaster> GetCourierDetails(long id);
        Task<PagedResponse<object>> FetchCourierList(PageFilter filter);

        Task<List<DropdwonSelector>> GetCourierDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
