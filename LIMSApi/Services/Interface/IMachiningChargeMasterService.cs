using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IMachiningChargeMasterService
    {
        Task CreateMachiningChargeMaster(MachiningChargeMaster model);
        Task ModifyMachiningChargeMaster(MachiningChargeMaster model);
        Task RemoveMachiningChargeMaster(long id);
        Task<MachiningChargeMaster> GetMachiningChargeMasterDetails(long id);
        Task<PagedResponse<object>> FetchMachiningChargeMasterList(PageFilter filter);
        Task<List<DropdwonSelector>> GetMachiningChargeMasterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<MachiningChargeMaster>> GetByLabTestAndStandard(long labTestId, long standardId);
    }
}
