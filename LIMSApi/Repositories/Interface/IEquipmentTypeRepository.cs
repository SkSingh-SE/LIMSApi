using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IEquipmentTypeRepository
    {
        Task AddEquipmentType(EquipmentTypeMaster model);
        Task UpdateEquipmentType(EquipmentTypeMaster model);
        Task DeleteEquipmentType(long id);
        Task<EquipmentTypeMaster> GetEquipmentTypeById(long id);
        Task<PagedResponse<object>> GetAllEquipmentTypes(PageFilter filter);

        Task<List<DropdwonSelector>> GetEquipmentTypeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
