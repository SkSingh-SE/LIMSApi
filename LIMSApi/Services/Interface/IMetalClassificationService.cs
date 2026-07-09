using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IMetalClassificationService
    {
        Task CreateMetalClassification(MetalClassificationMaster model);
        Task ModifyMetalClassification(MetalClassificationMaster model);
        Task RemoveMetalClassification(long id);
        Task<MetalClassificationMaster> GetMetalClassificationDetails(long id);
        Task<PagedResponse<object>> FetchMetalClassificationList(PageFilter filter);

        Task<List<DropdwonSelector>> GetMetalClassificationDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<ParameterMaster>> GetParameterByMetalId(long id);
        Task<List<DropdwonSelector>> GetTechniquesForMetal(long metalId);
    }
}
