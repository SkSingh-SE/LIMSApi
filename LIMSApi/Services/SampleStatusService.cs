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

            // 6 = UNDER REVIEW
            { SampleStatus.PAYMENT_COMPLETED, 6 },
            { SampleStatus.FINAL_REPORT_APPROVED, 6 },

            // 5 = PARTIALLY COMPLETED
            { SampleStatus.TPI_WAITING_FOR_AGENT, 5 },
            { SampleStatus.TPI_IN_PROGRESS, 5 },
            { SampleStatus.TPI_COMPLETED, 5 },
            { SampleStatus.TESTING_IN_PROGRESS, 5 },
            { SampleStatus.TESTING_COMPLETED, 5 },
            { SampleStatus.REPORT_GENERATION_IN_PROGRESS, 5 },
            { SampleStatus.REPORT_GENERATED, 5 },
            { SampleStatus.REPORT_UNDER_REVIEW, 5 },
            { SampleStatus.REPORT_SENT_FOR_CUSTOMER_REVIEW, 5 },
            { SampleStatus.CUSTOMER_REQUESTED_AMENDMENT, 5 },
            { SampleStatus.AMENDMENT_IN_PROGRESS, 5 },
            { SampleStatus.AMENDMENT_COMPLETED, 5 },
            { SampleStatus.PAYMENT_PENDING, 5 },

            // 4 = UNDER PLANNING
            { SampleStatus.UNDER_REVIEW_REQUEST, 4 },
            { SampleStatus.REQUEST_APPROVED, 4 },
            { SampleStatus.PREPARATION_REQUIRED, 4 },
            { SampleStatus.PREPARATION_IN_PROGRESS, 4 },
            { SampleStatus.PREPARATION_COMPLETED, 4 },
            { SampleStatus.PI_GENERATED, 4 },
            { SampleStatus.ADVANCE_PAYMENT_PENDING, 4 },
            { SampleStatus.ADVANCE_PAYMENT_COMPLETED, 4 },
            { SampleStatus.UNDER_PLANNING, 4 },

            // 2 = IN PROGRESS
            { SampleStatus.AWAITING_MISSING_INFORMATION, 2 },
            { SampleStatus.INWARD_COMPLETED, 2 },

            // 1 = NOT STARTED (default)
            { SampleStatus.SAMPLE_INWARD_REGISTERED, 1 }
        };

        public static readonly Dictionary<int, InwardStatus> PriorityToInward =
            new()
            {
                { 99, InwardStatus.REJECTED },
                { 7,  InwardStatus.COMPLETED },
                { 6,  InwardStatus.UNDER_REVIEW },
                { 5,  InwardStatus.PARTIALLY_COMPLETED },
                { 4,  InwardStatus.UNDER_PLANNING },
                { 2,  InwardStatus.IN_PROGRESS },
                { 1,  InwardStatus.NOT_STARTED }
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

        public async Task<bool> UpdateInwardStatus(long inwardId, long empId)
        {
            try
            {
                var inward = await _repo.GetInward(inwardId);
                if (inward == null) return false;

                var priorities = inward.SampleDetails
                    .Select(s => Enum.TryParse<SampleStatus>(s.SampleStatus, out var st) ? WorkflowPriority[st] : 1)
                    .ToList();

                int maxPriority = priorities.Max();
                inward.InwardStatus = PriorityToInward[maxPriority].ToString();
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


        public InwardStatus MapToInwardStatus(SampleStatus status)
        {
            return status switch
            {
                // NOT STARTED
                SampleStatus.SAMPLE_INWARD_REGISTERED =>
                    InwardStatus.NOT_STARTED,

                // IN PROGRESS
                SampleStatus.AWAITING_MISSING_INFORMATION or
                SampleStatus.INWARD_COMPLETED =>
                    InwardStatus.IN_PROGRESS,

                // UNDER PLANNING
                SampleStatus.UNDER_REVIEW_REQUEST or
                SampleStatus.REQUEST_APPROVED or
                SampleStatus.PREPARATION_REQUIRED or
                SampleStatus.PREPARATION_IN_PROGRESS or
                SampleStatus.PREPARATION_COMPLETED or
                SampleStatus.PI_GENERATED or
                SampleStatus.ADVANCE_PAYMENT_PENDING or
                SampleStatus.ADVANCE_PAYMENT_COMPLETED =>
                    InwardStatus.UNDER_PLANNING,

                // REJECTED
                SampleStatus.REQUEST_REJECTED or
                SampleStatus.REPORT_REJECTED_BY_INTERNAL =>
                    InwardStatus.REJECTED,

                // PARTIALLY COMPLETED
                SampleStatus.TPI_WAITING_FOR_AGENT or
                SampleStatus.TPI_IN_PROGRESS or
                SampleStatus.TPI_COMPLETED or
                SampleStatus.TESTING_IN_PROGRESS or
                SampleStatus.TESTING_COMPLETED or
                SampleStatus.REPORT_GENERATION_IN_PROGRESS or
                SampleStatus.REPORT_GENERATED or
                SampleStatus.REPORT_UNDER_REVIEW or
                SampleStatus.REPORT_SENT_FOR_CUSTOMER_REVIEW or
                SampleStatus.CUSTOMER_REQUESTED_AMENDMENT or
                SampleStatus.AMENDMENT_IN_PROGRESS or
                SampleStatus.AMENDMENT_COMPLETED or
                SampleStatus.PAYMENT_PENDING =>
                    InwardStatus.PARTIALLY_COMPLETED,

                // UNDER REVIEW
                SampleStatus.PAYMENT_COMPLETED or
                SampleStatus.FINAL_REPORT_APPROVED =>
                    InwardStatus.UNDER_REVIEW,

                // COMPLETED
                SampleStatus.REPORT_DISPATCHED or
                SampleStatus.CASE_CLOSED =>
                    InwardStatus.COMPLETED,

                // DEFAULT
                _ => InwardStatus.IN_PROGRESS
            };
        }


    }

}
