using LIMSApi.Data;
using LIMSApi.Helpers.Enums;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.ServiceWORepo
{
    /// <summary>
    /// Single source of truth for recalculating BillingStatus on a SampleInward.
    /// Called by both PaymentService (Razorpay gateway) and CustomerLedgerService (manual receipt).
    /// Prevents the two paths from using different data sources or conflicting logic.
    /// </summary>
    public class BillingSettlementService
    {
        private readonly LIMSContext _db;
        private readonly ISampleStatusService _sampleStatusService;
        private readonly ILogger<BillingSettlementService> _logger;

        public BillingSettlementService(LIMSContext db, ISampleStatusService sampleStatusService, ILogger<BillingSettlementService> logger)
        {
            _db = db;
            _sampleStatusService = sampleStatusService;
            _logger = logger;
        }

        /// <summary>
        /// Recalculates BillingStatus for an inward based on BOTH payment sources:
        ///   1. PaymentOrders (Razorpay gateway payments)
        ///   2. CustomerLedgers (manual receipt entries)
        /// Picks the HIGHER of the two totals to avoid undercounting.
        /// Also syncs SampleStatus when payment is completed.
        /// </summary>
        public async Task RecalculateBillingStatusAsync(long inwardId, long employeeId)
        {
            var inward = await _db.SampleInwards.FindAsync(inwardId);
            if (inward == null) return;

            // Total invoiced amount
            var totalInvoiced = await _db.TaxInvoices
                .Where(t => t.InwardID == inwardId)
                .SumAsync(t => t.GrandTotal);

            if (totalInvoiced <= 0) return; // No invoice yet — nothing to settle

            // Source 1: Gateway payments (PaymentOrders with Paid status)
            var gatewayPaid = await _db.PaymentOrders
                .Where(p => p.InwardID == inwardId &&
                           (p.PaymentType == PaymentType.Invoice || p.PaymentType == PaymentType.AmendmentInvoice) &&
                           p.Status == PaymentStatus.Paid)
                .SumAsync(p => p.Amount);

            // Source 2: Manual payments (CustomerLedger credit entries)
            var ledgerPaid = await _db.CustomerLedgers
                .Where(l => l.InwardId == inwardId && l.TransactionType == "Payment" && l.IsActive)
                .SumAsync(l => l.CreditAmount);

            // Use the HIGHER of the two — they may overlap or one may be the sole source
            var totalPaid = Math.Max(gatewayPaid, ledgerPaid);

            // Determine new status
            var previousBillingStatus = inward.BillingStatus;

            if (totalPaid >= totalInvoiced)
            {
                inward.BillingStatus = BillingStatus.PAYMENT_COMPLETED.ToString();
                inward.ModifiedOn = DateTime.UtcNow;
                await _db.SaveChangesAsync();

                // Sync SampleStatus for all samples
                var samples = await _db.SampleDetails
                    .Where(s => s.InwardID == inwardId && s.IsActive)
                    .Select(s => s.ID)
                    .ToListAsync();

                foreach (var sampleId in samples)
                {
                    await _sampleStatusService.ForceAutoStatusAsync(
                        sampleId, SampleStatus.PAYMENT_COMPLETED, employeeId);
                }
            }
            else if (totalPaid > 0)
            {
                inward.BillingStatus = BillingStatus.PAYMENT_PARTIAL.ToString();
                inward.ModifiedOn = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            if (previousBillingStatus != inward.BillingStatus)
            {
                _logger.LogInformation(
                    "BillingStatus updated for Inward {InwardId}: {Old} → {New} (invoiced={Invoiced}, paid={Paid})",
                    inwardId, previousBillingStatus, inward.BillingStatus, totalInvoiced, totalPaid);
            }
        }
    }
}
