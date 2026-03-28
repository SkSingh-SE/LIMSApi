using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IEquipmentReferenceMaterialService
    {
        Task<List<EquipmentReferenceMaterial>> GetByEquipment(long equipmentMasterID);
        Task Create(EquipmentReferenceMaterial model);
        Task Delete(long id);
    }
}
