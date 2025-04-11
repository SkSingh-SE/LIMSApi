using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ICalibrationAgencyRepository
    {
        Task AddCalibrationAgency(CalibrationAgencyMaster model);
        Task UpdateCalibrationAgency(CalibrationAgencyMaster model);
        Task DeleteCalibrationAgency(long id);
        Task<CalibrationAgencyMaster> GetCalibrationAgencyById(long id);
        Task<PagedResponse<object>> GetAllCalibrationAgencys(PageFilter filter);

        Task<List<DropdwonSelector>> GetCalibrationAgencyDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
