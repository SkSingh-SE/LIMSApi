using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IClassificationRepository
    {
        Task AddClassification(ClassificationMaster model);
        Task UpdateClassification(ClassificationMaster model);
        Task<ClassificationMaster?> GetClassificationById(long id);
        Task<PagedResponse<object>> GetAllClassifications(PageFilter filter);
        Task<List<DropdwonSelector>> GetClassificationDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByCode(string code);
        Task<bool> ExistsByCodeAndNotId(string code, long id);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
    }
}
