using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace LIMSApi.ServiceWORepo
{
    public class AccountService : IAccountService
    {
        private readonly LIMSContext _db;
        private readonly EmailService _emailService;
        private readonly WhatsAppService _whatsAppService;
        private readonly InvoicePdfService _invoicePdfService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProformaInvoiceRepository _proformaInvoiceRepository;
        private readonly TemplateService _templateService;
        private readonly ISampleStatusService _sampleStatusService;
        private readonly ICustomerLedgerService _customerLedgerService;
        private readonly INotificationService _notificationService;

        public AccountService(LIMSContext db, EmailService emailService, WhatsAppService whatsAppService, InvoicePdfService invoicePdfService, IPriceCalculationService priceCalculationService, IProformaInvoiceRepository proformaInvoiceRepository, TemplateService templateService, ISampleStatusService sampleStatusService, ICustomerLedgerService customerLedgerService, INotificationService notificationService)
        {
            _db = db;
            _emailService = emailService;
            _whatsAppService = whatsAppService;
            _invoicePdfService = invoicePdfService;
            _priceCalculationService = priceCalculationService;
            _proformaInvoiceRepository = proformaInvoiceRepository;
            _templateService = templateService;
            _sampleStatusService = sampleStatusService;
            _customerLedgerService = customerLedgerService;
            _notificationService = notificationService;
        }

        public async Task<AccountDashboardDto> GetDashboardAsync()
        {
            var user = LoggedInUserProvider.CurrentUser;
            var companyCode = user?.CompanyCode ?? "";

            // Base query — active inwards for this company
            var inwards = _db.SampleInwards.Where(x => x.IsActive && x.CompanyCode == companyCode);

            // Case status counts
            var piPending = await inwards
                .Where(x => !x.PIReceived && x.AdvancePIRequired)
                .CountAsync();

            // Invoice pending: only cases where testing is done (all samples report approved) but invoice not yet generated
            var invoicePending = await inwards
                .Where(x => !x.IsInvoiceGenerated
                    && x.SampleDetails.Any()
                    && x.SampleDetails.All(s => s.SampleStatus == "FINAL_REPORT_APPROVED"
                        || s.SampleStatus == "PAYMENT_PENDING"
                        || s.SampleStatus == "COMPLETED"))
                .CountAsync();

            // Payment Pending: invoice generated but total payments < invoice amount
            var paymentPending = await inwards
                .Where(x => x.IsInvoiceGenerated
                    && x.BillingStatus != "PAYMENT_COMPLETED"
                    && _db.CustomerLedgers
                        .Where(l => l.InwardId == x.ID && l.TransactionType == "Payment" && l.CreditAmount > 0)
                        .Sum(l => (decimal?)l.CreditAmount ?? 0) < _db.TaxInvoices
                        .Where(t => t.InwardID == x.ID)
                        .Sum(t => (decimal?)t.GrandTotal ?? 0))
                .CountAsync();

            var fullySettled = await inwards
                .Where(x => x.IsInvoiceGenerated
                    && (x.BillingStatus == "PAYMENT_COMPLETED"
                        || _db.CustomerLedgers
                            .Where(l => l.InwardId == x.ID && l.TransactionType == "Payment" && l.CreditAmount > 0)
                            .Sum(l => (decimal?)l.CreditAmount) >= _db.TaxInvoices
                            .Where(t => t.InwardID == x.ID)
                            .Sum(t => (decimal?)t.GrandTotal)))
                .CountAsync();

            // Financial summary
            var totalRevenue = await _db.TaxInvoices
                .Where(t => t.IsActive && t.CompanyCode == companyCode)
                .SumAsync(t => (decimal?)t.GrandTotal) ?? 0;

            var today = DateTime.UtcNow.Date;

            var todayCollection = await _db.CustomerLedgers
                .Where(l => l.IsActive && l.CompanyCode == companyCode
                    && l.TransactionType == "Credit" && l.Date >= today)
                .SumAsync(l => (decimal?)l.CreditAmount) ?? 0;

            // Outstanding: sum of all debit - credit per customer where balance > 0
            var totalOutstanding = await _db.CustomerLedgers
                .Where(l => l.IsActive && l.CompanyCode == companyCode)
                .GroupBy(l => l.CustomerId)
                .Select(g => new { Balance = g.Sum(l => l.DebitAmount) - g.Sum(l => l.CreditAmount) })
                .Where(g => g.Balance > 0)
                .SumAsync(g => (decimal?)g.Balance) ?? 0;

            // Overdue: invoices past due date and not fully paid
            var totalOverdue = await _db.TaxInvoices
                .Where(t => t.IsActive && t.CompanyCode == companyCode
                    && t.PaymentDueDate != null && t.PaymentDueDate < today
                    && t.Status != "Paid")
                .SumAsync(t => (decimal?)t.GrandTotal) ?? 0;

            // Customer type breakdown
            var breakdown = await (
                from i in inwards
                join c in _db.Customers on i.CustomerID equals c.ID
                group i by c.CustomerType into g
                select new CustomerTypeBreakdownDto
                {
                    Type = g.Key,
                    CaseCount = g.Count()
                }
            ).ToListAsync();

            // Add outstanding per customer type
            var outstandingByType = await (
                from l in _db.CustomerLedgers.Where(l => l.IsActive && l.CompanyCode == companyCode)
                join c in _db.Customers on l.CustomerId equals c.ID
                group l by c.CustomerType into g
                select new
                {
                    Type = g.Key,
                    Outstanding = g.Sum(l => l.DebitAmount) - g.Sum(l => l.CreditAmount)
                }
            ).ToListAsync();

            foreach (var b in breakdown)
            {
                var match = outstandingByType.FirstOrDefault(o => o.Type == b.Type);
                b.OutstandingAmount = match != null && match.Outstanding > 0 ? match.Outstanding : 0;
            }

            return new AccountDashboardDto
            {
                PiPendingCount = piPending,
                InvoicePendingCount = invoicePending,
                PaymentPendingCount = paymentPending,
                FullySettledCount = fullySettled,
                TotalRevenue = totalRevenue,
                TotalOutstanding = totalOutstanding,
                TotalOverdue = totalOverdue,
                TodayCollection = todayCollection,
                CustomerTypeBreakdown = breakdown
            };
        }

        public async Task<PagedResponse<object>> GetCaseAccountListAsync(PageFilter filter)
        {
            var query =
                (from i in _db.SampleInwards
                 join c in _db.Customers on i.CustomerID equals c.ID
                 where i.IsActive

                 select new
                 {
                     i.ID,
                     InwardId = i.ID,
                     i.CaseNo,
                     CustomerID = c.ID,
                     CustomerName = c.Name,
                     CustomerType = c.CustomerType, // Walk-in / Credit

                     PIStatus = i.AdvancePIRequired ? i.PIReceived ? "Completed" : _db.ProformaInvoiceHeader.Any(x => x.InwardID == i.ID) ? "Generated" : "Pending" : "Completed",
                     InvoiceStatus = i.IsInvoiceGenerated ? "Completed" : "Pending",
                     PaymentStatus = !i.IsInvoiceGenerated ? "N/A"
                         : (i.BillingStatus == "PAYMENT_COMPLETED"
                            || (_db.CustomerLedgers
                                .Where(l => l.InwardId == i.ID && l.TransactionType == "Payment" && l.CreditAmount > 0)
                                .Sum(l => (decimal?)l.CreditAmount) ?? 0) >= (_db.TaxInvoices
                                .Where(t => t.InwardID == i.ID)
                                .Sum(t => (decimal?)t.GrandTotal) ?? 0))
                             ? "Paid"
                             : "Pending",
                     i.CreatedOn,
                     i.ModifiedOn
                 })
                .AsQueryable()
                .ApplyFilters(filter.Filter);

            // ---------------- SEARCH ----------------
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();

                query = query.Where(x =>
                    (x.CaseNo != null && x.CaseNo.Contains(search)) ||
                    (x.CustomerName != null && x.CustomerName.Contains(search)) ||
                    (x.CustomerType != null && x.CustomerType.Contains(search)) ||
                    (x.PIStatus != null && x.PIStatus.Contains(search)) ||
                    (x.InvoiceStatus != null && x.InvoiceStatus.Contains(search)) ||
                    (x.PaymentStatus != null && x.PaymentStatus.Contains(search))
                );
            }

            // ---------------- SORT ----------------
            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                query = query.OrderBy(
                    $"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}"
                );
            }

            // ---------------- COUNT ----------------
            int totalRecords = await query.CountAsync();

            // ---------------- PAGINATION ----------------
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(
                items.Cast<object>().ToList(),
                totalRecords,
                filter.PageNumber,
                filter.PageSize
            );
        }

        public async Task<CaseAccountSummaryDto> GetCaseAccountSummaryAsync(long inwardId)
        {
            var inward = await _db.SampleInwards
                .Include(x => x.Customer)
                .Include(x => x.SampleDetails)
                .FirstOrDefaultAsync(x => x.ID == inwardId);

            if (inward == null)
                throw new Exception("Case not found");

            var hasPendingPayment = await _db.PaymentOrders
                .AnyAsync(p => p.InwardID == inwardId && p.Status != PaymentStatus.Paid);

            // DirectTaxInvoice: skip PI step entirely
            var piStatus = inward.Customer!.DirectTaxInvoiceNoPerforma
                ? "NOT_REQUIRED"
                : inward.PIReceived ? "Completed" : "Pending";

            // Load PI details
            var pi = await _db.ProformaInvoiceHeader
                .Where(p => p.InwardID == inwardId)
                .OrderByDescending(p => p.PIDate)
                .FirstOrDefaultAsync();

            // Update piStatus if PI exists but inward not marked
            if (pi != null && piStatus == "Pending")
                piStatus = "Generated";

            // Load Invoice details
            var invoice = await _db.TaxInvoices
                .Where(i => i.InwardID == inwardId && i.IsActive)
                .OrderByDescending(i => i.InvoiceDate)
                .FirstOrDefaultAsync();

            // Load advance payments from PaymentOrders (Razorpay online)
            var advancePayments = await _db.PaymentOrders
                .Where(p => p.InwardID == inwardId && p.PaymentType == PaymentType.Advance)
                .OrderByDescending(p => p.CreatedOn)
                .Select(p => new AdvancePaymentSummary
                {
                    Id = p.ID,
                    Amount = p.Amount,
                    Date = p.CreatedOn,
                    PaymentMode = p.PaymentType.ToString(),
                    Status = p.Status.ToString()
                })
                .ToListAsync();

            // Include manual advance payments from ledger (credit entries before invoice was generated)
            var manualAdvances = await _db.CustomerLedgers
                .Where(l => l.InwardId == inwardId && l.TransactionType == "Payment" && l.CreditAmount > 0
                    && l.InvoiceId == null) // Exclude payments against final invoice (those are balance payments)
                .OrderByDescending(l => l.Date)
                .Select(l => new AdvancePaymentSummary
                {
                    Id = l.Id,
                    Amount = l.CreditAmount,
                    Date = l.Date,
                    PaymentMode = l.PaymentMode ?? "Manual",
                    Status = "Paid"
                })
                .ToListAsync();

            advancePayments.AddRange(manualAdvances);

            // Determine report status — check ReportHeaders as source of truth
            // (SampleStatus can be overwritten by advance payment or other status changes)
            var activeSampleIds = inward.SampleDetails.Where(s => s.IsActive).Select(s => s.ID).ToList();
            var hasAnyReportHeader = await _db.ReportHeaders.AnyAsync(r => activeSampleIds.Contains(r.SampleID));
            var allReportsApproved = hasAnyReportHeader && await _db.ReportHeaders
                .Where(r => activeSampleIds.Contains(r.SampleID))
                .AllAsync(r => r.Status == "Approved" || r.Status == "Completed" || r.Status == "Dispatched");
            var reportStatus = allReportsApproved ? "FINAL_REPORT_APPROVED" : "PENDING";

            return new CaseAccountSummaryDto
            {
                InwardID = inward.ID,
                CaseNo = inward.CaseNo,
                CustomerName = inward.Customer.Name,
                CustomerType = inward.Customer.CustomerType,
                CustomerId = inward.CustomerID,
                PIStatus = piStatus,
                InvoiceStatus = inward.IsInvoiceGenerated ? "Completed" : "Pending",
                BillingStatus = inward.BillingStatus ?? "",
                ReportStatus = reportStatus,
                HasPendingPayment = hasPendingPayment,

                ProformaInvoice = pi != null ? new ProformaInvoiceSummary
                {
                    Id = pi.ID,
                    PiNo = pi.PINo ?? $"PI-{pi.ID}",
                    PiDate = pi.PIDate,
                    Status = "Generated",
                    SubTotal = pi.SubTotal,
                    TaxAmount = pi.TaxAmount,
                    GrandTotal = pi.GrandTotal,
                    Amount = pi.GrandTotal
                } : null,

                FinalInvoice = invoice != null ? new InvoiceSummary
                {
                    InvoiceId = invoice.ID,
                    InvoiceNo = invoice.InvoiceNo,
                    InvoiceDate = invoice.InvoiceDate,
                    SubTotal = invoice.SubTotal,
                    TaxAmount = invoice.CGST + invoice.SGST + invoice.IGST,
                    GrandTotal = invoice.GrandTotal,
                    Amount = invoice.GrandTotal
                } : null,

                AdvancePayments = advancePayments
            };
        }

        public async Task<PagedResponse<object>> GetCasePaymentListAsync(long inwardId, PageFilter filter)
        {
            // ── 1. Payment Orders (Razorpay / system-generated) ──
            var paymentOrderItems = await _db.PaymentOrders
                .Where(p => p.InwardID == inwardId)
                .Select(p => new
                {
                    id = p.ID,
                    date = p.CreatedOn,
                    paymentType = p.PaymentType.ToString(),
                    source = "PaymentOrder",
                    against = p.SampleID != null ? "Sample" : p.ReportID != null ? "Report" : "Case",
                    reference = p.CaseNo ?? "",
                    amount = p.Amount,
                    status = p.Status.ToString(),
                    receiptNo = "",
                    paymentMode = "",
                    action = (p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Failed) ? "SendLink" : "-",
                    paymentOrderId = (long?)p.ID
                })
                .ToListAsync();

            // ── 2. Ledger Credit Entries (manual payments — Cash, Cheque, NEFT, UPI) ──
            var ledgerCreditItems = await _db.CustomerLedgers
                .Where(l => l.InwardId == inwardId && l.CreditAmount > 0)
                .Select(l => new
                {
                    id = l.Id,
                    date = l.Date,
                    paymentType = "Manual",
                    source = "PaymentReceipt",
                    against = "Case",
                    reference = l.Description ?? "",
                    amount = l.CreditAmount,
                    status = "Paid",
                    receiptNo = l.ReferenceNo ?? "",
                    paymentMode = "",
                    action = "-",
                    paymentOrderId = (long?)null
                })
                .ToListAsync();

            // ── 3. Combine in memory ──
            var allItems = paymentOrderItems
                .Cast<object>()
                .Concat(ledgerCreditItems.Cast<object>())
                .OrderByDescending(x => ((dynamic)x).date)
                .ToList();

            var totalRecords = allItems.Count;

            // Simple pagination in memory
            var items = allItems
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new PagedResponse<object>(
                items,
                totalRecords,
                filter.PageNumber,
                filter.PageSize
            );
        }

        public async Task CreatePriceSnapshotAsync(long inwardId)
        {
            // Role check: Only Accounts role can create price snapshot
            var user = Helpers.LoggedInUserProvider.CurrentUser;
            if (user == null || (user.Role != "Accounts" && !user.Role.Contains("Admin", StringComparison.OrdinalIgnoreCase)))
            {
                throw new UnauthorizedAccessException("Only Accounts role can create price snapshot.");
            }

            await _priceCalculationService.CreatePriceSnapshotAsync(inwardId);
        }

        public async Task<long> GenerateInvoiceAsync(long inwardId)
        {
            // Role check: Only Accounts role can generate invoices
            var user = Helpers.LoggedInUserProvider.CurrentUser;
            if (user == null || (user.Role != "Accounts" && !user.Role.Contains("Admin", StringComparison.OrdinalIgnoreCase)))
            {
                throw new UnauthorizedAccessException("Only Accounts role can generate invoices.");
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            var inward = await _db.SampleInwards
                .Include(x => x.Customer)
                .FirstAsync(x => x.ID == inwardId);

            if (inward.IsInvoiceGenerated)
                throw new Exception("Invoice already generated");

            // Check for existing price snapshot, auto-create if not exists
            var snapshotEvents = await _db.ChargeEvents
                .Where(x => x.InwardID == inwardId && x.Status == ChargeEventStatus.SNAPSHOT.ToString())
                .ToListAsync();

            if (!snapshotEvents.Any())
            {
                // Auto-create snapshot from TestResultHeader saved prices (source of truth)
                // This avoids re-running the pricing engine which may fail if configs changed
                var testHeaders = await _db.TestResultHeaders
                    .Include(h => h.Sample)
                    .Where(h => h.Sample != null && h.Sample.InwardID == inwardId
                        && h.CalculatedPrice != null && h.CalculatedPrice > 0)
                    .ToListAsync();

                if (testHeaders.Any())
                {
                    // Create SNAPSHOT ChargeEvents directly from saved test prices
                    foreach (var header in testHeaders)
                    {
                        var finalPrice = header.OverridePrice ?? header.CalculatedPrice ?? 0;
                        if (finalPrice <= 0) continue;

                        var labTest = await _db.LaboratoryTests.FindAsync(header.LaboratoryTestID);
                        var description = labTest?.SubGroup ?? "Test";
                        if (header.SequenceNo > 1 || testHeaders.Count(h => h.LaboratoryTestID == header.LaboratoryTestID) > 1)
                            description += $" - Specimen {header.SequenceNo}";

                        _db.ChargeEvents.Add(new ChargeEvent
                        {
                            InwardID = inwardId,
                            SampleID = header.SampleID,
                            ChargeType = "Test",
                            Description = description,
                            Amount = finalPrice,
                            Status = ChargeEventStatus.SNAPSHOT.ToString(),
                            SnapshotDate = DateTime.UtcNow,
                            CreatedOn = DateTime.UtcNow
                        });
                    }

                    // Also add cutting/machining charges if any
                    var cuttingTotal = await _db.CuttingChargeHeaders
                        .Where(c => c.InwardID == inwardId && c.IsActive)
                        .SumAsync(c => (decimal?)c.GrandTotal) ?? 0;
                    if (cuttingTotal > 0)
                    {
                        _db.ChargeEvents.Add(new ChargeEvent
                        {
                            InwardID = inwardId,
                            ChargeType = "Cutting",
                            Description = "Cutting Charges",
                            Amount = cuttingTotal,
                            Status = ChargeEventStatus.SNAPSHOT.ToString(),
                            SnapshotDate = DateTime.UtcNow,
                            CreatedOn = DateTime.UtcNow
                        });
                    }

                    await _db.SaveChangesAsync();

                    // Update billing status
                    var inwardForStatus = await _db.SampleInwards.FindAsync(inwardId);
                    if (inwardForStatus != null)
                    {
                        inwardForStatus.BillingStatus = BillingStatus.PRICE_SNAPSHOT.ToString();
                        await _db.SaveChangesAsync();
                    }
                }
                else
                {
                    // Fallback: try pricing engine (may fail if config not set up)
                    try
                    {
                        await _priceCalculationService.CalculateAndCreateChargeEventsAsync(inwardId);
                        await _priceCalculationService.CreatePriceSnapshotAsync(inwardId);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            $"No test pricing found. Either calculate price on Test Result page first, " +
                            $"or configure Invoice Case pricing. Detail: {ex.Message}");
                    }
                }

                // Re-fetch snapshot events
                snapshotEvents = await _db.ChargeEvents
                    .Where(x => x.InwardID == inwardId && x.Status == ChargeEventStatus.SNAPSHOT.ToString())
                    .ToListAsync();

                if (!snapshotEvents.Any())
                    throw new InvalidOperationException(
                        "No charges could be created. Please calculate price on the Test Result page first.");
            }

            // Calculate totals from SNAPSHOT ChargeEvents
            var subTotal = snapshotEvents.Sum(x => x.Amount);

            // ── Customer Discount (applied before GST — standard Indian taxation) ──
            decimal discountPct = 0;
            decimal discountAmt = 0;
            if (inward.Customer?.ConstantDiscount == true
                && inward.Customer.ConstantDiscountPercentage.HasValue
                && inward.Customer.ConstantDiscountPercentage.Value > 0
                && inward.Customer.ConstantDiscountPercentage.Value <= 100)
            {
                discountPct = inward.Customer.ConstantDiscountPercentage.Value;
                discountAmt = Math.Round(subTotal * discountPct / 100m, 2, MidpointRounding.AwayFromZero);
            }
            var discountedSubTotal = subTotal - discountAmt;

            // ── GST from System Configuration (calculated on discounted subtotal) ──
            var gstConfig = await _db.GstConfigs.FirstOrDefaultAsync();
            var gstApplicable = gstConfig?.DefaultGstRate > 0 || gstConfig == null; // Default: GST applicable
            var gstRate = gstConfig?.DefaultGstRate ?? 18m;
            var halfRate = gstRate / 2m;
            var companyState = gstConfig?.State?.Trim().ToLower() ?? "";
            var customerState = inward.State?.Trim().ToLower() ?? "";

            var isInterState = gstApplicable
                && !string.IsNullOrEmpty(companyState)
                && !string.IsNullOrEmpty(customerState)
                && companyState != customerState;

            var customerGstExempt = inward.Customer?.GSTNA ?? false;

            // SpecialAccountingCase: SEZ or No GST = exempt from GST
            var specialCase = inward.Customer?.SpecialAccountingCase ?? "";
            if (specialCase.Equals("SEZ", StringComparison.OrdinalIgnoreCase)
                || specialCase.Equals("No GST applicable", StringComparison.OrdinalIgnoreCase))
            {
                customerGstExempt = true;
            }

            decimal cgst = 0, sgst = 0, igst = 0;

            if (gstApplicable && !customerGstExempt)
            {
                if (isInterState)
                {
                    igst = Math.Round(discountedSubTotal * gstRate / 100m, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    cgst = Math.Round(discountedSubTotal * halfRate / 100m, 2, MidpointRounding.AwayFromZero);
                    sgst = Math.Round(discountedSubTotal * halfRate / 100m, 2, MidpointRounding.AwayFromZero);
                }
            }

            var grandTotal = discountedSubTotal + cgst + sgst + igst;

            // ── PO BALANCE VALIDATION (before creating invoice) ──
            Models.CustomerPurchaseOrder? linkedPO = null;
            if (inward.PurchaseOrderId.HasValue)
            {
                linkedPO = await _db.CustomerPurchaseOrders.FindAsync(inward.PurchaseOrderId.Value);
                if (linkedPO != null && linkedPO.Status == "Active" && linkedPO.RemainingAmount < grandTotal)
                {
                    throw new InvalidOperationException(
                        $"PO remaining balance ({linkedPO.RemainingAmount:N2}) is insufficient for invoice amount ({grandTotal:N2}).");
                }
            }

            // ADVANCE PAYMENT ADJUSTMENT
            var advancePayment = inward.AdvancePayment;

            // If advance exceeds grand total, cap adjustment at grand total (excess stays as credit)
            var advanceAdjusted = Math.Min(advancePayment, grandTotal);
            var balancePayable = grandTotal - advanceAdjusted;

            var currentFY = await _db.FinancialYears.FirstOrDefaultAsync(f => f.IsCurrent);

            var invoice = new TaxInvoice
            {
                InvoiceNo = await GenerateInvoiceNoAsync(),
                InvoiceDate = DateTime.UtcNow,
                InwardID = inward.ID,
                CustomerID = inward.CustomerID,
                PurchaseOrderId = inward.PurchaseOrderId,
                SubTotal = subTotal,
                DiscountPercentage = discountAmt > 0 ? discountPct : null,
                DiscountAmount = discountAmt,
                CGST = cgst,
                SGST = sgst,
                IGST = igst,
                GrandTotal = grandTotal,
                FinancialYearId = currentFY?.Id
            };

            _db.TaxInvoices.Add(invoice);
            await _db.SaveChangesAsync(); // Save to get invoice ID

            // Move ChargeEvents from SNAPSHOT to INVOICED
            foreach (var evt in snapshotEvents)
            {
                evt.Status = ChargeEventStatus.INVOICED.ToString();
                evt.TaxInvoiceID = invoice.ID;
                evt.InvoicedDate = DateTime.UtcNow;
                evt.ModifiedOn = DateTime.UtcNow;
            }

            // ── UPDATE PO UTILIZATION ──
            if (linkedPO != null)
            {
                linkedPO.UtilizedAmount += grandTotal;
                linkedPO.RemainingAmount = linkedPO.POAmount - linkedPO.UtilizedAmount;
                if (linkedPO.RemainingAmount <= 0)
                    linkedPO.Status = "Exhausted";
                linkedPO.ModifiedOn = DateTime.UtcNow;
            }

            // Update inward status
            inward.IsInvoiceGenerated = true;
            inward.BillingStatus = BillingStatus.INVOICE_GENERATED.ToString();
            inward.ModifiedOn = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // ── CREATE LEDGER DEBIT ENTRY ──
            await _customerLedgerService.AddDebitEntry(
                inward.CustomerID,
                grandTotal,
                invoice.InvoiceNo,
                $"Tax Invoice for Case {inward.CaseNo}",
                inwardId,
                invoice.ID);

            await transaction.CommitAsync();

            // ── NOTIFICATION (fire-and-forget — outside transaction) ──
            try
            {
                await _notificationService.CreateNotificationAsync(new Notification
                {
                    UserID = user.EmployeeID,
                    Title = "Invoice Generated",
                    Message = $"Tax Invoice {invoice.InvoiceNo} for Case {inward.CaseNo} — Amount: {grandTotal:N2}",
                    Type = NotificationType.System,
                    EntityID = invoice.ID,
                    EntityType = "TaxInvoice"
                });
            }
            catch (Exception ex)
            {
                // Log but don't fail — invoice is already saved
                System.Diagnostics.Debug.WriteLine($"Notification failed for invoice {invoice.InvoiceNo}: {ex.Message}");
            }

            // ── PO LOW/EXHAUSTED NOTIFICATION ──
            try
            {
                if (linkedPO != null)
                {
                    if (linkedPO.Status == "Exhausted")
                    {
                        await _notificationService.NotifyByRoleAsync("Accounts",
                            "PO Exhausted", $"PO {linkedPO.PONumber} for {inward.Customer?.Name} has been fully utilized.",
                            "CustomerPurchaseOrder", linkedPO.ID);
                    }
                    else if (linkedPO.RemainingAmount <= linkedPO.POAmount * 0.2m)
                    {
                        await _notificationService.NotifyByRoleAsync("Accounts",
                            "PO Balance Low", $"PO {linkedPO.PONumber} for {inward.Customer?.Name} has only {linkedPO.RemainingAmount:N2} remaining ({(linkedPO.RemainingAmount / linkedPO.POAmount * 100):N0}%)",
                            "CustomerPurchaseOrder", linkedPO.ID);
                    }
                }
            }
            catch { /* PO notification failure must not block */ }

            // ── CREDIT LIMIT EXCEEDED NOTIFICATION ──
            try
            {
                var customer = inward.Customer;
                if (customer?.CreditLimitAmount > 0)
                {
                    var outstanding = await _db.CustomerLedgers
                        .Where(l => l.CustomerId == customer.ID)
                        .SumAsync(l => l.DebitAmount - l.CreditAmount);
                    if (outstanding > customer.CreditLimitAmount)
                    {
                        await _notificationService.NotifyByRoleAsync("Accounts",
                            "Credit Limit Exceeded",
                            $"Customer {customer.Name} outstanding ({outstanding:N2}) exceeds credit limit ({customer.CreditLimitAmount:N2})",
                            "Customer", customer.ID);
                    }
                }
            }
            catch { /* credit notification failure must not block */ }

            // Set PAYMENT_PENDING on all active samples for this inward
            var samples = await _db.SampleDetails
                .Where(s => s.InwardID == inward.ID && s.IsActive)
                .ToListAsync();
            foreach (var sample in samples)
            {
                await _sampleStatusService.ForceAutoStatusAsync(
                    sample.ID, SampleStatus.PAYMENT_PENDING, LoggedInUserProvider.CurrentUser?.EmployeeID ?? 0);
            }

            return invoice.ID;
        }


        public async Task SendInvoiceAsync(long invoiceId, bool sendEmail, bool sendWhatsApp)
        {
            var invoice = await _db.TaxInvoices
       .Include(x => x.Customer)
       .Include(x => x.Inward)
       .FirstAsync(x => x.ID == invoiceId);

            var model = MapToPdfModel(invoice);

            var pdfPath = await _invoicePdfService
                .GenerateTaxInvoicePdfAsync(invoice, model);

            invoice.PdfPath = pdfPath;
            invoice.Status = "Sent";
            
            await _db.SaveChangesAsync();

            var modelBody = new Dictionary<string, string>
            {
                { "InvoiceNo", invoice.InvoiceNo },
                { "InvoiceDate", invoice.InvoiceDate.ToString("dd-MM-yyyy") },
                { "CustomerName", invoice.Customer!.Name },
                { "GrandTotal", invoice.GrandTotal.ToString("C") }
            };
            var paths = pdfPath != null ? pdfPath.Split('/').ToList() : new List<string>();
            var fileName = paths.Count > 0 ? paths.Last() : "invoice.pdf";
            if (sendEmail)
            {
                var body =await _templateService.GetTemplateAsync(MessageTemplateKey.FINAL_INVOICE_GENERATED, NotificationType.Email, modelBody);
                await _emailService.SendEmailWithAttachment(
                    invoice.Inward.Contacts.First().EmailId,
                    $"Tax Invoice - {invoice.InvoiceNo}",
                    body,
                    pdfPath,
                    fileName
                );
            }

            if (sendWhatsApp)
            {
                var body = await _templateService.GetTemplateAsync(MessageTemplateKey.FINAL_INVOICE_GENERATED, NotificationType.WhatsApp, modelBody);
                await _whatsAppService.SendWhatsAppMessageAsync(
                    invoice.Inward.Contacts.First().MobileNo,
                    body
                );
            }
        }

        private TaxInvoicePdfModelDto MapToPdfModel(TaxInvoice invoice)
        {
            var inward = invoice.Inward;
            var advancePayment = inward?.AdvancePayment ?? 0;
            var balancePayable = invoice.GrandTotal - advancePayment;

            return new TaxInvoicePdfModelDto
            {
                InvoiceNo = invoice.InvoiceNo,
                InvoiceDate = invoice.InvoiceDate,
                CustomerName = invoice.Customer!.Name,
                CustomerAddress = invoice.Customer.Address,
                CustomerGst = invoice.Customer.GSTNo ?? "",
                SubTotal = invoice.SubTotal,
                DiscountPercentage = invoice.DiscountPercentage,
                DiscountAmount = invoice.DiscountAmount,
                DiscountedSubTotal = invoice.SubTotal - invoice.DiscountAmount,
                CGST = invoice.CGST,
                SGST = invoice.SGST,
                IGST = invoice.IGST,
                GrandTotal = invoice.GrandTotal,
                AdvancePayment = advancePayment,
                BalancePayable = balancePayable
            };
        }

        public async Task<InvoiceDetailsDto> GetInvoiceDetailsAsync(long invoiceId)
        {
            var invoice = await _db.TaxInvoices
                .Include(i => i.Customer)
                .Include(i => i.Inward)
                .FirstOrDefaultAsync(i => i.ID == invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException($"Invoice {invoiceId} not found.");

            var chargeEvents = await _db.ChargeEvents
                .Where(e => e.TaxInvoiceID == invoiceId)
                .OrderBy(e => e.ID)
                .Select(e => new InvoiceChargeLineDto
                {
                    Id = e.ID,
                    ChargeType = e.ChargeType,
                    Description = e.Description ?? e.ChargeType,
                    Amount = e.Amount
                })
                .ToListAsync();

            var adHocLines = await _db.InvoiceLineItems
                .Where(li => li.TaxInvoiceID == invoiceId)
                .OrderBy(li => li.ID)
                .Select(li => new InvoiceAdHocLineDto
                {
                    Id = li.ID,
                    Description = li.Description ?? "",
                    Amount = li.Amount,
                    TaxPercent = li.TaxPercent,
                    TaxAmount = li.TaxAmount,
                    TotalAmount = li.TotalAmount
                })
                .ToListAsync();

            var advance = invoice.Inward?.AdvancePayment ?? 0;
            var poNo = invoice.PurchaseOrderId.HasValue
                ? (await _db.CustomerPurchaseOrders.FindAsync(invoice.PurchaseOrderId.Value))?.PONumber
                : null;

            return new InvoiceDetailsDto
            {
                InvoiceId = invoice.ID,
                InvoiceNo = invoice.InvoiceNo,
                InvoiceDate = invoice.InvoiceDate,
                Status = invoice.Status,
                SACCode = invoice.SACCode,
                PaymentTerms = invoice.PaymentTerms,
                PaymentDueDate = invoice.PaymentDueDate,
                PdfPath = invoice.PdfPath,
                CustomerId = invoice.CustomerID,
                CustomerName = invoice.Customer?.Name ?? "",
                CustomerAddress = invoice.Customer?.Address ?? "",
                CustomerGSTIN = invoice.Customer?.GSTNo ?? "",
                CustomerState = invoice.Customer?.StateID > 0
                    ? (await _db.StateMasters.FindAsync(invoice.Customer.StateID))?.Name ?? ""
                    : "",
                InwardId = invoice.InwardID,
                CaseNo = invoice.Inward?.CaseNo ?? "",
                SubTotal = invoice.SubTotal,
                DiscountPercentage = invoice.DiscountPercentage,
                DiscountAmount = invoice.DiscountAmount,
                TaxableAmount = invoice.SubTotal - invoice.DiscountAmount,
                CGST = invoice.CGST,
                SGST = invoice.SGST,
                IGST = invoice.IGST,
                GrandTotal = invoice.GrandTotal,
                AdvanceAdjusted = advance,
                BalancePayable = invoice.GrandTotal - advance,
                PurchaseOrderNo = poNo,
                ChargeLines = chargeEvents,
                AdHocLines = adHocLines
            };
        }

        public async Task<long> GenerateProformaInvoiceAsync(long inwardId)
        {
            var proformaInvoiceId = await _proformaInvoiceRepository.GeneratePIAsync(inwardId);

            // Notification (fire-and-forget)
            try
            {
                var user = Helpers.LoggedInUserProvider.CurrentUser;
                var inward = await _db.SampleInwards.FindAsync(inwardId);
                await _notificationService.CreateNotificationAsync(new Notification
                {
                    UserID = user?.EmployeeID,
                    Title = "Proforma Invoice Generated",
                    Message = $"PI generated for Case {inward?.CaseNo}",
                    Type = NotificationType.System,
                    EntityID = inwardId,
                    EntityType = "SampleInward"
                });
            }
            catch { /* notification failure must not block PI generation */ }

            return proformaInvoiceId;
        }

        // -----------------------------------------------
        // LEDGER PERIOD-BASED SUMMARY (Gap #16)
        // -----------------------------------------------

        public async Task<LedgerPeriodSummaryDto> GetLedgerPeriodSummaryAsync(long customerId, DateTime periodStart, DateTime periodEnd)
        {
            var customer = await _db.Customers.FindAsync(customerId)
                ?? throw new KeyNotFoundException($"Customer with ID {customerId} not found.");

            // Normalize date range: periodStart = start-of-day, periodEnd = end-of-day
            var periodStartDay = periodStart.Date;
            var periodEndDay = periodEnd.Date.AddDays(1).AddTicks(-1);

            // Opening balance = sum of all ledger entries BEFORE periodStart
            var prePeriodEntries = await _db.CustomerLedgers
                .Where(l => l.CustomerId == customerId && l.Date < periodStartDay && l.IsActive)
                .ToListAsync();

            decimal openingBalance = prePeriodEntries.Sum(l => l.DebitAmount - l.CreditAmount);

            // Entries within the period
            var periodEntries = await _db.CustomerLedgers
                .Where(l => l.CustomerId == customerId && l.Date >= periodStartDay && l.Date <= periodEndDay && l.IsActive)
                .OrderBy(l => l.Date)
                .ThenBy(l => l.Id)
                .ToListAsync();

            decimal totalDebit = periodEntries.Sum(l => l.DebitAmount);
            decimal totalCredit = periodEntries.Sum(l => l.CreditAmount);
            decimal closingBalance = openingBalance + totalDebit - totalCredit;

            // Build entry list with running balance
            decimal runningBalance = openingBalance;
            var entries = periodEntries.Select(l =>
            {
                decimal amount = l.DebitAmount > 0 ? l.DebitAmount : l.CreditAmount;
                string type = l.DebitAmount > 0 ? "Debit" : "Credit";
                runningBalance += l.DebitAmount - l.CreditAmount;

                return new LedgerEntryDto
                {
                    Id = l.Id,
                    Date = l.Date,
                    Description = l.Description ?? l.TransactionType,
                    Type = type,
                    Amount = amount,
                    RunningBalance = runningBalance
                };
            }).ToList();

            return new LedgerPeriodSummaryDto
            {
                CustomerId = customerId,
                CustomerName = customer.Name,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                OpeningBalance = openingBalance,
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                ClosingBalance = closingBalance,
                Entries = entries
            };
        }

        // -----------------------------------------------
        // INVOICE LINE ITEMS (Ad-hoc / Miscellaneous Charges)
        // -----------------------------------------------

        public async Task<List<InvoiceLineItem>> GetLineItemsAsync(long proformaInvoiceHeaderId)
        {
            return await _db.InvoiceLineItems
                .Where(x => x.ProformaInvoiceHeaderID == proformaInvoiceHeaderId)
                .OrderByDescending(x => x.ID)
                .ToListAsync();
        }

        public async Task<List<InvoiceLineItem>> GetLineItemsByTaxInvoiceIdAsync(long taxInvoiceId)
        {
            return await _db.InvoiceLineItems
                .Where(x => x.TaxInvoiceID == taxInvoiceId)
                .OrderByDescending(x => x.ID)
                .ToListAsync();
        }

        public async Task<InvoiceLineItem> CreateLineItemAsync(InvoiceLineItemDto dto)
        {
            var taxAmount = dto.Amount * dto.TaxPercent / 100;
            var lineItem = new InvoiceLineItem
            {
                ProformaInvoiceHeaderID = dto.ProformaInvoiceHeaderID,
                TaxInvoiceID = dto.TaxInvoiceID,
                SampleInwardID = dto.SampleInwardID,
                Description = dto.Description,
                Amount = dto.Amount,
                TaxPercent = dto.TaxPercent,
                TaxAmount = taxAmount,
                TotalAmount = dto.Amount + taxAmount,
                Remark = dto.Remark
            };

            _db.InvoiceLineItems.Add(lineItem);
            await _db.SaveChangesAsync();
            return lineItem;
        }

        public async Task<InvoiceLineItem> UpdateLineItemAsync(long id, InvoiceLineItemDto dto)
        {
            var lineItem = await _db.InvoiceLineItems.FindAsync(id)
                ?? throw new KeyNotFoundException($"InvoiceLineItem with ID {id} not found.");

            var taxAmount = dto.Amount * dto.TaxPercent / 100;

            lineItem.ProformaInvoiceHeaderID = dto.ProformaInvoiceHeaderID;
            lineItem.TaxInvoiceID = dto.TaxInvoiceID;
            lineItem.SampleInwardID = dto.SampleInwardID;
            lineItem.Description = dto.Description;
            lineItem.Amount = dto.Amount;
            lineItem.TaxPercent = dto.TaxPercent;
            lineItem.TaxAmount = taxAmount;
            lineItem.TotalAmount = dto.Amount + taxAmount;
            lineItem.Remark = dto.Remark;

            await _db.SaveChangesAsync();
            return lineItem;
        }

        public async Task DeleteLineItemAsync(long id)
        {
            var lineItem = await _db.InvoiceLineItems.FindAsync(id)
                ?? throw new KeyNotFoundException($"InvoiceLineItem with ID {id} not found.");

            _db.InvoiceLineItems.Remove(lineItem);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Generates sequential invoice number: TI/YY-MM/XXXXXX (max 16 chars per GST Rule 46)
        /// </summary>
        private async Task<string> GenerateInvoiceNoAsync()
        {
            var now = DateTime.UtcNow;
            var prefix = $"TI/{now:yy}-{now:MM}/";
            var lastInvoice = await _db.TaxInvoices
                .Where(i => i.InvoiceNo.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNo)
                .Select(i => i.InvoiceNo)
                .FirstOrDefaultAsync();

            var nextNum = 1;
            if (lastInvoice != null)
            {
                var parts = lastInvoice.Split('/');
                if (parts.Length == 3 && int.TryParse(parts[2], out var lastNum))
                    nextNum = lastNum + 1;
            }

            return $"{prefix}{nextNum:D6}"; // e.g. TI/26-04/000001 (16 chars)
        }
    }

}
