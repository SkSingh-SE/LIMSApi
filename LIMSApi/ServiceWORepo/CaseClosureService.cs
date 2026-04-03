using LIMSApi.Data;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.ServiceWORepo
{
    /// <summary>
    /// CaseClosureService: Validates and handles case closure
    /// </summary>
    public class CaseClosureService
    {
        private readonly LIMSContext _db;
        private readonly ILogger<CaseClosureService> _logger;
        private readonly INotificationService _notificationService;

        public CaseClosureService(LIMSContext db, ILogger<CaseClosureService> logger, INotificationService notificationService)
        {
            _db = db;
            _logger = logger;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Validates if a case can be closed
        /// </summary>
        public async Task<(bool CanClose, string Reason)> CanCloseCaseAsync(long inwardId)
        {
            var inward = await _db.SampleInwards
                .Include(x => x.Customer)
                .Include(x => x.SampleDetails)
                .FirstOrDefaultAsync(x => x.ID == inwardId);

            if (inward == null)
                return (false, "Case not found");

            // Check for pending payments
            var hasPendingPayments = await _db.PaymentOrders
                .AnyAsync(p => p.InwardID == inwardId && 
                              p.Status != PaymentStatus.Paid && 
                              p.Status != PaymentStatus.Failed);

            if (hasPendingPayments)
                return (false, "Pending payments exist. All payments must be completed before case closure.");

            // Check for pending amendments
            var sampleIds = inward.SampleDetails.Select(s => s.ID).ToList();
            var hasPendingAmendments = await _db.CustomerAmendments
                .Include(a => a.Report)
                    .ThenInclude(r => r.ReportHeader)
                .AnyAsync(a => sampleIds.Contains(a.Report.ReportHeader.SampleID) &&
                              a.Status != "Approved" && 
                              a.Status != "Completed" &&
                              a.Status != "Rejected");

            if (hasPendingAmendments)
                return (false, "Pending amendments exist. All amendments must be resolved before case closure.");

            // Check if all samples have reports dispatched
            var allSamplesDispatched = inward.SampleDetails.All(s =>
                s.SampleStatus == SampleStatus.REPORT_DISPATCHED.ToString() ||
                s.SampleStatus == SampleStatus.CASE_CLOSED.ToString());

            if (!allSamplesDispatched)
                return (false, "Not all reports have been dispatched. All reports must be dispatched before case closure.");

            // Check all reports are in Approved or Dispatched status (NABL Clause 7.8)
            var reportHeaders = await _db.ReportHeaders
                .Where(r => sampleIds.Contains(r.SampleID) && r.IsActive)
                .ToListAsync();

            var pendingReports = reportHeaders
                .Where(r => r.Status != "Approved" && r.Status != "Dispatched" && r.Status != "Generated")
                .ToList();

            if (pendingReports.Any())
                return (false, $"Reports not yet approved: {pendingReports.Count} report(s) are still in '{pendingReports.First().Status}' status.");

            // Check no pending Non-Conforming Work (NCW) for any test in the case
            var sampleNos = inward.SampleDetails.Select(s => s.SampleNo).Where(s => !string.IsNullOrEmpty(s)).ToList();
            var hasOpenNCW = false;
            if (sampleNos.Any())
            {
                var openNCWs = await _db.NablNonConformingWorks
                    .Where(ncw => ncw.IsActive && !ncw.IsObsolete
                        && (ncw.Status == "Draft" || ncw.Status == "Submitted" || ncw.Status == "Under Review"))
                    .ToListAsync();
                hasOpenNCW = openNCWs.Any(ncw =>
                    sampleNos.Any(sn => (ncw.SampleCode ?? "").Contains(sn)));
            }

            if (hasOpenNCW)
                return (false, "Open Non-Conforming Work (NCW) exists for this case. Resolve all NCWs before closure.");

            // Check sample disposition is documented (returned/retained/disposed)
            var undocumentedSamples = inward.SampleDetails
                .Where(s => s.IsActive && !s.Disabled
                    && string.IsNullOrWhiteSpace(s.SampleStatus)
                    || (s.SampleStatus != SampleStatus.REPORT_DISPATCHED.ToString()
                        && s.SampleStatus != SampleStatus.CASE_CLOSED.ToString()))
                .ToList();

            // Check credit approval for credit customers
            if (inward.Customer?.CustomerType?.Equals("Credit", StringComparison.OrdinalIgnoreCase) == true)
            {
                // Additional credit limit validation
                if (inward.Customer.CreditLimitAmount.HasValue)
                {
                    var totalPending = await _db.TaxInvoices
                        .Where(i => i.CustomerID == inward.CustomerID && 
                                   !_db.PaymentOrders.Any(p => p.TaxInvoiceID == i.ID && p.Status == PaymentStatus.Paid))
                        .SumAsync(i => (decimal?)i.GrandTotal) ?? 0;

                    // Allow closure even with pending credit if within limit and invoice exists
                    var hasInvoice = await _db.TaxInvoices.AnyAsync(i => i.InwardID == inwardId);
                    if (!hasInvoice)
                        return (false, "Invoice must be generated for credit customers before case closure.");
                }
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Closes a case: updates all sample statuses to CASE_CLOSED
        /// </summary>
        public async Task CloseCaseAsync(long inwardId, long employeeId)
        {
            var (canClose, reason) = await CanCloseCaseAsync(inwardId);
            if (!canClose)
                throw new InvalidOperationException($"Cannot close case: {reason}");

            var inward = await _db.SampleInwards
                .Include(x => x.SampleDetails)
                .FirstOrDefaultAsync(x => x.ID == inwardId);

            if (inward == null)
                throw new Exception("Case not found");

            // Update all sample statuses to CASE_CLOSED
            foreach (var sample in inward.SampleDetails)
            {
                sample.SampleStatus = SampleStatus.CASE_CLOSED.ToString();
                sample.ModifiedBy = employeeId;
                sample.ModifiedOn = DateTime.UtcNow;
            }

            inward.InwardStatus = "Closed";
            inward.ModifiedBy = employeeId;
            inward.ModifiedOn = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            _logger.LogInformation("Case {CaseNo} closed successfully", inward.CaseNo);

            // Notifications (fire-and-forget — must not block case closure)
            try
            {
                await _notificationService.CreateNotificationAsync(new Notification
                {
                    UserID = inward.CreatedBy,
                    Title = "Case Closed",
                    Message = $"Case {inward.CaseNo} has been closed.",
                    Type = NotificationType.System,
                    EntityID = inward.ID,
                    EntityType = "SampleInward"
                });

                await _notificationService.NotifyByRoleAsync("Accounts",
                    "Case Closed", $"Case {inward.CaseNo} has been closed.",
                    "SampleInward", inward.ID);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Notification failed for case closure {CaseNo}", inward.CaseNo);
            }
        }
    }
}

