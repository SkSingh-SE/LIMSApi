using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IEquipmentTypeService
    {
        Task CreateEquipmentType(EquipmentTypeMaster model);
        Task ModifyEquipmentType(EquipmentTypeMaster model);
        Task RemoveEquipmentType(long id);
        Task<EquipmentTypeMaster> GetEquipmentTypeDetails(long id);
        Task<PagedResponse<object>> FetchEquipmentTypeList(PageFilter filter);

        Task<List<DropdwonSelector>> GetEquipmentTypeDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
