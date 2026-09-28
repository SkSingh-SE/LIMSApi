using System;
using System.Linq;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class UniversalReviewService : IUniversalReviewService
    {
        private readonly LIMSContext _context;
        private readonly ISnapshotIntegrityValidator _integrity;
        private readonly IUniversalResultService _results;

        public UniversalReviewService(
            LIMSContext context,
            ISnapshotIntegrityValidator integrity,
            IUniversalResultService results)
        {
            _context = context;
            _integrity = integrity;
            _results = results;
        }

        public async Task<UniversalResultDetailDto> AssignReviewerAsync(long resultId, long reviewerId, long userId, long branchId, long organizationId, string? remarks = null)
        {
            var result = await LoadOwnedAsync(resultId, branchId, organizationId);
            EnsureStatus(result, "Finalized", "UnderReview");
            var exec = await _context.TestExecutions.FirstOrDefaultAsync(e => e.ID == result.TestExecutionID)
                ?? throw new KeyNotFoundException("Linked execution not found.");
            var reviewerUserId = await ResolveUserIdByEmployeeAsync(reviewerId);
            if (exec.ExecutionAnalystID.HasValue && reviewerUserId == exec.ExecutionAnalystID.Value)
                throw new InvalidOperationException("Four-eyes violation: the analyst who performed the test cannot be assigned as reviewer.");

            result.ReviewerID = reviewerId;
            if (result.ResultStatus == "Finalized")
                result.ResultStatus = "UnderReview";
            if (!string.IsNullOrWhiteSpace(remarks)) result.ReviewRemarks = remarks.Trim();
            result.ConcurrencyToken = Guid.NewGuid().ToString("N");
            result.ModifiedBy = userId;
            result.ModifiedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await AuditAsync(result, userId, "ReviewerAssigned", $"Reviewer={reviewerId}; remarks={remarks}");
            await _context.SaveChangesAsync();
            return (await _results.GetByIdAsync(result.ID, branchId, organizationId, true))!;
        }

        public async Task<UniversalReviewFindingDto> CreateFindingAsync(long resultId, CreateFindingRequestDto dto, long userId, long branchId, long organizationId)
        {
            var result = await LoadOwnedAsync(resultId, branchId, organizationId);
            if (result.ResultStatus == "Approved")
                throw new InvalidOperationException("Approved results are immutable; open a rework cycle before adding findings.");
            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new ArgumentException("Finding description is required.");
            var allowedTypes = new[] { "Observation", "Nonconformity", "Clarification", "Correction" };
            if (!allowedTypes.Contains(dto.FindingType))
                throw new ArgumentException($"Finding type must be one of: {string.Join(", ", allowedTypes)}.");
            var allowedSev = new[] { "Critical", "Major", "Minor" };
            if (!allowedSev.Contains(dto.Severity))
                throw new ArgumentException("Severity must be Critical, Major or Minor.");

            var finding = new UniversalReviewFinding
            {
                UniversalTestResultID = result.ID,
                TestExecutionID = result.TestExecutionID,
                FindingType = dto.FindingType,
                Description = dto.Description.Trim(),
                Severity = dto.Severity,
                Status = "Open",
                IsBlocking = dto.IsBlocking,
                CompanyCode = result.CompanyCode,
                IsActive = true,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                ModifiedBy = userId,
                ModifiedOn = DateTime.UtcNow
            };
            _context.UniversalReviewFindings.Add(finding);
            await _context.SaveChangesAsync();
            await AuditAsync(result, userId, "FindingCreated", $"Finding {finding.ID}: {finding.FindingType}/{finding.Severity} blocking={finding.IsBlocking}: {finding.Description}");
            await _context.SaveChangesAsync();
            return new UniversalReviewFindingDto
            {
                ID = finding.ID,
                UniversalTestResultID = finding.UniversalTestResultID,
                TestExecutionID = finding.TestExecutionID,
                FindingType = finding.FindingType,
                Description = finding.Description,
                Severity = finding.Severity,
                Status = finding.Status,
                IsBlocking = finding.IsBlocking
            };
        }

        public async Task<UniversalReviewFindingDto> ResolveFindingAsync(long findingId, string resolution, long userId, long branchId, long organizationId)
        {
            var finding = await _context.UniversalReviewFindings
                .Include(f => f.UniversalTestResult)
                .FirstOrDefaultAsync(f => f.ID == findingId && f.IsActive)
                ?? throw new KeyNotFoundException($"Finding {findingId} not found.");
            if (finding.UniversalTestResult == null || finding.UniversalTestResult.OrganizationID != organizationId || finding.UniversalTestResult.BranchID != branchId)
                throw new UnauthorizedAccessException("Finding is not authorized for this branch/organization.");
            if (finding.Status != "Open")
                throw new InvalidOperationException($"Only Open findings can be resolved. Current: '{finding.Status}'.");
            if (string.IsNullOrWhiteSpace(resolution))
                throw new ArgumentException("Resolution is required.");
            finding.Status = "Resolved";
            finding.Resolution = resolution.Trim();
            finding.ResolvedBy = userId;
            finding.ResolvedOn = DateTime.UtcNow;
            finding.ModifiedBy = userId;
            finding.ModifiedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await AuditAsync(finding.UniversalTestResult, userId, "FindingResolved", $"Finding {finding.ID} resolved: {resolution}");
            await _context.SaveChangesAsync();
            return new UniversalReviewFindingDto
            {
                ID = finding.ID,
                UniversalTestResultID = finding.UniversalTestResultID,
                TestExecutionID = finding.TestExecutionID,
                FindingType = finding.FindingType,
                Description = finding.Description,
                Severity = finding.Severity,
                Status = finding.Status,
                IsBlocking = finding.IsBlocking,
                Resolution = finding.Resolution,
                ResolvedBy = finding.ResolvedBy,
                ResolvedOn = finding.ResolvedOn
            };
        }

        public async Task<UniversalResultDetailDto> RequestReworkAsync(long resultId, string concurrencyToken, long userId, long branchId, long organizationId, string? remarks = null)
        {
            var result = await LoadOwnedAsync(resultId, branchId, organizationId);
            CheckToken(result, concurrencyToken);
            if (result.ResultStatus == "Approved")
                throw new InvalidOperationException("Approved results are immutable; rework must create a new revision via re-evaluation, not overwrite.");
            if (result.ResultStatus != "Finalized" && result.ResultStatus != "UnderReview" && result.ResultStatus != "Verified")
                throw new InvalidOperationException($"Rework can only be requested from Finalized/UnderReview/Verified. Current: '{result.ResultStatus}'.");
            if (string.IsNullOrWhiteSpace(remarks))
                throw new ArgumentException("Rework remarks are mandatory.");

            var integrity = await _integrity.ValidateIntegrityAsync(result.TestExecutionID);
            if (!integrity.IsValid)
                throw new InvalidOperationException($"Rework blocked by snapshot integrity violation: {string.Join("; ", integrity.Errors)}");

            result.ResultStatus = "ReworkRequired";
            result.ReviewRemarks = remarks.Trim();
            result.ConcurrencyToken = Guid.NewGuid().ToString("N");
            result.ModifiedBy = userId;
            result.ModifiedOn = DateTime.UtcNow;

            var exec = await _context.TestExecutions.FirstOrDefaultAsync(e => e.ID == result.TestExecutionID);
            if (exec != null && (exec.Status == "Completed" || exec.Status == "Verified"))
            {
                exec.Status = "InProgress";
                exec.ReviewRemarks = $"[REWORK] {remarks.Trim()}";
                exec.ModifiedBy = userId;
                exec.ModifiedOn = DateTime.UtcNow;
                var utg = await _context.UniversalTestGroups.FirstOrDefaultAsync(u => u.ID == exec.UniversalTestGroupID);
                if (utg != null)
                {
                    utg.Status = "InProgress";
                    utg.ModifiedBy = userId;
                    utg.ModifiedOn = DateTime.UtcNow;
                }
            }
            await _context.SaveChangesAsync();
            await AuditAsync(result, userId, "ReworkRequested", remarks);
            await _context.SaveChangesAsync();
            return (await _results.GetByIdAsync(result.ID, branchId, organizationId, true))!;
        }

        public async Task<UniversalResultDetailDto> VerifyAsync(long resultId, string concurrencyToken, long userId, long branchId, long organizationId, string? remarks = null)
        {
            var result = await LoadOwnedAsync(resultId, branchId, organizationId);
            CheckToken(result, concurrencyToken);
            EnsureStatus(result, "Finalized", "UnderReview");

            var exec = await _context.TestExecutions.FirstOrDefaultAsync(e => e.ID == result.TestExecutionID)
                ?? throw new KeyNotFoundException("Linked execution not found.");
            if (exec.ExecutionAnalystID.HasValue && exec.ExecutionAnalystID.Value == userId)
                throw new InvalidOperationException("Four-eyes violation: the analyst who performed the test cannot verify it.");
            if (result.ReviewerID.HasValue)
            {
                var assignedUserId = await ResolveUserIdByEmployeeAsync(result.ReviewerID.Value);
                if (assignedUserId != userId)
                    throw new InvalidOperationException($"This result is assigned to reviewer employee {result.ReviewerID}. Only the assigned reviewer can verify.");
            }

            var integrity = await _integrity.ValidateIntegrityAsync(result.TestExecutionID);
            if (!integrity.IsValid)
                throw new InvalidOperationException($"Verification blocked by snapshot integrity violation: {string.Join("; ", integrity.Errors)}");

            await EnsureLatestRevisionAsync(result);
            var blocking = await _context.UniversalReviewFindings
                .AnyAsync(f => f.UniversalTestResultID == result.ID && f.IsActive && f.IsBlocking && f.Status == "Open");
            if (blocking)
                throw new InvalidOperationException("Verification blocked: unresolved blocking findings exist.");

            result.ResultStatus = "Verified";
            result.VerifiedBy = userId;
            result.VerifiedOn = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(remarks)) result.ReviewRemarks = remarks.Trim();
            result.ConcurrencyToken = Guid.NewGuid().ToString("N");
            result.ModifiedBy = userId;
            result.ModifiedOn = DateTime.UtcNow;

            exec.Status = "Verified";
            exec.VerifiedBy = userId;
            exec.VerifiedOn = DateTime.UtcNow;
            exec.ModifiedBy = userId;
            exec.ModifiedOn = DateTime.UtcNow;
            var utg = await _context.UniversalTestGroups.FirstOrDefaultAsync(u => u.ID == exec.UniversalTestGroupID);
            if (utg != null)
            {
                utg.Status = "Verified";
                utg.ModifiedBy = userId;
                utg.ModifiedOn = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            await AuditAsync(result, userId, "VerificationCompleted", remarks ?? $"Verified revision {result.RevisionNo}.");
            await _context.SaveChangesAsync();
            return (await _results.GetByIdAsync(result.ID, branchId, organizationId, true))!;
        }

        public async Task<UniversalResultDetailDto> ApproveAsync(long resultId, string concurrencyToken, long userId, long branchId, long organizationId, string? remarks = null)
        {
            var result = await LoadOwnedAsync(resultId, branchId, organizationId);
            CheckToken(result, concurrencyToken);
            EnsureStatus(result, "Verified");

            var exec = await _context.TestExecutions.FirstOrDefaultAsync(e => e.ID == result.TestExecutionID)
                ?? throw new KeyNotFoundException("Linked execution not found.");
            if (exec.ExecutionAnalystID.HasValue && exec.ExecutionAnalystID.Value == userId)
                throw new InvalidOperationException("Four-eyes violation: the analyst who performed the test cannot approve it.");
            if (result.VerifiedBy.HasValue && result.VerifiedBy.Value == userId)
                throw new InvalidOperationException("Four-eyes violation: the verifier cannot approve the same result.");
            if (!result.VerifiedBy.HasValue || !result.VerifiedOn.HasValue)
                throw new InvalidOperationException("Approval requires a completed verification first.");

            var integrity = await _integrity.ValidateIntegrityAsync(result.TestExecutionID);
            if (!integrity.IsValid)
                throw new InvalidOperationException($"Approval blocked by snapshot integrity violation: {string.Join("; ", integrity.Errors)}");

            await EnsureLatestRevisionAsync(result);
            var blocking = await _context.UniversalReviewFindings
                .AnyAsync(f => f.UniversalTestResultID == result.ID && f.IsActive && f.IsBlocking && f.Status == "Open");
            if (blocking)
                throw new InvalidOperationException("Approval blocked: unresolved blocking findings exist.");

            result.ResultStatus = "Approved";
            result.ApprovedBy = userId;
            result.ApprovedOn = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(remarks)) result.ApprovalRemarks = remarks.Trim();
            result.ConcurrencyToken = Guid.NewGuid().ToString("N");
            result.ModifiedBy = userId;
            result.ModifiedOn = DateTime.UtcNow;

            exec.Status = "Approved";
            exec.ApprovedBy = userId;
            exec.ApprovedOn = DateTime.UtcNow;
            exec.ModifiedBy = userId;
            exec.ModifiedOn = DateTime.UtcNow;
            var utg = await _context.UniversalTestGroups.FirstOrDefaultAsync(u => u.ID == exec.UniversalTestGroupID);
            if (utg != null)
            {
                utg.Status = "Approved";
                utg.ModifiedBy = userId;
                utg.ModifiedOn = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            await AuditAsync(result, userId, "ApprovalGranted", remarks ?? $"Approved revision {result.RevisionNo}, overall={result.OverallDecision}.");
            await _context.SaveChangesAsync();
            return (await _results.GetByIdAsync(result.ID, branchId, organizationId, true))!;
        }

        public async Task<UniversalResultDetailDto> RejectApprovalAsync(long resultId, string concurrencyToken, long userId, long branchId, long organizationId, string? remarks = null)
        {
            var result = await LoadOwnedAsync(resultId, branchId, organizationId);
            CheckToken(result, concurrencyToken);
            EnsureStatus(result, "Verified");
            if (string.IsNullOrWhiteSpace(remarks))
                throw new ArgumentException("Rejection remarks are mandatory.");
            result.ResultStatus = "ReworkRequired";
            result.ReviewRemarks = remarks.Trim();
            result.ConcurrencyToken = Guid.NewGuid().ToString("N");
            result.ModifiedBy = userId;
            result.ModifiedOn = DateTime.UtcNow;
            var exec = await _context.TestExecutions.FirstOrDefaultAsync(e => e.ID == result.TestExecutionID);
            if (exec != null)
            {
                exec.Status = "InProgress";
                exec.ReviewRemarks = $"[APPROVAL REJECTED] {remarks.Trim()}";
                exec.ModifiedBy = userId;
                exec.ModifiedOn = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            await AuditAsync(result, userId, "ApprovalRejected", remarks);
            await _context.SaveChangesAsync();
            return (await _results.GetByIdAsync(result.ID, branchId, organizationId, true))!;
        }

        private async Task<long> ResolveUserIdByEmployeeAsync(long employeeId)
        {
            var userId = await _context.UserMasters
                .Where(u => u.EmployeeID == employeeId && u.IsActive)
                .Select(u => (long?)u.ID)
                .FirstOrDefaultAsync();
            if (!userId.HasValue)
                throw new InvalidOperationException($"Reviewer employee {employeeId} has no active login account; assignment blocked (fail-closed).");
            return userId.Value;
        }

        private async Task<UniversalTestResult> LoadOwnedAsync(long resultId, long branchId, long organizationId)
        {
            var result = await _context.UniversalTestResults
                .Include(r => r.Parameters)
                .FirstOrDefaultAsync(r => r.ID == resultId && r.OrganizationID == organizationId && r.IsActive)
                ?? throw new KeyNotFoundException($"Result {resultId} not found.");
            if (result.BranchID != branchId)
                throw new UnauthorizedAccessException($"Result belongs to Branch {result.BranchID}, not authorized for Branch {branchId}.");
            return result;
        }

        private static void CheckToken(UniversalTestResult result, string token)
        {
            if (!string.Equals(result.ConcurrencyToken, token, StringComparison.Ordinal))
                throw new InvalidOperationException("Stale result revision. Reload and retry.");
        }

        private static void EnsureStatus(UniversalTestResult result, params string[] allowed)
        {
            if (!allowed.Contains(result.ResultStatus, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Operation requires status {string.Join("/", allowed)}. Current: '{result.ResultStatus}'.");
        }

        private async Task EnsureLatestRevisionAsync(UniversalTestResult result)
        {
            var maxRev = await _context.UniversalTestResults
                .Where(r => r.TestExecutionID == result.TestExecutionID && r.IsActive)
                .MaxAsync(r => r.RevisionNo);
            if (result.RevisionNo != maxRev)
                throw new InvalidOperationException($"Stale revision: result revision {result.RevisionNo} is superseded by revision {maxRev}.");
        }

        private async Task AuditAsync(UniversalTestResult result, long actorId, string eventType, string? details)
        {
            var employeeId = await _context.UserMasters.AsNoTracking()
                .Where(u => u.ID == actorId).Select(u => u.EmployeeID).FirstOrDefaultAsync();
            var name = employeeId.HasValue
                ? await _context.EmployeeMasters.AsNoTracking()
                    .Where(e => e.ID == employeeId.Value).Select(e => e.Name).FirstOrDefaultAsync()
                : null;
            _context.UniversalResultAudits.Add(new UniversalResultAudit
            {
                UniversalTestResultID = result.ID,
                TestExecutionID = result.TestExecutionID,
                RevisionNo = result.RevisionNo,
                EventType = eventType,
                ActorID = actorId,
                ActorName = name ?? $"User {actorId}",
                EventOn = DateTime.UtcNow,
                SnapshotHash = result.SnapshotHash,
                DetailsJson = details,
                CreatedBy = actorId,
                CreatedOn = DateTime.UtcNow,
                CompanyCode = result.CompanyCode,
                IsActive = true
            });
        }
    }
}
