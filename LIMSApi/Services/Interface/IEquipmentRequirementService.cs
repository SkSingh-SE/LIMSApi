using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IEquipmentRequirementService
    {
        Task CreateRequirement(EquipmentRequirementCreateDto dto);
        Task ModifyRequirement(EquipmentRequirementUpdateDto dto);
        Task RemoveRequirement(long id);
        Task<EquipmentRequirementDetailDto> GetRequirementDetails(long id);
        Task<PagedResponse<object>> FetchRequirementList(EquipmentRequirementListRequest request);
        Task<bool> ToggleRequirementStatus(long id);
        Task<List<DropdwonSelector>> GetRequirementDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<EligibleEquipmentDto>> GetEligibleEquipment(long requirementId, long branchId);
    }
}
