using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IHeatTreatmentService
    {
        Task CreateHeatTreatment(HeatTreatmentMaster model);
        Task ModifyHeatTreatment(HeatTreatmentMaster model);
        Task RemoveHeatTreatment(long id);
        Task<HeatTreatmentMaster> GetHeatTreatmentDetails(long id);
        Task<PagedResponse<object>> FetchHeatTreatmentList(PageFilter filter);

        Task<List<DropdwonSelector>> GetHeatTreatmentDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
