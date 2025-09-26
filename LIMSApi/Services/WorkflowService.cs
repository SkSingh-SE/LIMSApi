using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Protocol.Core.Types;

namespace LIMSApi.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRepository _repository;
        private readonly ILogger<WorkflowService> _logger;
        private readonly LoggedInUserDTO _loggedInUser;
        private readonly INotificationService _notificationService;
        private readonly IEmployeeService _employeeService;
        public WorkflowService(IWorkflowRepository WorkflowRepository, ILogger<WorkflowService> logger, INotificationService notification, IEmployeeService employeeService)
        {
            _logger = logger;
            _repository = WorkflowRepository;
            _notificationService = notification;
            _employeeService = employeeService;
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<Workflow?> GetWorkflow(long id) =>
             await _repository.GetWorkflowByIdAsync(id);

        public async Task<PagedResponse<object>> GetAllWorkflows(PageFilter filter) =>
            await _repository.GetAllWorkflowsAsync(filter);

        public async Task CreateWorkflow(WorkflowDto dto)
        {
            var workflow = new Workflow
            {
                Name = dto.Name,
                EntityType = dto.EntityType,
                IsActive = true,
                Steps = dto.Steps.Select(s => new WorkflowStep
                {
                    OrderNo = s.OrderNo,
                    Name = s.Name,
                    AssignedToType = s.AssignedToType,
                    AssignedToValue = s.AssignedToValue
                }).ToList()
            };

            var savedWorkflow = await _repository.AddWorkflowAsync(workflow);

            // Build mapping OrderNo → Id
            var stepMap = savedWorkflow.Steps.Where(s => s.IsActive).ToDictionary(s => s.Name, s => s.ID);
            foreach (var s in dto.Steps)
            {
                var dbStep = savedWorkflow.Steps.First(x => x.OrderNo == s.OrderNo);
                foreach (var t in s.Transitions)
                {
                    dbStep.Transitions.Add(new WorkflowTransition
                    {
                        Action = t.Action,
                        Alias = t.Alias,
                        ToStepName = t.ToStepName,
                        ToStepID = !string.IsNullOrWhiteSpace(t.ToStepName) && stepMap.TryGetValue(t.ToStepName, out var mappedId) ? mappedId : null,

                    });

                }
            }

            await _repository.UpdateWorkflowAsync(workflow);
            _logger.LogInformation("Workflow definition created: {@Workflow}", workflow);

        }
        public async Task UpdateWorkflow(WorkflowDto dto)
        {
            // 1. Check uniqueness
            var exists = await _repository.ExistsByNameAndNotId(dto.EntityType, dto.ID);
            if (exists)
                throw new InvalidOperationException($"Workflow for EntityType '{dto.EntityType}' already exists.");

            // 2. Get workflow
            var workflow = await _repository.GetWorkflowByIdAsync(dto.ID);
            if (workflow == null)
                throw new KeyNotFoundException($"Workflow {dto.ID} not found.");

            workflow.IsActive = true;
            workflow.ModifiedBy = _loggedInUser.EmployeeID;
            workflow.ModifiedOn = DateTime.UtcNow;

            // 3. Update Steps
            foreach (var dbStep in workflow.Steps)
            {
                var dtoStep = dto.Steps.FirstOrDefault(s => s.ID == dbStep.ID);

                if (dtoStep == null)
                {
                    // Soft delete missing step
                    dbStep.IsActive = false;
                    foreach (var trans in dbStep.Transitions)
                        trans.IsActive = false;
                }
                else
                {
                    // Update existing step
                    dbStep.Name = dtoStep.Name;
                    dbStep.AssignedToType = dtoStep.AssignedToType;
                    dbStep.AssignedToValue = dtoStep.AssignedToValue;
                    dbStep.IsActive = true;

                    // Handle transitions
                    foreach (var dbTrans in dbStep.Transitions)
                    {
                        var dtoTrans = dtoStep.Transitions.FirstOrDefault(t => t.ID == dbTrans.ID);
                        if (dtoTrans == null)
                        {
                            dbTrans.IsActive = false;
                        }
                        else
                        {
                            dbTrans.Alias = dtoTrans.Alias;
                            dbTrans.IsActive = true;
                            dbTrans.ToStepID = null; // remap later
                            dbTrans.ToStepName = dtoTrans.ToStepName;
                        }
                    }

                    // Add new transitions
                    var newTransitions = dtoStep.Transitions
                        .Where(t => dbStep.Transitions.All(x => x.Action != t.Action))
                        .Select(t => new WorkflowTransition
                        {
                            Action = t.Action,
                            Alias = t.Alias,
                            ToStepID = null,
                            ToStepName = t.ToStepName,
                            IsActive = true
                        }).ToList();

                    foreach (var nt in newTransitions)
                        dbStep.Transitions.Add(nt);
                }
            }

            // 4. Add brand new steps
            var newSteps = dto.Steps
                .Where(s => workflow.Steps.All(x => x.ID != s.ID))
                .Select(s => new WorkflowStep
                {
                    OrderNo = s.OrderNo,
                    Name = s.Name,
                    AssignedToType = s.AssignedToType,
                    AssignedToValue = s.AssignedToValue,
                    IsActive = true,
                    Transitions = s.Transitions.Select(t => new WorkflowTransition
                    {
                        Action = t.Action,
                        Alias = t.Alias,
                        ToStepID = null,
                        ToStepName = t.ToStepName,
                        IsActive = true
                    }).ToList()
                }).ToList();

            foreach (var ns in newSteps)
                workflow.Steps.Add(ns);

            // 5. Save once to get IDs
            workflow = await _repository.UpdateWorkflowAsync(workflow);

            // 6. Re-map ToStepID (based on ToStepName instead of ToStepName)
            var stepMap = workflow.Steps.Where(s => s.IsActive).ToDictionary(s => s.Name, s => s.ID);

            foreach (var dbStep in workflow.Steps.Where(s => s.IsActive))
            {
                foreach (var dbTrans in dbStep.Transitions.Where(t => t.IsActive))
                {
                    if (!string.IsNullOrWhiteSpace(dbTrans.ToStepName)
                        && stepMap.TryGetValue(dbTrans.ToStepName, out var mappedId))
                    {
                        dbTrans.ToStepID = mappedId;
                    }
                }
            }

            // 7. Save again after ToStepID remap
            await _repository.UpdateWorkflowAsync(workflow);

            _logger.LogInformation("Workflow {WorkflowId} updated with smart sync.", dto.ID);
        }

        // ----------- Transactions ---------------------
        public async Task StartWorkflow(long entityId, string entityType)
        {
            var instance = await _repository.GetActiveInstanceForEntityAsync(entityId, entityType);

            if (instance != null)
            {
                // Reset existing workflow
                var logs = await _repository.GetActionLogsForInstanceAsync(instance.ID);

                // Only reset if the workflow already has actions (optional, can reset always)
                if (logs.Count > 0)
                {
                    instance.Status = "Cancelled";
                    await _repository.UpdateWorkflowInstanceAsync(instance);

                    await _repository.AddWorkflowActionLogAsync(new WorkflowActionLog
                    {
                        WorkflowID = instance.WorkflowID,
                        InstanceID = instance.ID,
                        StepID = instance.CurrentStepID,
                        Action = "Cancel",
                        EmployeeID = _loggedInUser.EmployeeID,
                        Comments = "Workflow reset due to repeated StartWorkflow call",
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            var workflow = await _repository.GetWorkflowByEntityNameAsync(entityType)
                ?? throw new Exception("Workflow not found");

            if (!workflow.EntityType.Equals(entityType, StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Workflow type mismatch. Expected {workflow.EntityType}, got {entityType}");

            var firstStep = workflow.Steps.OrderBy(s => s.OrderNo).FirstOrDefault()
                ?? throw new Exception("Workflow has no steps");

             instance = new WorkflowInstance
            {
                WorkflowID = workflow.ID,
                EntityID = entityId,
                EntityType = entityType,
                CurrentStepID = firstStep.ID,
                Status = "InProgress"
            };

            await _repository.AddWorkflowInstanceAsync(instance);

            await _repository.AddWorkflowActionLogAsync(new WorkflowActionLog
            {
                WorkflowID = workflow.ID,
                InstanceID = instance.ID,
                StepID = firstStep.ID,
                Action = "Start",
                EmployeeID = _loggedInUser.EmployeeID,
                Comments = "Workflow started automatically on entity creation",
                Timestamp = DateTime.UtcNow
            });

            //  Send notification to assigned users of the first step
            if (!string.IsNullOrWhiteSpace(firstStep.AssignedToValue))
            {
                var userIds = firstStep.AssignedToValue.Split(',')
                    .Select(x => long.Parse(x.Trim()))
                    .ToList();

                foreach (var userId in userIds)
                {
                    var user = await _employeeService.GetEmployeeDetails(userId);
                    var notification = new Notification
                    {
                        UserID = userId,
                        Email = user?.EmailId,
                        Title = $"Workflow Started: {workflow.Name}",
                        Message = $"Entity {entityType} (ID: {entityId}) has entered step: {firstStep.Name}",
                        Type = NotificationType.Workflow,
                        EntityID = entityId,
                        EntityType = entityType,
                        WorkflowID = workflow.ID,
                        StepID = firstStep.ID,
                        Action = "Start",
                        CreatedOn = DateTime.UtcNow,
                        IsRead = false
                    };

                    // store + real-time + email if needed
                    await _notificationService.CreateNotificationAsync(notification);
                }
            }
            _logger.LogInformation(
                "Workflow started. WorkflowID: {WorkflowId}, EntityID: {EntityId}, EntityType: {EntityType}, InstanceID: {InstanceId}, Step: {StepName}",
                workflow.ID, entityId, entityType, instance.ID, firstStep.Name);
        }


        public async Task PerformAction(long instanceId, string action, long employeeId, string comments)
        {
            var instance = await _repository.GetWorkflowInstanceAsync(instanceId)
                ?? throw new Exception("Instance not found");

            var workflow = await _repository.GetWorkflowByIdAsync(instance.WorkflowID)
                ?? throw new Exception("Workflow not found");

            var currentStep = workflow.Steps.First(s => s.ID == instance.CurrentStepID);
            var transition = currentStep.Transitions.FirstOrDefault(t =>
                t.Action.Equals(action, StringComparison.OrdinalIgnoreCase));

            if (transition == null)
                throw new Exception($"Invalid action '{action}' for step '{currentStep.Name}'");

            var log = new WorkflowActionLog
            {
                WorkflowID = workflow.ID,
                InstanceID = instanceId,
                StepID = currentStep.ID,
                Action = action,
                EmployeeID = employeeId,
                Comments = comments,
                Timestamp = DateTime.UtcNow
            };

            await _repository.AddWorkflowActionLogAsync(log);

            // Move to next step
            instance.CurrentStepID = transition.ToStepID ?? 0;
            instance.Status = action.Equals("Reject", StringComparison.OrdinalIgnoreCase) ? "Rejected" :
                              action.Equals("Approve", StringComparison.OrdinalIgnoreCase) ? "Approved" : "Pending";

            await _repository.UpdateWorkflowInstanceAsync(instance);

            _logger.LogInformation(
                "Workflow action performed. WorkflowID: {WorkflowId}, InstanceID: {InstanceId}, EntityType: {EntityType}, Step: {StepName}, Action: {Action}, User: {UserId}, Comments: {Comments}",
                workflow.ID, instanceId, instance.EntityType, currentStep.Name, action, employeeId, comments);
        }

        public async Task<List<WorkflowActionLog>> GetWorkflowActionHistory(long workflowId)
        {
            var logs = await _repository.GetWorkflowActionLogsAsync(workflowId);
            return logs;
        }

        public async Task<bool> CanUpdateEntity(long entityId, string entityType)
        {
            var instance = await _repository.GetActiveInstanceForEntityAsync(entityId, entityType);
            if (instance == null) return true; // No existingWorkflow → free update

            var logs = await _repository.GetActionLogsForInstanceAsync(instance.ID);

            // Only "Start" log exists → still safe to update
            return logs.Count <= 1;
        }

        public async Task RestartWorkflowIfNeeded(long entityId, string entityType, long userId, string reason)
        {
            var instance = await _repository.GetActiveInstanceForEntityAsync(entityId, entityType);
            if (instance == null) return; // no existingWorkflow

            var logs = await _repository.GetActionLogsForInstanceAsync(instance.ID);

            if (logs.Count > 1) // existingWorkflow already acted upon
            {
                // Cancel current existingWorkflow
                instance.Status = "Cancelled";
                await _repository.UpdateWorkflowInstanceAsync(instance);

                await _repository.AddWorkflowActionLogAsync(new WorkflowActionLog
                {
                    WorkflowID = instance.WorkflowID,
                    InstanceID = instance.ID,
                    StepID = instance.CurrentStepID,
                    Action = "Cancel",
                    EmployeeID = userId,
                    Comments = $"Workflow cancelled due to entity update. Reason: {reason}",
                    Timestamp = DateTime.UtcNow
                });

                // Restart existingWorkflow (preserve entityType!)
                await StartWorkflow(entityId, entityType);
            }
        }
    }
}
