using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ITestLayoutTemplateRepository
    {
        Task AddLayoutTemplate(TestLayoutTemplate model);
        Task UpdateLayoutTemplate(TestLayoutTemplate model);
        Task DeleteLayoutTemplate(TestLayoutTemplate model);
        Task<TestLayoutTemplate?> GetLayoutTemplateById(long id);
        Task<TestLayoutTemplate?> GetLayoutByTestHierarchy(long? testId, long? subGroupId, long? analysisTypeId, string? leafNode, long? testMethodId = null, string? testMethodName = null);
        Task<PagedResponse<object>> GetAllLayoutTemplates(PageFilter filter);
        Task<List<DropdwonSelector>> GetLayoutDropdown(long? testId, string? searchTerm, int pageNo, int pageSize);
        Task ResetDefaultLayouts(long? testId, long? analysisTypeId, long currentId);
        Task SyncMappedTests(long templateId, List<long> testIds);
        Task SyncMappedLeafNodes(long templateId, List<TestLayoutTemplateMappingItemDto> leafNodes);
        Task<List<long>> GetMappedTestIds(long templateId);
        Task<List<TestLayoutTemplateMappingItemDto>> GetMappedLeafNodes(long templateId);
    }
}
