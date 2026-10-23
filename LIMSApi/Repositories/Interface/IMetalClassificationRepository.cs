using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IMetalClassificationRepository
    {
        Task AddMetalClassification(MetalClassificationMaster model);
        Task UpdateMetalClassification(MetalClassificationMaster model);
        Task DeleteMetalClassification(long id);
        Task<MetalClassificationMaster> GetMetalClassificationById(long id);
        Task<PagedResponse<object>> GetAllMetalClassifications(PageFilter filter);

        Task<List<DropdwonSelector>> GetMetalClassificationDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<ParameterMaster>> GetParameterByMetalId(long id);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
        Task<bool> ExistsByCode(string code);
        Task<bool> ExistsByCodeAndNotId(string code, long Id);
    }
}
