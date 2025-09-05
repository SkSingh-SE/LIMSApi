using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISampleInwardRepository
    {
        Task AddSampleInward(SampleInward model);
        Task UpdateSampleInward(SampleInward model);
        Task DeleteSampleInward(long id);
        Task<SampleInward> GetSampleInwardById(long id);
        Task<SampleInward> GetSampleInwardWithPlans(long id);
        Task<PagedResponse<object>> GetAllSampleInwards(PageFilter filter);
        Task<List<DropdwonSelector>> GetSampleInwardDropdown(string? searchTerm, int pageNo, int pageSize);

        Task<object> GetCaseNoAndSampleNo();
    }
}
