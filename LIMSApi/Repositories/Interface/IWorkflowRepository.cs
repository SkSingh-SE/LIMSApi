using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IWorkflowRepository
    {
        Task<Workflow?> GetWorkflowByIdAsync(long id);
        Task<List<Workflow>> GetAllWorkflowsAsync();
        Task<Workflow> AddWorkflowAsync(Workflow workflow);
        Task<Workflow> UpdateWorkflowAsync(Workflow workflow);
        Task<bool> ExistsByNameAndNotId(string name, long Id);

        Task<WorkflowInstance?> GetWorkflowInstanceAsync(long id);
        Task AddWorkflowInstanceAsync(WorkflowInstance instance);
        Task UpdateWorkflowInstanceAsync(WorkflowInstance instance);

        Task AddWorkflowActionLogAsync(WorkflowActionLog log);
        Task<List<WorkflowActionLog>> GetWorkflowActionLogsAsync(long workflowId);
        Task<WorkflowInstance?> GetActiveInstanceForEntityAsync(long entityId, string entityType);
        Task<List<WorkflowActionLog>> GetActionLogsForInstanceAsync(long instanceId);
    }
}
