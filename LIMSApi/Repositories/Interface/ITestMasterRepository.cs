using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ITestMasterRepository
    {
        Task AddTestMaster(TestMaster model);
        Task UpdateTestMaster(TestMaster model);
        Task DeleteTestMaster(long id);
        Task<TestMaster> GetTestMasterById(long id);
        Task<PagedResponse<object>> GetAllTestMasters(PageFilter filter);

        Task<List<DropdwonSelector>> GetTestMasterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetGeneralTestMasterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetChemicalTestMasterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
