using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISubContractorRepository
    {
        Task AddSubContractor(SubContractorMaster model);
        Task UpdateSubContractor(SubContractorMaster model);
        Task DeleteSubContractor(SubContractorMaster model);
        Task<SubContractorMaster> GetSubContractorById(long id);
        Task<PagedResponse<object>> GetAllSubContractors(PageFilter filter);

        Task<List<DropdwonSelector>> GetSubContractorDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
