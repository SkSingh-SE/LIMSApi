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
        Task<PagedResponse<object>> GetAllSampleInwards(PageFilter filter);
        Task<List<DropdwonSelector>> GetSampleInwardDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
