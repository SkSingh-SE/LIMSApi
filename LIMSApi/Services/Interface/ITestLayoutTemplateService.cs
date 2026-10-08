using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ITestLayoutTemplateService
    {
        Task CreateLayoutTemplate(TestLayoutTemplate model);
        Task ModifyLayoutTemplate(TestLayoutTemplate model);
        Task RemoveLayoutTemplate(long id);
        Task<TestLayoutTemplate?> GetLayoutTemplateDetails(long id);
        Task<TestLayoutTemplate?> GetLayoutByHierarchy(long? testId, long? subGroupId, long? analysisTypeId, string? leafNode);
        Task<PagedResponse<object>> FetchLayoutTemplateList(PageFilter filter);
        Task<List<DropdwonSelector>> GetLayoutTemplateDropdown(long? testId, string? searchTerm, int pageNo, int pageSize);
    }
}
