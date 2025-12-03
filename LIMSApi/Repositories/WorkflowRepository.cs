using System.Linq.Dynamic.Core;
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
            .FirstOrDefaultAsync(w => w.ID == id && w.IsActive);
        public async Task<Workflow?> GetWorkflowByEntityNameAsync(string entityName) =>
        await _context.Workflows
            .Include(w => w.Steps)
            .ThenInclude(s => s.Transitions)
            .FirstOrDefaultAsync(w => w.EntityType == entityName && w.IsActive);

        public async Task<PagedResponse<object>> GetAllWorkflowsAsync(PageFilter filter)
        {
            var _query = from c in _context.Workflows
                         where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                         join e in _context.EmployeeMasters on c.CreatedBy equals e.ID into emp
                         from e in emp.DefaultIfEmpty()
                         select new
                         {
                             c.ID,
                             c.Name,
                             c.EntityType,
                             c.CreatedOn,
                             CreatedBy = e.Name,
                         };

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search))
                || x.EntityType.ToLower().Contains(search)
                || x.CreatedBy.ToLower().Contains(search)
                || x.CreatedOn.ToString().Contains(search));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            // Total Records Count
            int totalRecords = await _query.CountAsync();

            // Apply Pagination
            var items = await _query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(items.Cast<object>().ToList(), totalRecords, filter.PageNumber, filter.PageSize);
        }

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

        public async Task<Workflow> UpdateWorkflowAsync(Workflow workflow)
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
                .Include(i => i.Workflow)
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
            return await _context.WorkflowInstances.Where(w => w.EntityID == entityId && w.EntityType == entityType && w.IsActive).FirstOrDefaultAsync();
        }

        public async Task<List<WorkflowActionLog>> GetActionLogsForInstanceAsync(long instanceId)
        {
            return await _context.WorkflowActionLogs
                .Where(l => l.InstanceID == instanceId)
                .OrderBy(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task<WorkflowStep> GetCurrentWorkflowStepAsync(long entityId, string entityType)
        {
            var instance = await GetActiveInstanceForEntityAsync(entityId, entityType);
            if (instance == null) return null;

            var step = await _context.WorkflowSteps.Include(x => x.Transitions)
                .FirstOrDefaultAsync(s => s.WorkflowID == instance.WorkflowID && s.ID == instance.CurrentStepID);
            return step;
        }
    }
}
