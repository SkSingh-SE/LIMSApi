using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IEquipmentRepository
    {
        Task AddEquipment(EquipmentMaster model);
        Task UpdateEquipment(EquipmentMaster model);
        Task DeleteEquipment(long id);
        Task<EquipmentMaster> GetEquipmentById(long id);
        Task<PagedResponse<object>> GetAllEquipments(PageFilter filter);

        Task<List<DropdwonSelector>> GetEquipmentDropdown(string? searchTerm, int pageNo, int pageSize, long? labTestId = null, long? subGroupId = null, long? analysisTypeId = null);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
