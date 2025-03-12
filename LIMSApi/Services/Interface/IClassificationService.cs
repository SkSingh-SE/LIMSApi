using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IClassificationService
    {
        Task CreateClassification(ClassificationMaster model);
        Task ModifyClassification(ClassificationMaster model);
        Task RemoveClassification(long id);
        Task<ClassificationMaster> GetClassificationDetails(long id);
        Task<PagedResponse<object>> FetchClassificationList(PageFilter filter);

        Task<List<DropdwonSelector>> GetClassificationDropdown(string? searchTerm, int pageNo, int pageSize);

    }
}
