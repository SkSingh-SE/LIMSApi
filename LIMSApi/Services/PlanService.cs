using LIMSApi.Data;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LIMSApi.Services
{
    public class PlanService : IPlanService
    {
        private readonly LIMSContext _context;
        private readonly ILogger<PlanService> _logger;
        private readonly LoggedInUserDTO loggedInUser;

        public PlanService(LIMSContext context, ILogger<PlanService> logger)
        {
            _context = context;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<List<PlanHistory>> GetPlanHistory(long planId)
        {
            return await _context.PlanHistories
                .Where(h => h.PlanId == planId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }

        public async Task RequestReplan(long planId, string reason)
        {
            var plan = await _context.TestPlans.FirstOrDefaultAsync(p => p.ID == planId);
            if (plan == null)
                throw new Exception($"Plan with ID {planId} not found.");

            if (plan.PlanStatus != "Approved")
                throw new InvalidOperationException("Replan can only be requested for approved plans.");

            var replanRequest = new ReplanRequest
            {
                PlanId = planId,
                RequestedById = loggedInUser.EmployeeID,
                RequestedByName = loggedInUser.Name,
                RequestedAt = DateTime.UtcNow,
                Reason = reason,
                Status = "Pending"
            };

            _context.ReplanRequests.Add(replanRequest);

            plan.PlanStatus = "ReplanRequested";

            // Create history entry
            await CreatePlanHistoryEntry(
                planId,
                "ReplanRequested",
                null,
                null,
                null,
                $"Replan requested. Reason: {reason}"
            );

            await _context.SaveChangesAsync();
            _logger.LogInformation("Replan requested for plan {PlanId} by {UserName}", planId, loggedInUser.Name);
        }

        public async Task ApproveReplan(long requestId, string? remarks)
        {
            var request = await _context.ReplanRequests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null)
                throw new Exception($"Replan request with ID {requestId} not found.");

            if (request.Status != "Pending")
                throw new InvalidOperationException("Only pending replan requests can be approved.");

            var plan = await _context.TestPlans.FirstOrDefaultAsync(p => p.ID == request.PlanId);
            if (plan == null)
                throw new Exception($"Plan with ID {request.PlanId} not found.");

            // Approve the request
            request.Status = "Approved";
            request.ApprovedById = loggedInUser.EmployeeID;
            request.ApprovedByName = loggedInUser.Name;
            request.ApprovedAt = DateTime.UtcNow;
            request.ApprovalRemarks = remarks;

            // Update plan: increment version, reset status, increment replan count
            plan.Version += 1;
            plan.ReplanCount += 1;
            plan.PlanStatus = "Draft";
            plan.ApprovedById = null;
            plan.ApprovedByName = null;
            plan.ApprovedAt = null;

            // Create history entry
            await CreatePlanHistoryEntry(
                plan.ID,
                "ReplanApproved",
                null,
                null,
                JsonSerializer.Serialize(new[]
                {
                    new { field = "Version", oldValue = (plan.Version - 1).ToString(), newValue = plan.Version.ToString() },
                    new { field = "PlanStatus", oldValue = "ReplanRequested", newValue = "Draft" },
                    new { field = "ReplanCount", oldValue = (plan.ReplanCount - 1).ToString(), newValue = plan.ReplanCount.ToString() }
                }),
                $"Replan approved. {(string.IsNullOrWhiteSpace(remarks) ? "" : $"Remarks: {remarks}")}"
            );

            await _context.SaveChangesAsync();
            _logger.LogInformation("Replan approved for plan {PlanId}, new version {Version}", plan.ID, plan.Version);
        }

        public async Task RejectReplan(long requestId, string? remarks)
        {
            var request = await _context.ReplanRequests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null)
                throw new Exception($"Replan request with ID {requestId} not found.");

            if (request.Status != "Pending")
                throw new InvalidOperationException("Only pending replan requests can be rejected.");

            var plan = await _context.TestPlans.FirstOrDefaultAsync(p => p.ID == request.PlanId);
            if (plan == null)
                throw new Exception($"Plan with ID {request.PlanId} not found.");

            // Reject the request
            request.Status = "Rejected";
            request.ApprovedById = loggedInUser.EmployeeID;
            request.ApprovedByName = loggedInUser.Name;
            request.ApprovedAt = DateTime.UtcNow;
            request.ApprovalRemarks = remarks;

            // Restore plan status back to Approved
            plan.PlanStatus = "Approved";

            // Create history entry
            await CreatePlanHistoryEntry(
                plan.ID,
                "ReplanRejected",
                null,
                null,
                null,
                $"Replan request rejected. {(string.IsNullOrWhiteSpace(remarks) ? "" : $"Remarks: {remarks}")}"
            );

            await _context.SaveChangesAsync();
            _logger.LogInformation("Replan rejected for plan {PlanId}", plan.ID);
        }

        public async Task CreatePlanHistoryEntry(long planId, string changeType, string? previousDataJson, string? newDataJson, string? fieldChangesJson, string? remarks)
        {
            var plan = await _context.TestPlans.AsNoTracking().FirstOrDefaultAsync(p => p.ID == planId);
            if (plan == null) return;

            var history = new PlanHistory
            {
                PlanId = planId,
                Version = plan.Version,
                ChangeType = changeType,
                ChangedById = loggedInUser.EmployeeID,
                ChangedByName = loggedInUser.Name,
                ChangedAt = DateTime.UtcNow,
                PreviousDataJson = previousDataJson,
                NewDataJson = newDataJson,
                FieldChangesJson = fieldChangesJson,
                Remarks = remarks
            };

            _context.PlanHistories.Add(history);
        }
    }
}
