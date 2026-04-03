using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers.Enums;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.ServiceWORepo
{
    public class PaymentGatingService : IPaymentGatingService
    {
        private readonly LIMSContext _db;

        public PaymentGatingService(LIMSContext db)
        {
            _db = db;
        }

        public async Task<PaymentGateResult> CheckPaymentGateAsync(long sampleInwardId)
        {
            var inward = await _db.SampleInwards
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.ID == sampleInwardId);

            if (inward == null)
                throw new Exception($"Sample inward with ID {sampleInwardId} not found.");

            var customer = inward.Customer
                ?? throw new Exception($"Customer not found for inward {sampleInwardId}.");

            var customerType = customer.CustomerType ?? "";

            // ─── WALK-IN: Hard block until payment is received ───
            if (CustomerTypeConstants.IsWalkIn(customerType))
            {
                return await CheckWalkInGateAsync(inward, customer);
            }

            // ─── PO CHECK: If a PO is linked (any customer type can have PO) ───
            if (inward.PurchaseOrderId.HasValue || CustomerTypeConstants.IsPurchaseOrder(customerType))
            {
                return await CheckPurchaseOrderGateAsync(inward, customer);
            }

            // ─── RELATIONSHIP CREDIT: Same soft warning as Credit ───
            if (CustomerTypeConstants.IsRelationshipCredit(customerType))
            {
                return await CheckCreditGateAsync(inward, customer);
            }

            // ─── CREDIT (default): Check credit limit ───
            return await CheckCreditGateAsync(inward, customer);
        }

        public async Task<CreditCheckResult> CheckCreditLimitAsync(long customerId)
        {
            var customer = await _db.Customers
                .FirstOrDefaultAsync(x => x.ID == customerId);

            if (customer == null)
                throw new Exception($"Customer with ID {customerId} not found.");

            var creditLimit = customer.CreditLimitAmount ?? 0;
            var outstanding = await GetOutstandingBalanceAsync(customerId);
            var available = creditLimit - outstanding;

            var result = new CreditCheckResult
            {
                CreditLimit = creditLimit,
                OutstandingBalance = outstanding,
                AvailableCredit = available,
                IsOverLimit = creditLimit > 0 && outstanding > creditLimit
            };

            if (creditLimit > 0)
            {
                var utilizationPercent = (outstanding / creditLimit) * 100;

                if (utilizationPercent >= 120)
                {
                    result.Warning = $"CRITICAL: Customer outstanding ({outstanding:N2}) is at {utilizationPercent:N0}% of credit limit ({creditLimit:N2}). " +
                                     $"Auto-blocked — payment required.";
                }
                else if (utilizationPercent >= 100)
                {
                    result.Warning = $"Customer outstanding ({outstanding:N2}) exceeds credit limit ({creditLimit:N2}). " +
                                     $"Over-limit by {outstanding - creditLimit:N2}. Lab Manager override required.";
                }
                else if (utilizationPercent >= 80)
                {
                    result.Warning = $"Customer is approaching credit limit ({utilizationPercent:N0}% utilized). " +
                                     $"Available credit: {available:N2} of {creditLimit:N2}.";
                }
            }

            return result;
        }

        // ──────────────────────────────────────────
        // Private helpers
        // ──────────────────────────────────────────

        private async Task<PaymentGateResult> CheckWalkInGateAsync(
            Models.SampleInward inward, Models.Customer customer)
        {
            // Walk-in: Check if a proforma invoice exists AND a payment has been received
            var hasProformaInvoice = await _db.ProformaInvoiceHeader
                .AnyAsync(pi => pi.InwardID == inward.ID);

            var hasPaidPayment = await _db.PaymentOrders
                .AnyAsync(p => p.InwardID == inward.ID && p.Status == PaymentStatus.Paid);

            // Also check if advance payment was received on the inward itself
            var hasAdvancePayment = inward.AdvancePayment > 0;

            var isPaid = hasPaidPayment || hasAdvancePayment;

            if (!isPaid)
            {
                // Calculate amount due from proforma invoice or charge events
                decimal amountDue = 0;
                if (hasProformaInvoice)
                {
                    amountDue = await _db.ProformaInvoiceHeader
                        .Where(pi => pi.InwardID == inward.ID)
                        .OrderByDescending(pi => pi.PIDate)
                        .Select(pi => pi.GrandTotal)
                        .FirstOrDefaultAsync();
                }
                else
                {
                    // Fall back to total test charges on the inward
                    amountDue = inward.TotalTestCharges;
                }

                return new PaymentGateResult
                {
                    IsBlocked = true,
                    BlockReason = "Walk-in customer: Payment required before testing can begin. " +
                                  (hasProformaInvoice
                                      ? "Proforma invoice generated but payment not yet received."
                                      : "No proforma invoice generated yet."),
                    CustomerType = CustomerTypeConstants.WalkIn,
                    AmountDue = amountDue > 0 ? amountDue : null
                };
            }

            return new PaymentGateResult
            {
                IsBlocked = false,
                CustomerType = CustomerTypeConstants.WalkIn
            };
        }

        private async Task<PaymentGateResult> CheckCreditGateAsync(
            Models.SampleInward inward, Models.Customer customer)
        {
            var creditLimit = customer.CreditLimitAmount ?? 0;
            var outstanding = await GetOutstandingBalanceAsync(customer.ID);

            var result = new PaymentGateResult
            {
                IsBlocked = false,
                CustomerType = CustomerTypeConstants.Credit,
                CreditLimit = creditLimit > 0 ? creditLimit : null,
                OutstandingBalance = outstanding,
                CreditEscalationLevel = "None"
            };

            if (creditLimit <= 0)
                return result;

            var utilizationPercent = (outstanding / creditLimit) * 100;

            // ── 120%+ AUTO-BLOCK: No override possible ──
            if (utilizationPercent >= 120)
            {
                result.IsBlocked = true;
                result.AllowOverride = false;
                result.CreditEscalationLevel = "AutoBlock";
                result.BlockReason = $"BLOCKED: Customer outstanding ({outstanding:N2}) is at {utilizationPercent:N0}% of credit limit ({creditLimit:N2}). " +
                                     $"Payment required before new testing can begin. No override possible.";
            }
            // ── 100%+ HARD BLOCK: LabManager can override ──
            else if (utilizationPercent >= 100)
            {
                result.IsBlocked = true;
                result.AllowOverride = true;
                result.CreditEscalationLevel = "HardBlock";
                result.BlockReason = $"BLOCKED: Customer outstanding ({outstanding:N2}) has reached {utilizationPercent:N0}% of credit limit ({creditLimit:N2}). " +
                                     $"Lab Manager override with documented justification required to proceed.";
            }
            // ── 80%+ WARNING: Soft advisory ──
            else if (utilizationPercent >= 80)
            {
                result.CreditEscalationLevel = "Warning";
                result.BlockReason = $"Warning: Customer outstanding ({outstanding:N2}) is at {utilizationPercent:N0}% of credit limit ({creditLimit:N2}). " +
                                     $"Available credit: {creditLimit - outstanding:N2}.";
            }

            return result;
        }

        private async Task<PaymentGateResult> CheckPurchaseOrderGateAsync(
            Models.SampleInward inward, Models.Customer customer)
        {
            // Check if a PO is linked to this inward
            if (inward.PurchaseOrderId == null)
            {
                return new PaymentGateResult
                {
                    IsBlocked = false,
                    CustomerType = CustomerTypeConstants.PurchaseOrder,
                    BlockReason = "Warning: No purchase order linked to this inward."
                };
            }

            var po = await _db.CustomerPurchaseOrders
                .FirstOrDefaultAsync(p => p.ID == inward.PurchaseOrderId);

            if (po == null)
            {
                return new PaymentGateResult
                {
                    IsBlocked = false,
                    CustomerType = CustomerTypeConstants.PurchaseOrder,
                    BlockReason = "Warning: Linked purchase order not found."
                };
            }

            if (po.Status != "Active")
            {
                return new PaymentGateResult
                {
                    IsBlocked = false,
                    CustomerType = CustomerTypeConstants.PurchaseOrder,
                    BlockReason = $"Warning: Purchase order {po.PONumber} is {po.Status}."
                };
            }

            var estimatedCharges = inward.TotalTestCharges;
            if (estimatedCharges > 0 && po.RemainingAmount < estimatedCharges)
            {
                return new PaymentGateResult
                {
                    IsBlocked = false,
                    CustomerType = CustomerTypeConstants.PurchaseOrder,
                    BlockReason = $"Warning: PO remaining balance ({po.RemainingAmount:N2}) " +
                                  $"may not cover estimated charges ({estimatedCharges:N2}).",
                    AmountDue = estimatedCharges - po.RemainingAmount
                };
            }

            return new PaymentGateResult
            {
                IsBlocked = false,
                CustomerType = CustomerTypeConstants.PurchaseOrder
            };
        }

        /// <summary>
        /// Calculates a customer's outstanding balance from the ledger.
        /// Outstanding = total debits - total credits (positive means customer owes money).
        /// </summary>
        private async Task<decimal> GetOutstandingBalanceAsync(long customerId)
        {
            // Use CustomerLedger: sum of debits minus sum of credits
            var ledgerTotals = await _db.CustomerLedgers
                .Where(l => l.CustomerId == customerId)
                .GroupBy(l => l.CustomerId)
                .Select(g => new
                {
                    TotalDebit = g.Sum(l => l.DebitAmount),
                    TotalCredit = g.Sum(l => l.CreditAmount)
                })
                .FirstOrDefaultAsync();

            if (ledgerTotals == null)
                return 0;

            // Positive = customer owes; Negative = customer has credit balance
            return ledgerTotals.TotalDebit - ledgerTotals.TotalCredit;
        }
    }
}
