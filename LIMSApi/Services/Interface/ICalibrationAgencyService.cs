using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ICalibrationAgencyService
    {
        Task CreateCalibrationAgency(CalibrationAgencyMaster model);
        Task ModifyCalibrationAgency(CalibrationAgencyMaster model);
        Task RemoveCalibrationAgency(long id);
        Task<CalibrationAgencyMaster> GetCalibrationAgencyDetails(long id);
        Task<PagedResponse<object>> FetchCalibrationAgencyList(PageFilter filter);

        Task<List<DropdwonSelector>> GetCalibrationAgencyDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
