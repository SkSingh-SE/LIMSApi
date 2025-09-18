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
        public WorkflowService(IWorkflowRepository WorkflowRepository, ILogger<WorkflowService> logger)
        {
            _logger = logger;
            _repository = WorkflowRepository;  
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<Workflow?> GetWorkflow(long id) =>
             await _repository.GetWorkflowByIdAsync(id);

        public async Task<List<Workflow>> GetAllWorkflows() =>
            await _repository.GetAllWorkflowsAsync();

        public async Task CreateWorkflow(WorkflowDto dto)
        {
            var workflow = new Workflow
            {
                Name = dto.Name,
                EntityType = dto.EntityType,
                IsActive = dto.IsActive,
                Steps = dto.Steps.Select(s => new WorkflowStep
                {
                    OrderNo = s.OrderNo,
                    Name = s.Name,
                    AssignedToType = s.AssignedToType,
                    AssignedToValue = s.AssignedToValue
                }).ToList()
            };

            var savedWorkflow =  await _repository.AddWorkflowAsync(workflow);

            // Build mapping OrderNo → Id
            var stepMap = workflow.Steps.ToDictionary(s => s.OrderNo, s => s.ID);

            foreach (var s in dto.Steps)
            {
                var dbStep = savedWorkflow.Steps.First(x => x.OrderNo == s.OrderNo);
                foreach (var t in s.Transitions)
                {
                    dbStep.Transitions.Add(new WorkflowTransition
                    {
                        Action = t.Action,
                        Alias = t.Alias,
                        ToStepID = t.ToStepID.HasValue && stepMap.ContainsKey((int)t.ToStepID)
                            ? stepMap[(int)t.ToStepID.Value]
                            : null
                    });
                }
            }

            await _repository.UpdateWorkflowAsync(workflow);
            _logger.LogInformation("Workflow definition created: {@Workflow}", workflow);
            
        }
        public async Task UpdateWorkflow(WorkflowDto dto)
        {
            //  Check uniqueness
            var existing = await _repository.ExistsByNameAndNotId(dto.EntityType, dto.ID);
            if (existing)
                throw new InvalidOperationException(
                    $"Workflow for EntityType '{dto.EntityType}' already exists."
                );
            // Get workflow
            var existingWorkflow = await _repository.GetWorkflowByIdAsync(dto.ID);
            if (existingWorkflow == null)
                throw new KeyNotFoundException($"Workflow {dto.ID} not found.");

            // Update metadata
            existingWorkflow.IsActive = dto.IsActive;
            existingWorkflow.ModifiedBy = _loggedInUser.EmployeeID;
            existingWorkflow.ModifiedOn = DateTime.UtcNow;

            // Soft delete all old steps & transitions
            foreach (var step in existingWorkflow.Steps)
            {
                step.IsActive = false;
                foreach (var trans in step.Transitions)
                {
                    trans.IsActive = false;
                }
            }

            //  Insert new steps
            existingWorkflow.Steps = dto.Steps.Select(s => new WorkflowStep
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
                    IsActive = true
                }).ToList()
            }).ToList();

            //  Save and remap step IDs
            existingWorkflow = await _repository.UpdateWorkflowAsync(existingWorkflow);

            var stepMap = existingWorkflow.Steps.ToDictionary(s => s.OrderNo, s => s.ID);
            foreach (var s in dto.Steps)
            {
                var dbStep = existingWorkflow.Steps.First(x => x.OrderNo == s.OrderNo);
                foreach (var t in s.Transitions)
                {
                    var dbTransition = dbStep.Transitions.First(x => x.Action == t.Action);
                    dbTransition.ToStepID = t.ToStepID;
                }
            }

            await _repository.UpdateWorkflowAsync(existingWorkflow);

            _logger.LogInformation("Workflow {WorkflowId} updated with soft delete enabled.", dto.ID);
        }


        public async Task StartWorkflow(long workflowId, long entityId, string entityType)
        {
            var workflow = await _repository.GetWorkflowByIdAsync(workflowId)
                ?? throw new Exception("Workflow not found");

            if (!workflow.EntityType.Equals(entityType, StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Workflow type mismatch. Expected {workflow.EntityType}, got {entityType}");

            var firstStep = workflow.Steps.OrderBy(s => s.OrderNo).FirstOrDefault()
                ?? throw new Exception("Workflow has no steps");

            var instance = new WorkflowInstance
            {
                WorkflowID = workflowId,
                EntityID = entityId,
                EntityType = entityType,  
                CurrentStepID = firstStep.ID,
                Status = "InProgress"
            };

            await _repository.AddWorkflowInstanceAsync(instance);

            await _repository.AddWorkflowActionLogAsync(new WorkflowActionLog
            {
                WorkflowID = workflowId,
                InstanceID = instance.ID,
                StepID = firstStep.ID,
                Action = "Start",
                EmployeeID = _loggedInUser.EmployeeID,
                Comments = "Workflow started automatically on entity creation",
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation(
                "Workflow started. WorkflowID: {WorkflowId}, EntityID: {EntityId}, EntityType: {EntityType}, InstanceID: {InstanceId}, Step: {StepName}",
                workflowId, entityId, entityType, instance.ID, firstStep.Name);
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
                await StartWorkflow(instance.WorkflowID, entityId, entityType);
            }
        }
    }
}
