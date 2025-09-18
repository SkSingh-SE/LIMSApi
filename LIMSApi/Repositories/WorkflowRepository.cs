using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class WorkflowRepository : IWorkflowRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;
        public WorkflowRepository(LIMSContext _context)
        {
            this._context = _context;
            this.loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<Workflow?> GetWorkflowByIdAsync(long id) =>
        await _context.Workflows
            .Include(w => w.Steps)
            .ThenInclude(s => s.Transitions)
            .FirstOrDefaultAsync(w => w.ID == id);

        public async Task<List<Workflow>> GetAllWorkflowsAsync() =>
            await _context.Workflows
                .Include(w => w.Steps)
                .ThenInclude(s => s.Transitions)
                .ToListAsync();

        public async Task<Workflow> AddWorkflowAsync(Workflow workflow)
        {
            workflow.CompanyCode = loggedInUser.CompanyCode;
            workflow.IsActive = true;
            workflow.CreatedBy = loggedInUser.EmployeeID;
            workflow.CreatedOn = DateTime.UtcNow;
            await _context.Workflows.AddAsync(workflow);
            await _context.SaveChangesAsync();
            return workflow;
        }

        public  async Task<Workflow> UpdateWorkflowAsync(Workflow workflow)
        {
            workflow.ModifiedBy = loggedInUser.EmployeeID;
            workflow.ModifiedOn = DateTime.UtcNow;
            _context.Workflows.Update(workflow);
            await _context.SaveChangesAsync();
            return workflow;
        }
        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.Workflows.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }


        public async Task<WorkflowInstance?> GetWorkflowInstanceAsync(long id) =>
            await _context.WorkflowInstances
                .Include(i => i.WorkflowID)
                .FirstOrDefaultAsync(i => i.ID == id);

        public async Task AddWorkflowInstanceAsync(WorkflowInstance instance)
        {
            await _context.WorkflowInstances.AddAsync(instance);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateWorkflowInstanceAsync(WorkflowInstance instance)
        {
            _context.WorkflowInstances.Update(instance);

            await _context.SaveChangesAsync();
        }

        public async Task AddWorkflowActionLogAsync(WorkflowActionLog log)
        {
            await _context.WorkflowActionLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
        public async Task<List<WorkflowActionLog>> GetWorkflowActionLogsAsync(long workflowId)
        {
            return await _context.WorkflowActionLogs
                .Where(log => log.WorkflowID == workflowId)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }
        public async Task<WorkflowInstance?> GetActiveInstanceForEntityAsync(long entityId, string entityType)
        {
            return await _context.WorkflowInstances
                .Include(i => i.Workflow)
                .FirstOrDefaultAsync(i =>
                    i.EntityID == entityId &&
                    i.Workflow.EntityType == entityType &&
                    i.Status != "Completed" &&
                    i.Status != "Cancelled");
        }

        public async Task<List<WorkflowActionLog>> GetActionLogsForInstanceAsync(long instanceId)
        {
            return await _context.WorkflowActionLogs
                .Where(l => l.InstanceID == instanceId)
                .OrderBy(l => l.Timestamp)
                .ToListAsync();
        }
    }
}
