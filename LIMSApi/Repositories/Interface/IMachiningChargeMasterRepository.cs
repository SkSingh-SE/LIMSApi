using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IMachiningChargeMasterRepository
    {
        Task AddMachiningChargeMaster(MachiningChargeMaster model);
        Task SaveChangesAsync();
        Task<long?> GetFinancialYearIdForDate(DateTime date);
        Task<MachiningChargeMaster?> GetMachiningChargeMasterById(long id);
        Task<PagedResponse<object>> GetAllMachiningChargeMasters(PageFilter filter);
        Task<List<DropdwonSelector>> GetMachiningChargeMasterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsBySpecimenSizeAndTest(string specimenSize, long laboratoryTestID, long testMethodStandardID);
        Task<bool> ExistsBySpecimenSizeAndTestAndNotId(string specimenSize, long laboratoryTestID, long testMethodStandardID, long id);
        Task<List<MachiningChargeMaster>> GetByLabTestAndStandard(long labTestId, long standardId);
    }
}
