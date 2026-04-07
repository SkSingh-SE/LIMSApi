using System.Text.Json;
using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface INablService
    {
        Task<PagedResponse<object>> FetchList(string formType, PageFilter filter);
        Task<object?> GetDetails(string formType, long id);
        Task<object?> GetByDesignationId(string formType, long designationId);
        Task<long> Save(string formType, JsonElement body);
        Task Remove(string formType, long id);

        // Workflow
        Task Submit(string formType, long id);
        Task Review(string formType, long id);
        Task Approve(string formType, long id);
        Task Reject(string formType, long id, string? remarks);

        // History & Audit
        Task<List<object>> GetRevisionHistory(string formType, long id);
        Task<List<object>> GetAuditLog(string formType, long id);

        // Form Defaults & Reviewers
        Task<object> GetFormDefaults(string formType);
        Task<object> GetSuggestedReviewers();
    }
}
