using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
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
        private readonly ISampleInwardRepository _sampleInwardRepo;
        private readonly ISampleStatusService _statusService;
        public WorkflowService(IWorkflowRepository WorkflowRepository, ILogger<WorkflowService> logger, INotificationService notification, IEmployeeService employeeService, ISampleInwardRepository sampleInwardRepo , ISampleStatusService statusService)
        {
            _logger = logger;
            _repository = WorkflowRepository;
            _notificationService = notification;
            _employeeService = employeeService;
            _sampleInwardRepo = sampleInwardRepo;
            _statusService = statusService;
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
            try
            {

                // Cancel existing workflow if any
                var existing = await _repository.GetActiveInstanceForEntityAsync(entityId, entityType);
                if (existing != null)
                {
                    existing.Status = WorkflowInstanceStatus.Cancelled.ToString();
                    existing.IsActive = false;
                    await _repository.UpdateWorkflowInstanceAsync(existing);
                }

                var workflow = await _repository.GetWorkflowByEntityNameAsync(entityType)
                    ?? throw new Exception("Workflow not found");

                var firstStep = workflow.Steps
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.OrderNo)
                    .First();

                var instance = new WorkflowInstance
                {
                    WorkflowID = workflow.ID,
                    EntityID = entityId,
                    EntityType = entityType,
                    CurrentStepID = firstStep.ID,
                    Status = WorkflowInstanceStatus.InProgress.ToString(),
                    CreatedBy = _loggedInUser.EmployeeID,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true
                };

                await _repository.AddWorkflowInstanceAsync(instance);

                //  Extract approver list for first step
                var approvers = firstStep.AssignedToValue
                    .Split(',')
                    .Select(long.Parse)
                    .ToList();

                bool creatorIsApprover = approvers.Contains(_loggedInUser.EmployeeID);

                var nextTransition = firstStep.Transitions
                            .Where(t => t.Action == "Next").FirstOrDefault(t => t.ToStepName != "End")
                            ?? firstStep.Transitions.FirstOrDefault(t => t.Action == "Next");


                //  AUTO APPROVE if creator is the ONLY approver
                if (approvers.Count == 1 && creatorIsApprover)
                {
                    await PerformAction(
                        instance.ID,
                        nextTransition?.Action ?? "Next",
                        _loggedInUser.EmployeeID,
                        "Auto-approved (creator is only approver)"
                    );
                    return;
                }

                //  AUTO APPROVE creator's step if he is part of approver group
                if (creatorIsApprover)
                {
                    await PerformAction(
                        instance.ID,
                        nextTransition?.Action ?? "Next",
                        _loggedInUser.EmployeeID,
                        "Auto-approved (creator is approver)"
                    );

                    // Remove creator from list so we don't notify him
                    approvers = approvers.Where(a => a != _loggedInUser.EmployeeID).ToList();
                }

                //  Send notifications ONLY to other approvers
                foreach (var userId in approvers)
                {
                    var user = await _employeeService.GetEmployeeDetails(userId);

                    await _notificationService.CreateNotificationAsync(new Notification
                    {
                        UserID = userId,
                        Email = user?.EmailId,
                        Title = $"Workflow Started: {workflow.Name}",
                        Message = $"Entity {entityType} (ID: {entityId}) requires your approval at step '{firstStep.Name}'.",
                        EntityID = entityId,
                        EntityType = entityType,
                        WorkflowID = workflow.ID,
                        StepID = firstStep.ID,
                        Action = "Pending",
                        CreatedOn = DateTime.UtcNow,
                        IsRead = false
                    });
                }
            }catch(Exception ex)
            {
                throw ex;
            }
        }



        public async Task PerformAction(long instanceId, string action, long employeeId, string comments)
        {
            var instance = await _repository.GetWorkflowInstanceAsync(instanceId)
                ?? throw new Exception("Instance not found");

            var workflow = await _repository.GetWorkflowByIdAsync(instance.WorkflowID)
                ?? throw new Exception("Workflow not found");

            var currentStep = workflow.Steps.First(s => s.ID == instance.CurrentStepID);
            //  Permission Check
            var approvers = currentStep.AssignedToValue.Split(',').Select(long.Parse);
            if (!approvers.Contains(employeeId))
                throw new Exception("You are not allowed to perform this action");

            var transition = currentStep.Transitions.FirstOrDefault(t =>
                t.Action.Equals(action, StringComparison.OrdinalIgnoreCase));

            if (transition == null)
                throw new Exception($"Invalid action '{action}' for step '{currentStep.Name}'");

            bool isFinalStep = transition.ToStepID == null ? true : false;
            //  Log action
            await _repository.AddWorkflowActionLogAsync(new WorkflowActionLog
            {
                WorkflowID = workflow.ID,
                InstanceID = instance.ID,
                StepID = currentStep.ID,
                Action = action,
                EmployeeID = employeeId,
                Comments = comments,
                Timestamp = DateTime.UtcNow
            });

            //  Handle Reject
            if (action.Equals("Cancel", StringComparison.OrdinalIgnoreCase))
            {
                instance.Status = WorkflowInstanceStatus.Rejected.ToString();
                instance.IsActive = false;
                await _repository.UpdateWorkflowInstanceAsync(instance);

                await ApplyEntityStatusUpdate(instance, action, isFinal: isFinalStep);
                return;
            }

            //  Move to Next Step
            if (transition.ToStepID == null)
            {
                // Final Step Completed
                instance.CurrentStepID = 0;
                instance.Status = WorkflowInstanceStatus.Completed.ToString();
                instance.IsActive = false;
                await _repository.UpdateWorkflowInstanceAsync(instance);

                await ApplyEntityStatusUpdate(instance, action, isFinal: true);
                return;
            }

            instance.CurrentStepID = transition.ToStepID.Value;
            instance.Status = WorkflowInstanceStatus.InProgress.ToString();
            await _repository.UpdateWorkflowInstanceAsync(instance);

            await ApplyEntityStatusUpdate(instance, action, isFinal: false);

            //  Auto-approval if same approver in next step
            var nextStep = workflow.Steps.First(s => s.ID == transition.ToStepID.Value);
            var nextApprovers = nextStep.AssignedToValue.Split(',').Select(long.Parse);

            if (nextApprovers.SequenceEqual(approvers))
            {
                await PerformAction(instance.ID, "Approve", employeeId, "Auto-approved (same approver)");
                return;
            }

            //  Notify next approvers
            foreach (var nextUser in nextApprovers)
            {
                await _notificationService.CreateNotificationAsync(new Notification
                {
                    UserID = nextUser,
                    Title = "Action Required",
                    Message = $"Approval required on step '{nextStep.Name}'",
                    EntityID = instance.EntityID,
                    EntityType = instance.EntityType,
                    WorkflowID = instance.WorkflowID,
                    StepID = nextStep.ID,
                    Action = "Pending",
                    CreatedOn = DateTime.UtcNow
                });
            }
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

        public Task<WorkflowStep> GetCurrentWorkflowStepAsync(long entityId, string entityType)
        {
            return _repository.GetCurrentWorkflowStepAsync(entityId, entityType);
        }

        public Task<WorkflowInstance?> GetActiveInstanceForEntityAsync(long entityId, string entityType)
        {
            return _repository.GetActiveInstanceForEntityAsync(entityId, entityType);
        }

        public async Task PerformWorkflowActionAsync(WorkflowActionRequestDto dto)
        {
            await PerformAction(dto.Id, dto.Action, _loggedInUser.EmployeeID, dto.Remarks);

        }

        private async Task ApplyEntityStatusUpdate(WorkflowInstance instance, string action, bool isFinal)
        {
            switch (instance.EntityType)
            {
                case "Request of Review":
                    if (isFinal)
                    {
                        var inward = await _sampleInwardRepo.GetSampleInwardById(instance.EntityID);
                        if (inward != null)
                        {
                            if(inward.SampleDetails == null)
                            {
                                throw new KeyNotFoundException($"No Sample Details found for the Inward Request {instance.EntityID}.");
                            }

                            if (action == "Next")
                            {
                                foreach (var detail in inward.SampleDetails)
                                {
                                    if (detail.PreparationRequired)
                                    {
                                        await _statusService.ForceAutoStatusAsync(detail.ID, SampleStatus.PREPARATION_REQUIRED, _loggedInUser.EmployeeID);
                                    }
                                    else
                                    {
                                        await _statusService.ForceAutoStatusAsync(detail.ID, SampleStatus.REQUEST_APPROVED, _loggedInUser.EmployeeID);
                                    }
                                }
                                
                            }
                            else if (action == "Back")
                            {
                                foreach (var detail in inward.SampleDetails)
                                {
                                    await _statusService.ForceAutoStatusAsync(detail.ID, SampleStatus.UNDER_REVIEW_REQUEST, _loggedInUser.EmployeeID);
                                }
                            }
                            else if (action == "Cancel")
                            {
                                foreach (var detail in inward.SampleDetails)
                                {
                                    await _statusService.ForceAutoStatusAsync(detail.ID, SampleStatus.REQUEST_REJECTED, _loggedInUser.EmployeeID);
                                }
                            }
                        }
                    }
                    break;

                //case "Plan Approval":
                //    if (action == "Approve" && isFinal)
                //        await _planRepository.UpdatePlanStatus(instance.EntityID, "Plan Approved");

                //    if (action == "Reject")
                //        await _planRepository.UpdatePlanStatus(instance.EntityID, "Plan Rejected");
                //    break;

                //case "Sample Inward":
                //    if (action == "Approve" && isFinal)
                //        await _inwardRepository.UpdateInwardStatus(instance.EntityID, "Inward Approved");
                //    break;

                    // Add more modules later if needed
            }
        }

    }
}
