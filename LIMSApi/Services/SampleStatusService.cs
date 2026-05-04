using LIMSApi.Data;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SampleStatusService : ISampleStatusService
    {
        private readonly ISampleStatusRepository _repo;
        private readonly LIMSContext _db;
        private readonly ILogger<SampleStatusService> _logger;

        public SampleStatusService(ISampleStatusRepository repo, LIMSContext db, ILogger<SampleStatusService> logger)
        {
            _repo = repo;
            _db = db;
            _logger = logger;
        }

        public static readonly Dictionary<SampleStatus, int> WorkflowPriority = new()
            {
                // 98 = CANCELLED (terminal, distinct from rejected)
                { SampleStatus.SAMPLE_CANCELLED, 98 },

                // 99 = REJECTED
                { SampleStatus.REQUEST_REJECTED, 99 },
                { SampleStatus.REPORT_REJECTED_BY_INTERNAL, 99 },

                // 7 = COMPLETED
                { SampleStatus.REPORT_DISPATCHED, 7 },
                { SampleStatus.CASE_CLOSED, 7 },

                // 6 = REPORTING
                { SampleStatus.REPORT_GENERATION_IN_PROGRESS, 6 },
                { SampleStatus.REPORT_GENERATED, 6 },
                { SampleStatus.REPORT_UNDER_REVIEW, 6 },
                { SampleStatus.REPORT_SENT_FOR_CUSTOMER_REVIEW, 6 },
                { SampleStatus.CUSTOMER_REQUESTED_AMENDMENT, 6 },
                { SampleStatus.AMENDMENT_IN_PROGRESS, 6 },
                { SampleStatus.PAYMENT_COMPLETED, 6 },
                { SampleStatus.FINAL_REPORT_APPROVED, 6 },
                { SampleStatus.REPORT_AMENDED_BY_INTERNAL, 6 },
                { SampleStatus.REPORT_AMENDED_REJECTED, 6 },

            // 5 = TESTING / EXECUTION (Preparation runs parallel with testing)
                { SampleStatus.AMENDMENT_COMPLETED, 5 },
                { SampleStatus.REPORT_AMENDMENT_APPROVED, 5 },
                { SampleStatus.TPI_WAITING_FOR_AGENT, 5 },
                { SampleStatus.TPI_IN_PROGRESS, 5 },
                { SampleStatus.TPI_COMPLETED, 5 },
                { SampleStatus.TESTING_IN_PROGRESS, 5 },
                { SampleStatus.TESTING_COMPLETED, 5 },
                { SampleStatus.TESTING_UNDER_VERIFICATION, 5 },
                { SampleStatus.TESTING_VERIFIED, 5 },
                { SampleStatus.TESTING_VERIFICATION_REJECTED, 5 },
                { SampleStatus.PAYMENT_PENDING, 5 },
                { SampleStatus.REQUEST_APPROVED, 5 },
                { SampleStatus.PREPARATION_REQUIRED, 5 },
                { SampleStatus.PREPARATION_IN_PROGRESS, 5 },
                { SampleStatus.PREPARATION_COMPLETED, 5 },

                // 4 = APPROVED / READY
                { SampleStatus.UNDER_PLANNING, 4 },
                { SampleStatus.UNDER_REVIEW_REQUEST, 4 },
                { SampleStatus.PI_GENERATED, 4 },
                { SampleStatus.ADVANCE_PAYMENT_PENDING, 4 },
                { SampleStatus.ADVANCE_PAYMENT_COMPLETED, 4 },

                // 2 = INWARD
                { SampleStatus.AWAITING_MISSING_INFORMATION, 2 },
                { SampleStatus.INWARD_COMPLETED, 2 },

                // 1 = START
                { SampleStatus.SAMPLE_INWARD_REGISTERED, 1 }
            };

        public async Task<(bool ok, string msg)> UpdateStatusAsync(long sampleId, SampleStatus newStatus, long empId)
        {
            var sample = await _repo.GetSample(sampleId);
            if (sample == null) return (false, "Sample not found");

            var previousStatus = sample.SampleStatus;
            sample.SampleStatus = newStatus.ToString();
            sample.ModifiedBy = empId;
            sample.ModifiedOn = DateTime.UtcNow;

            await _repo.Save();

            await LogStatusChange("Sample", sampleId, previousStatus, newStatus.ToString(), empId, "UpdateStatusAsync");

            return (true, "Updated");
        }

        #region ForceAuto
        public async Task ForceAutoStatusAsync(long sampleId, SampleStatus newStatus, long empId)
        {
            var sample = await _repo.GetSample(sampleId);
            if (sample == null) return;

            var previousStatus = sample.SampleStatus;
            sample.SampleStatus = newStatus.ToString();
            sample.ModifiedBy = empId;
            sample.ModifiedOn = DateTime.UtcNow;

            await _repo.Save();

            await LogStatusChange("Sample", sampleId, previousStatus, newStatus.ToString(), empId, "ForceAutoStatusAsync");
        }
        #endregion

        public async Task<bool> UpdateInwardStatus(long inwardId, InwardStatus newStatus, long empId)
        {
            try
            {
                var inward = await _repo.GetInward(inwardId);
                if (inward == null)
                {
                    _logger.LogError("UpdateInwardStatus: Inward {InwardId} not found — status not updated to {NewStatus}", inwardId, newStatus);
                    return false;
                }

                var previousStatus = inward.InwardStatus;
                inward.InwardStatus = newStatus.ToString();
                inward.ModifiedOn = DateTime.UtcNow;
                inward.ModifiedBy = empId;

                await _repo.Save();

                await LogStatusChange("Inward", inwardId, previousStatus, newStatus.ToString(), empId, "UpdateInwardStatus");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inward status for InwardID: {InwardID}", inwardId);
                return false;
            }
        }

        /// <summary>
        /// Immutable audit log entry. Never updates or deletes.
        /// Resolves employee name from LoggedInUserProvider (already available in request scope).
        /// </summary>
        private async Task LogStatusChange(string entityType, long entityId, string? previousStatus, string newStatus, long changedBy, string source)
            => await LogStatusChangePublic(entityType, entityId, previousStatus, newStatus, changedBy, source);

        /// <summary>
        /// Public overload — allows callers to include a remarks string (e.g. cancellation reason).
        /// Immutable: entries are never updated or deleted.
        /// </summary>
        public async Task LogStatusChangePublic(string entityType, long entityId, string? previousStatus,
            string newStatus, long changedBy, string source, string? remarks = null)
        {
            try
            {
                var entry = new SampleStatusHistory
                {
                    EntityType = entityType,
                    EntityID = entityId,
                    PreviousStatus = previousStatus,
                    NewStatus = newStatus,
                    ChangedBy = changedBy,
                    ChangedByName = LoggedInUserProvider.CurrentUser?.Name,
                    ChangedOn = DateTime.UtcNow,
                    Source = source,
                    Remarks = remarks
                };

                _db.SampleStatusHistories.Add(entry);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Never block the main flow for audit logging failure
                _logger.LogError(ex, "Failed to log status change: {EntityType} {EntityID} → {NewStatus}", entityType, entityId, newStatus);
            }
        }

    }

}
