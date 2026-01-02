using LIMSApi.Helpers.Enums;
using LIMSApi.Helpers.StatusFlow.Extensions;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SampleStatusService : ISampleStatusService
    {
        private readonly ISampleStatusRepository _repo;
        private readonly ILogger<SampleStatusService> _logger;

        public SampleStatusService(ISampleStatusRepository repo, ILogger<SampleStatusService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public static readonly Dictionary<SampleStatus, int> WorkflowPriority = new()
            {
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

            // 5 = TESTING / EXECUTION
                { SampleStatus.AMENDMENT_COMPLETED, 5 },
                { SampleStatus.REPORT_AMENDMENT_APPROVED, 5 },
                { SampleStatus.TPI_WAITING_FOR_AGENT, 5 },
                { SampleStatus.TPI_IN_PROGRESS, 5 },
                { SampleStatus.TPI_COMPLETED, 5 },
                { SampleStatus.TESTING_IN_PROGRESS, 5 },
                { SampleStatus.TESTING_COMPLETED, 5 },
                { SampleStatus.PAYMENT_PENDING, 5 },
                { SampleStatus.REQUEST_APPROVED, 5 },
                { SampleStatus.PREPARATION_COMPLETED, 5 },

                // 4 = APPROVED / READY
                { SampleStatus.UNDER_PLANNING, 4 },
                { SampleStatus.UNDER_REVIEW_REQUEST, 4 },
                { SampleStatus.PREPARATION_REQUIRED, 4 },
                { SampleStatus.PREPARATION_IN_PROGRESS, 4 },
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

            //Enum.TryParse(sample.SampleStatus, out SampleStatus current);

            //if (!SampleTransition.IsAllowed(current, newStatus))
            //    return (false, $"Invalid transition: {current} → {newStatus}");

            sample.SampleStatus = newStatus.ToString();
            sample.ModifiedBy = empId;
            sample.ModifiedOn = DateTime.UtcNow;

            await _repo.Save();


            return (true, "Updated");
        }

        #region ForceAuto
        public async Task ForceAutoStatusAsync(long sampleId, SampleStatus newStatus, long empId)
        {
            var sample = await _repo.GetSample(sampleId);
            if (sample == null) return;

            sample.SampleStatus = newStatus.ToString();
            sample.ModifiedBy = empId;
            sample.ModifiedOn = DateTime.UtcNow;

            await _repo.Save();
        }
        #endregion

        public async Task<bool> UpdateInwardStatus(long inwardId, InwardStatus newStaus, long empId)
        {
            try
            {
                var inward = await _repo.GetInward(inwardId);
                if (inward == null) return false;

                inward.InwardStatus = newStaus.ToString();
                inward.ModifiedOn = DateTime.UtcNow;
                inward.ModifiedBy = empId;

                await _repo.Save();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inward status for InwardID: {InwardID}", inwardId);
                return false;
            }
        }

    }

}
