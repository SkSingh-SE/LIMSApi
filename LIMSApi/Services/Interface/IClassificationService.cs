using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IClassificationService
    {
        Task CreateClassification(ClassificationMasterCreateDto dto);
        Task ModifyClassification(ClassificationMasterUpdateDto dto);
        Task RemoveClassification(long id);
        Task<ClassificationMasterDto> GetClassificationDetails(long id);
        Task<PagedResponse<object>> FetchClassificationList(PageFilter filter);
        Task<bool> ToggleClassificationStatus(long id);
        Task<List<DropdwonSelector>> GetClassificationDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
