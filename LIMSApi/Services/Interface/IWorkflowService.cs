using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IWorkflowService
    {
        Task<Workflow?> GetWorkflow(long id);
        Task<PagedResponse<object>> GetAllWorkflows(PageFilter filter);
        Task CreateWorkflow(WorkflowDto workflow);
        Task UpdateWorkflow(WorkflowDto workflow);
        Task StartWorkflow(long entityId, string entityType);
        Task PerformAction(long instanceId, string action, long userId, string comments);
        Task PerformWorkflowActionAsync(WorkflowActionRequestDto dto);
        Task<List<WorkflowActionLog>> GetWorkflowActionHistory(long workflowId);

        Task<WorkflowStep> GetCurrentWorkflowStepAsync(long entityId, string entityType);
        Task<WorkflowInstance?> GetActiveInstanceForEntityAsync(long entityId, string entityType);
        Task<bool> WorkflowExistsForEntityType(string entityType);

        // Phase 9: Batch status checks
        Task<Dictionary<long, bool>> CanUpdateEntitiesBatch(IEnumerable<long> entityIds, string entityType);
        Task<Dictionary<long, WorkflowInstance?>> GetActiveInstancesForEntitiesBatch(IEnumerable<long> entityIds, string entityType);
    }
}
