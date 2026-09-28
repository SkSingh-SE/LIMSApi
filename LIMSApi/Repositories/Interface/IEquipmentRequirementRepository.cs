using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IEquipmentRequirementRepository
    {
        Task AddRequirement(EquipmentRequirementMaster model);
        Task UpdateRequirement(EquipmentRequirementMaster model);
        Task<EquipmentRequirementMaster?> GetRequirementById(long id);
        Task<PagedResponse<object>> GetPagedRequirements(EquipmentRequirementListRequest request);
        Task<List<DropdwonSelector>> GetRequirementDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByCode(string code);
        Task<bool> ExistsByCodeAndNotId(string code, long id);
        Task<bool> ExistsCombination(long laboratoryTestId, long? methodId, long? parameterId, long equipmentTypeId);
        Task<bool> ExistsCombinationAndNotId(long id, long laboratoryTestId, long? methodId, long? parameterId, long equipmentTypeId);
        Task<List<EquipmentRequirementMaster>> GetActiveForTest(long laboratoryTestId, long? methodId);
    }
}
