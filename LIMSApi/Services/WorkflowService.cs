using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using LIMSApi.ServiceWORepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly LIMSContext context;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IMemoryCache _cache;
        private readonly IServiceProvider _serviceProvider;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
        private const string WorkflowCachePrefix = "Workflow_";
        private const string WorkflowEntityCachePrefix = "WorkflowEntity_";

        public WorkflowService(IWorkflowRepository WorkflowRepository, ILogger<WorkflowService> logger, INotificationService notification, IEmployeeService employeeService, ISampleInwardRepository sampleInwardRepo, ISampleStatusService statusService, LIMSContext context, IPriceCalculationService priceCalculationService, IMemoryCache cache, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _repository = WorkflowRepository;
            _notificationService = notification;
            _employeeService = employeeService;
            _sampleInwardRepo = sampleInwardRepo;
            _statusService = statusService;
            this.context = context;
            _priceCalculationService = priceCalculationService;
            _cache = cache;
            _serviceProvider = serviceProvider;
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<Workflow?> GetWorkflow(long id)
        {
            var cacheKey = $"{WorkflowCachePrefix}{id}";
            if (_cache.TryGetValue(cacheKey, out Workflow? cached))
                return cached;

            var workflow = await _repository.GetWorkflowByIdAsync(id);
            if (workflow != null)
                _cache.Set(cacheKey, workflow, CacheDuration);
            return workflow;
        }

        public async Task<PagedResponse<object>> GetAllWorkflows(PageFilter filter) =>
            await _repository.GetAllWorkflowsAsync(filter);

        /// <summary>
        /// Gets a workflow definition by entity type, with 30-minute caching.
        /// </summary>
        private async Task<Workflow?> GetWorkflowByEntityTypeCached(string entityType)
        {
            var cacheKey = $"{WorkflowEntityCachePrefix}{entityType}";
            if (_cache.TryGetValue(cacheKey, out Workflow? cached))
                return cached;

            var workflow = await _repository.GetWorkflowByEntityNameAsync(entityType);
            if (workflow != null)
                _cache.Set(cacheKey, workflow, CacheDuration);
            return workflow;
        }

        public async Task<bool> WorkflowExistsForEntityType(string entityType)
        {
            var workflow = await GetWorkflowByEntityTypeCached(entityType);
            return workflow != null;
        }

        /// <summary>
        /// Invalidates workflow caches when a workflow definition is created or updated.
        /// </summary>
        private void InvalidateWorkflowCache(long workflowId, string entityType)
        {
            _cache.Remove($"{WorkflowCachePrefix}{workflowId}");
            _cache.Remove($"{WorkflowEntityCachePrefix}{entityType}");
        }

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

            // Build mapping OrderNo → ID
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
            InvalidateWorkflowCache(workflow.ID, workflow.EntityType);
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
            InvalidateWorkflowCache(workflow.ID, workflow.EntityType);

            _logger.LogInformation("Workflow {WorkflowId} updated with smart sync.", dto.ID);
        }

        // ----------- Transactions ---------------------
        public async Task StartWorkflow(long entityId, string entityType)
        {
            try
            {

                // Cancel existing workflow if any (but not if already completed)
                var existing = await _repository.GetActiveInstanceForEntityAsync(entityId, entityType);
                if (existing != null)
                {
                    // Fix 5B — Don't restart a workflow that already completed
                    if (existing.Status == WorkflowInstanceStatus.Completed.ToString())
                        throw new InvalidOperationException(
                            "A completed workflow already exists for this entity. Cannot restart.");

                    existing.Status = WorkflowInstanceStatus.Cancelled.ToString();
                    existing.IsActive = false;
                    await _repository.UpdateWorkflowInstanceAsync(existing);
                }

                var workflow = await GetWorkflowByEntityTypeCached(entityType)
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

                // Fix 1A — Safe parse for first step approver list
                var approvers = (firstStep.AssignedToValue ?? string.Empty)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => long.TryParse(v.Trim(), out var id) ? id : 0L)
                    .Where(id => id > 0)
                    .ToList();

                if (!approvers.Any())
                    throw new InvalidOperationException($"No valid approvers configured for step '{firstStep.Name}'.");

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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task PerformAction(long instanceId, string action, long employeeId, string comments)
            => await PerformAction(instanceId, action, employeeId, comments, depth: 0);

        private async Task PerformAction(long instanceId, string action, long employeeId, string comments, int depth)
        {
            if (depth > 10)
                throw new InvalidOperationException("Workflow auto-approval recursion limit reached.");

            try
            {
                var instance = await _repository.GetWorkflowInstanceAsync(instanceId)
                    ?? throw new Exception("Instance not found");

                // Fix 5A — Block action on already-completed/cancelled instance
                if (instance.Status == WorkflowInstanceStatus.Completed.ToString() ||
                    instance.Status == WorkflowInstanceStatus.Cancelled.ToString())
                    throw new InvalidOperationException("This workflow instance is already completed or cancelled.");

                var workflow = await _repository.GetWorkflowByIdAsync(instance.WorkflowID)
                    ?? throw new Exception("Workflow not found");

                var currentStep = workflow.Steps.First(s => s.ID == instance.CurrentStepID);

                // Fix 1A — Safe long.TryParse for approver list
                var approvers = (currentStep.AssignedToValue ?? string.Empty)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => long.TryParse(v.Trim(), out var id) ? id : 0L)
                    .Where(id => id > 0)
                    .ToList();

                if (!approvers.Any())
                    throw new InvalidOperationException($"No valid approvers configured for step '{currentStep.Name}'.");

                if (!approvers.Contains(employeeId))
                    throw new Exception("You are not allowed to perform this action");

                // Fix 5C — Comments required for Back/Cancel
                if ((action.Equals(WorkflowActions.Back, StringComparison.OrdinalIgnoreCase) ||
                     action.Equals(WorkflowActions.Cancel, StringComparison.OrdinalIgnoreCase))
                    && string.IsNullOrWhiteSpace(comments))
                    throw new ArgumentException("Comments are required when sending back or rejecting.");

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

                    await ApplyEntityStatusUpdate(instance, action, isFinal: isFinalStep, transition.Alias, comments);
                    return;
                }

                //  Move to Next Step
                if (transition.ToStepID == null)
                {
                    // Final Step Completed
                    instance.Status = WorkflowInstanceStatus.Completed.ToString();
                    instance.IsActive = false;
                    await _repository.UpdateWorkflowInstanceAsync(instance);

                    await ApplyEntityStatusUpdate(instance, action, isFinal: true, transition.Alias, comments);
                    return;
                }

                instance.CurrentStepID = transition.ToStepID.Value;
                instance.Status = WorkflowInstanceStatus.InProgress.ToString();
                await _repository.UpdateWorkflowInstanceAsync(instance);

                await ApplyEntityStatusUpdate(instance, action, isFinal: false, transition.Alias, comments);

                //  Auto-approval if same approver in next step
                var nextStep = workflow.Steps.First(s => s.ID == transition.ToStepID.Value);

                // Fix 1A — Safe parse for next step approvers
                var nextApprovers = (nextStep.AssignedToValue ?? string.Empty)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => long.TryParse(v.Trim(), out var id) ? id : 0L)
                    .Where(id => id > 0)
                    .ToList();

                // Fix 1C — Use HashSet.SetEquals instead of SequenceEqual (order-independent)
                if (nextApprovers.ToHashSet().SetEquals(approvers.ToHashSet()))
                {
                    // Fix 1B — Explicitly pick "Next" transition (not just "not Cancel" which could pick "Back")
                    var nextTransition = nextStep.Transitions
                        .FirstOrDefault(t => t.IsActive && t.Action.Equals(WorkflowActions.Next, StringComparison.OrdinalIgnoreCase));

                    if (nextTransition != null)
                    {
                        await PerformAction(instance.ID, WorkflowActions.Next, employeeId, "Auto-approved (same approver)", depth + 1);
                        return;
                    }
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
            catch (Exception ex)
            {
                throw ex;
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
            await PerformAction(dto.Id, dto.Action, _loggedInUser.EmployeeID, dto.Remarks ?? string.Empty);
        }

        // Phase 9: Batch status check — avoids N+1 queries when checking multiple entities
        public async Task<Dictionary<long, bool>> CanUpdateEntitiesBatch(IEnumerable<long> entityIds, string entityType)
        {
            var instances = await _repository.GetActiveInstancesForEntitiesAsync(entityIds, entityType);
            var result = entityIds.ToDictionary(id => id, _ => true);

            if (!instances.Any())
                return result;

            var instanceIds = instances.Select(i => i.ID).ToList();
            var logCounts = await _repository.GetActionLogCountsForInstancesAsync(instanceIds);

            foreach (var instance in instances)
            {
                var count = logCounts.GetValueOrDefault(instance.ID, 0);
                result[instance.EntityID] = count <= 1;
            }

            return result;
        }

        public async Task<Dictionary<long, WorkflowInstance?>> GetActiveInstancesForEntitiesBatch(IEnumerable<long> entityIds, string entityType)
        {
            var instances = await _repository.GetActiveInstancesForEntitiesAsync(entityIds, entityType);
            var result = entityIds.ToDictionary(id => id, _ => (WorkflowInstance?)null);

            foreach (var instance in instances)
            {
                result[instance.EntityID] = instance;
            }

            return result;
        }

        private async Task ApplyEntityStatusUpdate(WorkflowInstance instance, string action, bool isFinal, string actionName, string? comments = null)
        {
            // Fix 1D — Only skip handler for intermediate Next steps.
            // Back/Cancel MUST update entity status even at non-final steps.
            if (!isFinal && action.Equals(WorkflowActions.Next, StringComparison.OrdinalIgnoreCase))
                return;

            switch (instance.EntityType)
            {
                case "Request Review":
                    await HandleRequestReview(instance.EntityID, action);
                    break;

                case "Report Review":
                    await HandleReportReview(instance.EntityID, action);
                    break;

                case "Report Amendment":
                    await HandleReportAmendment(instance.EntityID, action);
                    break;

                case "Test Result Verification":
                    await HandleTestVerification(instance.EntityID, action);
                    break;

                case "Customer Field Change":
                    await HandleCustomerFieldChange(instance.EntityID, action, comments);
                    break;

                // Fix 1E — Log unhandled entity types instead of silently ignoring
                default:
                    _logger.LogWarning("ApplyEntityStatusUpdate: No handler for EntityType '{EntityType}'", instance.EntityType);
                    break;
            }
        }

        private async Task HandleCustomerFieldChange(long changeRequestId, string action, string? comments)
        {
            var customerService = _serviceProvider.GetRequiredService<ICustomerService>();

            switch (action)
            {
                case WorkflowActions.Next:
                    await customerService.ApplyChangeRequest(changeRequestId);
                    break;
                case WorkflowActions.Back:
                case WorkflowActions.Cancel:
                    await customerService.RejectChangeRequest(changeRequestId, comments);
                    break;
            }
        }
        private async Task HandleRequestReview(long inwardId, string action)
        {
            var inward = await _sampleInwardRepo.GetSampleInwardById(inwardId)
                ?? throw new KeyNotFoundException("Inward not found.");

            if (inward.SampleDetails == null || !inward.SampleDetails.Any())
                throw new KeyNotFoundException("Sample details missing.");

            switch (action)
            {
                case WorkflowActions.Next:
                    foreach (var d in inward.SampleDetails.Where(s => s.IsActive && !s.IsCancelled))
                    {
                        var hasPrep = d.TestPlans.Any(tp => tp.GeneralTests.Any(gt => gt.Methods.Any(m => !m.Cancel && m.PreparationRequired)) || tp.ChemicalTests.Any(ct => ct.Methods.Any(m => !m.Cancel && m.PreparationRequired)));
                        var status = hasPrep
                            ? SampleStatus.PREPARATION_REQUIRED
                            : SampleStatus.REQUEST_APPROVED;

                        await _statusService.ForceAutoStatusAsync(d.ID, status, _loggedInUser.EmployeeID);
                    }

                    var statusUpdated = await _statusService.UpdateInwardStatus(
                        inward.ID,
                        InwardStatus.REVIEW_COMPLETED,
                        _loggedInUser.EmployeeID);

                    if (!statusUpdated)
                        _logger.LogError("HandleRequestReview: Failed to update inward {InwardId} to REVIEW_COMPLETED — status may remain UNDER_REVIEW", inward.ID);

                    // Set plan status to Approved and auto-create TestResultHeaders
                    await ApproveAndCreateTestHeaders(inward);
                    break;

                case WorkflowActions.Back:
                    // Send back for correction — sample returns to UNDER_PLANNING, plans reset to Draft
                    var sampleIdsBack = inward.SampleDetails
                        .Where(s => !s.IsCancelled).Select(d => d.ID).ToList();

                    foreach (var d in inward.SampleDetails.Where(s => !s.IsCancelled))
                    {
                        if (d.SampleStatus == SampleStatus.UNDER_REVIEW_REQUEST.ToString())
                            await _statusService.ForceAutoStatusAsync(
                                d.ID, SampleStatus.UNDER_PLANNING, _loggedInUser.EmployeeID);
                    }

                    var plansToReset = await context.TestPlans
                        .Where(tp => sampleIdsBack.Contains(tp.SampleID) && tp.PlanStatus == "Submitted")
                        .ToListAsync();
                    foreach (var plan in plansToReset)
                        plan.PlanStatus = "Draft";

                    await context.SaveChangesAsync();

                    await _statusService.UpdateInwardStatus(
                        inward.ID, InwardStatus.UNDER_PLANNING, _loggedInUser.EmployeeID);
                    break;

                case WorkflowActions.Cancel:
                    // Hard reject — sample marked REQUEST_REJECTED, plans set to ReplanRequested
                    var sampleIdsCancel = inward.SampleDetails
                        .Where(s => !s.IsCancelled).Select(d => d.ID).ToList();

                    foreach (var d in inward.SampleDetails.Where(s => !s.IsCancelled))
                    {
                        if (d.SampleStatus == SampleStatus.UNDER_REVIEW_REQUEST.ToString())
                            await _statusService.ForceAutoStatusAsync(
                                d.ID, SampleStatus.REQUEST_REJECTED, _loggedInUser.EmployeeID);
                    }

                    var plansToReject = await context.TestPlans
                        .Where(tp => sampleIdsCancel.Contains(tp.SampleID) && tp.PlanStatus == "Submitted")
                        .ToListAsync();
                    foreach (var plan in plansToReject)
                        plan.PlanStatus = "ReplanRequested";

                    await context.SaveChangesAsync();

                    await _statusService.UpdateInwardStatus(
                        inward.ID, InwardStatus.UNDER_PLANNING, _loggedInUser.EmployeeID);
                    break;
            }
        }

        /// <summary>
        /// Approve all test plans for an inward and auto-create TestResultHeaders
        /// </summary>
        private async Task ApproveAndCreateTestHeaders(SampleInward inward)
        {
            try
            {
                // Get all test plans for this inward's samples
                var sampleIds = inward.SampleDetails.Select(d => d.ID).ToList();
                var testPlans = await context.TestPlans
                    .Where(tp => sampleIds.Contains(tp.SampleID) && tp.PlanStatus == "Submitted")
                    .ToListAsync();

                // Resolve ITestResultService lazily to avoid circular dependency
                var testResultService = _serviceProvider.GetRequiredService<ITestResultService>();

                foreach (var plan in testPlans)
                {
                    // Set plan status to Approved
                    plan.PlanStatus = "Approved";
                    plan.ApprovedById = _loggedInUser.EmployeeID;
                    plan.ApprovedByName = _loggedInUser.Name;
                    plan.ApprovedAt = DateTime.UtcNow;

                    // Auto-create TestResultHeaders from plan
                    var result = await testResultService.AutoCreateHeadersFromPlanAsync(plan.ID);

                    if (result.Warnings.Any())
                    {
                        _logger.LogWarning("Plan {PlanId} auto-create warnings: {Warnings}",
                            plan.ID, string.Join("; ", result.Warnings));
                    }

                    _logger.LogInformation("Plan {PlanId} approved. {Count} TestResultHeaders created.",
                        plan.ID, result.HeadersCreated);
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during plan approval and header creation for inward {InwardId}", inward.ID);
                // Don't throw — plan approval should succeed even if header creation fails
                // Headers can be created on-demand during test result entry as fallback
            }
        }

        private async Task HandleReportReview(long reportHeaderId, string action)
        {
            var report = await context.ReportHeaders
                .Include(x => x.Sample)
                    .ThenInclude(s => s.SampleInward)
                .FirstOrDefaultAsync(x => x.ID == reportHeaderId)
                ?? throw new KeyNotFoundException("Report not found.");

            switch (action)
            {
                case WorkflowActions.Next:
                    report.Status = "Completed";
                    await _statusService.ForceAutoStatusAsync(
                        report.SampleID,
                        SampleStatus.FINAL_REPORT_APPROVED,
                        _loggedInUser.EmployeeID);

                    // Report approval implies test results are accepted — mark all as Verified
                    var pendingHeaders = await context.TestResultHeaders
                        .Where(h => h.SampleID == report.SampleID && h.IsActive
                            && (h.Status == "PendingVerification" || h.Status == "Completed"))
                        .ToListAsync();

                    foreach (var h in pendingHeaders)
                    {
                        h.Status = "Verified";
                        h.ModifiedBy = _loggedInUser.EmployeeID;
                        h.ModifiedOn = DateTime.UtcNow;
                    }

                    // After report approval, check if all samples for inward are approved
                    // If all approved, trigger price calculation
                    if (report.Sample != null && report.Sample.SampleInward != null)
                    {
                        var inwardId = report.Sample.InwardID;
                        await TriggerPriceCalculationIfAllSamplesApprovedAsync(inwardId);
                    }
                    break;

                case WorkflowActions.Back:
                    report.Status = "Send Back";
                    // Fix 2B — Back means reporter must fix and resubmit → REPORT_REJECTED_BY_INTERNAL
                    await _statusService.ForceAutoStatusAsync(
                        report.SampleID,
                        SampleStatus.REPORT_REJECTED_BY_INTERNAL,
                        _loggedInUser.EmployeeID);

                    // Report sent back — revert test headers to allow corrections
                    var sentBackHeaders = await context.TestResultHeaders
                        .Where(h => h.SampleID == report.SampleID && h.IsActive
                            && (h.Status == "Verified" || h.Status == "PendingVerification"))
                        .ToListAsync();

                    foreach (var h in sentBackHeaders)
                    {
                        h.Status = "VerificationRejected";
                        h.ModifiedBy = _loggedInUser.EmployeeID;
                        h.ModifiedOn = DateTime.UtcNow;
                    }
                    break;

                case WorkflowActions.Cancel:
                    report.Status = "Rejected";
                    await _statusService.ForceAutoStatusAsync(
                        report.SampleID,
                        SampleStatus.REPORT_REJECTED_BY_INTERNAL,
                        _loggedInUser.EmployeeID);
                    break;
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Triggers price calculation if all samples for an inward have FINAL_REPORT_APPROVED status
        /// </summary>
        private async Task TriggerPriceCalculationIfAllSamplesApprovedAsync(long inwardId)
        {
            try
            {
                // Get all samples for this inward
                var samples = await context.SampleDetails
                    .Where(s => s.InwardID == inwardId && s.IsActive)
                    .ToListAsync();

                if (!samples.Any())
                    return;

                // Fix 2C — Exclude cancelled samples when checking if all approved
                var allSamplesApproved = samples
                    .Where(s => !s.IsCancelled)
                    .All(s => s.SampleStatus == SampleStatus.FINAL_REPORT_APPROVED.ToString());

                if (allSamplesApproved)
                {
                    // Check if DRAFT ChargeEvents already exist (avoid duplicates)
                    var hasExistingDraftChargeEvents = await context.ChargeEvents
                        .AnyAsync(ce => ce.InwardID == inwardId && 
                                       ce.Status == ChargeEventStatus.DRAFT.ToString());

                    if (!hasExistingDraftChargeEvents)
                    {
                        // Trigger price calculation
                        var priceResult = await _priceCalculationService.CalculateAndCreateChargeEventsAsync(inwardId);
                        if (priceResult.HasFailures)
                        {
                            _logger.LogWarning("Price calculation for Inward {InwardID}: {SuccessCount} succeeded, {FailureCount} failed. Errors: {Errors}",
                                inwardId, priceResult.SuccessCount, priceResult.FailureCount, string.Join("; ", priceResult.Errors));
                        }
                        else
                        {
                            _logger.LogInformation("Price calculation triggered for Inward {InwardID}: Total={Total}", inwardId, priceResult.TotalAmount);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Price calculation already done for Inward {InwardID}, skipping", inwardId);
                    }
                }
                else
                {
                    _logger.LogDebug("Not all samples approved for Inward {InwardID}, skipping price calculation", inwardId);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the report approval
                // Price calculation can be retried later
                _logger.LogError(ex, "Failed to trigger price calculation for Inward {InwardID} after report approval", inwardId);
            }
        }
        private async Task HandleReportAmendment(long amendmentId, string action)
        {
            var amendment = await context.AmendmentRequests
                .Include(a => a.ReportHeader)
                .FirstOrDefaultAsync(a => a.ID == amendmentId)
                ?? throw new KeyNotFoundException("Amendment not found.");

            // AUDIT LOGGING: Track who and when (ModifiedBy/ModifiedOn set explicitly for audit trail)
            amendment.ModifiedBy = _loggedInUser.EmployeeID;
            amendment.ModifiedOn = DateTime.UtcNow;

            switch (action)
            {
                case WorkflowActions.Next: // APPROVED
                    amendment.Status = "Approved";
                    amendment.ReportHeader.Status = "Approved";

                    _logger.LogInformation(
                        "Internal amendment {AmendmentID} approved by Employee {EmployeeID}. " +
                        "Reason: {Reason}. Report {ReportHeaderID}.",
                        amendmentId, _loggedInUser.EmployeeID, amendment.Reason, amendment.ReportHeaderID);

                    await _statusService.ForceAutoStatusAsync(
                        amendment.ReportHeader.SampleID,
                        SampleStatus.REPORT_AMENDMENT_APPROVED,
                        _loggedInUser.EmployeeID);
                    break;

                case WorkflowActions.Back: // SEND BACK
                    amendment.Status = "Pending";
                    amendment.ReportHeader.Status = "Under Amendment Review";

                    await _statusService.ForceAutoStatusAsync(
                        amendment.ReportHeader.SampleID,
                        SampleStatus.REPORT_AMENDED_BY_INTERNAL,
                        _loggedInUser.EmployeeID);

                    _logger.LogInformation(
                        "Internal amendment {AmendmentID} sent back by Employee {EmployeeID}.",
                        amendmentId, _loggedInUser.EmployeeID);
                    break;

                case WorkflowActions.Cancel: // REJECT
                    amendment.Status = "Rejected";
                    amendment.ReportHeader.Status = "Report Generated";

                    _logger.LogInformation(
                        "Internal amendment {AmendmentID} rejected by Employee {EmployeeID}. " +
                        "Original reason: {Reason}. Report {ReportHeaderID}.",
                        amendmentId, _loggedInUser.EmployeeID, amendment.Reason, amendment.ReportHeaderID);

                    await _statusService.ForceAutoStatusAsync(
                        amendment.ReportHeader.SampleID,
                        SampleStatus.REPORT_AMENDED_REJECTED,
                        _loggedInUser.EmployeeID);
                    break;
            }

            await context.SaveChangesAsync();
        }

        private async Task HandleTestVerification(long headerId, string action)
        {
            var header = await context.TestResultHeaders
                .FirstOrDefaultAsync(h => h.ID == headerId)
                ?? throw new KeyNotFoundException("Test result header not found.");

            switch (action)
            {
                case WorkflowActions.Next: // VERIFIED
                    header.Status = "Verified";
                    header.ModifiedBy = _loggedInUser.EmployeeID;
                    header.ModifiedOn = DateTime.UtcNow;

                    // Check if ALL tests for this sample are now verified
                    bool allVerified = !await context.TestResultHeaders.AnyAsync(h =>
                        h.SampleID == header.SampleID && h.IsActive && h.ID != headerId
                        && h.Status != "Verified");

                    if (allVerified)
                    {
                        await _statusService.ForceAutoStatusAsync(
                            header.SampleID,
                            SampleStatus.TESTING_VERIFIED,
                            _loggedInUser.EmployeeID);
                    }
                    break;

                // Fix 2D — Back = soft send-back (tester can correct & resubmit)
                case WorkflowActions.Back:
                    header.Status = "NeedsCorrection";
                    header.ModifiedBy = _loggedInUser.EmployeeID;
                    header.ModifiedOn = DateTime.UtcNow;

                    await _statusService.ForceAutoStatusAsync(
                        header.SampleID,
                        SampleStatus.TESTING_VERIFICATION_REJECTED,
                        _loggedInUser.EmployeeID);
                    break;

                // Fix 2D — Cancel = hard reject (cannot resubmit)
                case WorkflowActions.Cancel:
                    header.Status = "VerificationRejected";
                    header.ModifiedBy = _loggedInUser.EmployeeID;
                    header.ModifiedOn = DateTime.UtcNow;

                    await _statusService.ForceAutoStatusAsync(
                        header.SampleID,
                        SampleStatus.TESTING_VERIFICATION_REJECTED,
                        _loggedInUser.EmployeeID);
                    break;
            }

            await context.SaveChangesAsync();
        }

    }
    public static class WorkflowActions
    {
        public const string Next = "Next";
        public const string Back = "Back";
        public const string Cancel = "Cancel";
    }
}
