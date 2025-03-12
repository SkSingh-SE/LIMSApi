using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IEquipmentService
    {
        Task CreateEquipment(EquipmentMaster model);
        Task ModifyEquipment(EquipmentMaster model);
        Task RemoveEquipment(long id);
        Task<EquipmentMaster> GetEquipmentDetails(long id);
        Task<PagedResponse<object>> FetchEquipmentList(PageFilter filter);

        Task<List<DropdwonSelector>> GetEquipmentDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
