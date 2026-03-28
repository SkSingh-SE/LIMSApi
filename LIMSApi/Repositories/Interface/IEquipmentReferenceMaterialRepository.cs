using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IEquipmentReferenceMaterialRepository
    {
        Task<List<EquipmentReferenceMaterial>> GetByEquipmentId(long equipmentMasterID);
        Task<EquipmentReferenceMaterial?> GetById(long id);
        Task Add(EquipmentReferenceMaterial model);
        Task Update(EquipmentReferenceMaterial model);
        Task Delete(long id);
    }
}
