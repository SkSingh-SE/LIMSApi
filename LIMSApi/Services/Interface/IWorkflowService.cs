using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IWorkflowService
    {
        Task<Workflow?> GetWorkflow(long id);
        Task<List<Workflow>> GetAllWorkflows();
        Task CreateWorkflow(WorkflowDto workflow);
        Task UpdateWorkflow(WorkflowDto workflow);
        Task StartWorkflow(long workflowId, long entityId, string entityType);
        Task PerformAction(long instanceId, string action, long userId, string comments);
        Task<List<WorkflowActionLog>> GetWorkflowActionHistory(long workflowId);

    }
}
