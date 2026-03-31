using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LIMSApi.ServiceWORepo
{
    public class CustomerLedgerService : ICustomerLedgerService
    {
        private readonly LIMSContext _db;
        private readonly BillingSettlementService _billingSettlement;
        private readonly INotificationService _notificationService;

        public CustomerLedgerService(LIMSContext db, BillingSettlementService billingSettlement, INotificationService notificationService)
        {
            _db = db;
            _billingSettlement = billingSettlement;
            _notificationService = notificationService;
        }

        // ===== LEDGER OPERATIONS =====

        public async Task<List<CustomerLedgerEntryDto>> GetCustomerLedger(long customerId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _db.CustomerLedgers
                .Where(x => x.CustomerId == customerId && x.IsActive)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(x => x.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.Date <= toDate.Value);

            return await query
                .OrderBy(x => x.Date).ThenBy(x => x.Id)
                .Select(x => new CustomerLedgerEntryDto
                {
                    Id = x.Id,
                    Date = x.Date,
                    TransactionType = x.TransactionType,
                    ReferenceNo = x.ReferenceNo,
                    Description = x.Description,
                    DebitAmount = x.DebitAmount,
                    CreditAmount = x.CreditAmount,
                    Balance = x.Balance,
                    PaymentMode = x.PaymentMode,
                    Remarks = x.Remarks
                })
                .ToListAsync();
        }

        public async Task<PagedResponse<CustomerLedgerEntryDto>> GetLedgerEntries(long customerId, PageFilter filter)
        {
            var loggedInUser = LoggedInUserProvider.CurrentUser;

            var query = _db.CustomerLedgers
                .Where(l => l.CustomerId == customerId && l.IsActive && l.CompanyCode == loggedInUser.CompanyCode)
                .OrderByDescending(l => l.Date).ThenByDescending(l => l.Id)
                .Select(l => new CustomerLedgerEntryDto
                {
                    Id = l.Id,
                    Date = l.Date,
                    TransactionType = l.TransactionType,
                    ReferenceNo = l.ReferenceNo,
                    Description = l.Description,
                    DebitAmount = l.DebitAmount,
                    CreditAmount = l.CreditAmount,
                    Balance = l.Balance,
                    PaymentMode = l.PaymentMode,
                    Remarks = l.Remarks
                });

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                query = query.Where(x =>
                    (x.ReferenceNo != null && x.ReferenceNo.ToLower().Contains(search)) ||
                    (x.Description != null && x.Description.ToLower().Contains(search)));
            }

            int totalRecords = await query.CountAsync();

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<CustomerLedgerEntryDto>(items, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<CustomerBalanceDto> GetCustomerBalance(long customerId)
        {
            var customer = await _db.Customers
                .Where(x => x.ID == customerId)
                .Select(x => new { x.ID, x.Name })
                .FirstOrDefaultAsync();

            if (customer == null)
                throw new Exception("Customer not found");

            var totals = await _db.CustomerLedgers
                .Where(x => x.CustomerId == customerId && x.IsActive)
                .GroupBy(x => x.CustomerId)
                .Select(g => new
                {
                    TotalDebit = g.Sum(x => x.DebitAmount),
                    TotalCredit = g.Sum(x => x.CreditAmount)
                })
                .FirstOrDefaultAsync();

            return new CustomerBalanceDto
            {
                CustomerId = customer.ID,
                CustomerName = customer.Name,
                TotalInvoiced = totals?.TotalDebit ?? 0,
                TotalPaid = totals?.TotalCredit ?? 0,
                Outstanding = (totals?.TotalDebit ?? 0) - (totals?.TotalCredit ?? 0)
            };
        }

        public async Task<CustomerStatementDto> GetCustomerStatement(long customerId, DateTime fromDate, DateTime toDate)
        {
            var customer = await _db.Customers
                .Where(x => x.ID == customerId)
                .Select(x => new { x.ID, x.Name })
                .FirstOrDefaultAsync();

            if (customer == null)
                throw new Exception("Customer not found");

            var openingBalance = await _db.CustomerLedgers
                .Where(x => x.CustomerId == customerId && x.IsActive && x.Date < fromDate)
                .SumAsync(x => x.DebitAmount - x.CreditAmount);

            var entries = await _db.CustomerLedgers
                .Where(x => x.CustomerId == customerId && x.IsActive && x.Date >= fromDate && x.Date <= toDate)
                .OrderBy(x => x.Date).ThenBy(x => x.Id)
                .Select(x => new CustomerLedgerEntryDto
                {
                    Id = x.Id,
                    Date = x.Date,
                    TransactionType = x.TransactionType,
                    ReferenceNo = x.ReferenceNo,
                    Description = x.Description,
                    DebitAmount = x.DebitAmount,
                    CreditAmount = x.CreditAmount,
                    Balance = x.Balance,
                    PaymentMode = x.PaymentMode,
                    Remarks = x.Remarks
                })
                .ToListAsync();

            var runningBalance = openingBalance;
            foreach (var entry in entries)
            {
                runningBalance += entry.DebitAmount - entry.CreditAmount;
                entry.Balance = runningBalance;
            }

            var totalDebit = entries.Sum(x => x.DebitAmount);
            var totalCredit = entries.Sum(x => x.CreditAmount);

            return new CustomerStatementDto
            {
                CustomerId = customer.ID,
                CustomerName = customer.Name,
                FromDate = fromDate,
                ToDate = toDate,
                OpeningBalance = openingBalance,
                Entries = entries,
                ClosingBalance = openingBalance + totalDebit - totalCredit,
                TotalDebit = totalDebit,
                TotalCredit = totalCredit
            };
        }

        // ===== LEDGER ENTRY CREATION =====

        public async Task AddDebitEntry(long customerId, decimal amount, string referenceNo, string description, long? inwardId = null, long? invoiceId = null)
        {
            var lastBalance = await GetLastBalance(customerId);
            var newBalance = lastBalance + amount;

            var entry = new CustomerLedger
            {
                CustomerId = customerId,
                Date = DateTime.UtcNow,
                TransactionType = "Invoice",
                ReferenceNo = referenceNo,
                Description = description,
                DebitAmount = amount,
                CreditAmount = 0,
                Balance = newBalance,
                InwardId = inwardId,
                InvoiceId = invoiceId
            };

            _db.CustomerLedgers.Add(entry);
            await _db.SaveChangesAsync();
        }

        public async Task AddCreditEntry(long customerId, decimal amount, string referenceNo, string description, string? paymentMode = null, string? chequeNo = null, string? bankName = null, string? transactionRef = null, long? inwardId = null, long? invoiceId = null)
        {
            var lastBalance = await GetLastBalance(customerId);
            var newBalance = lastBalance - amount;

            var entry = new CustomerLedger
            {
                CustomerId = customerId,
                Date = DateTime.UtcNow,
                TransactionType = "Payment",
                ReferenceNo = referenceNo,
                Description = description,
                DebitAmount = 0,
                CreditAmount = amount,
                Balance = newBalance,
                PaymentMode = paymentMode,
                ChequeNo = chequeNo,
                BankName = bankName,
                TransactionRef = transactionRef,
                InwardId = inwardId,
                InvoiceId = invoiceId
            };

            _db.CustomerLedgers.Add(entry);
            await _db.SaveChangesAsync();
        }

        // ===== PAYMENT RECORDING =====

        public async Task<PaymentReceiptDto> RecordPayment(RecordPaymentDto dto)
        {
            var customer = await _db.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
                throw new Exception("Customer not found");

            if (dto.Amount <= 0)
                throw new Exception("Payment amount must be greater than zero");

            var validModes = new[] { "Cash", "Cheque", "UPI", "NEFT", "RTGS", "NEFT/RTGS", "BankTransfer", "Razorpay" };
            if (!validModes.Contains(dto.PaymentMode))
                throw new InvalidOperationException($"Invalid payment mode: {dto.PaymentMode}");

            // Cheque-specific validation
            if (dto.PaymentMode == "Cheque")
            {
                if (string.IsNullOrWhiteSpace(dto.ChequeNo))
                    throw new InvalidOperationException("Cheque number is required for cheque payments.");
                if (!System.Text.RegularExpressions.Regex.IsMatch(dto.ChequeNo.Trim(), @"^\d{6}$"))
                    throw new InvalidOperationException("Cheque number must be exactly 6 digits.");
                if (string.IsNullOrWhiteSpace(dto.BankName))
                    throw new InvalidOperationException("Bank name is required for cheque payments.");
                if (!dto.ChequeDate.HasValue)
                    throw new InvalidOperationException("Cheque date is required for cheque payments.");
            }

            // NEFT/RTGS/UPI-specific validation
            if (dto.PaymentMode is "NEFT" or "RTGS" or "NEFT/RTGS")
            {
                if (string.IsNullOrWhiteSpace(dto.TransactionRef))
                    throw new InvalidOperationException("Transaction reference is required for NEFT/RTGS payments.");
                if (string.IsNullOrWhiteSpace(dto.BankName))
                    throw new InvalidOperationException("Bank name is required for NEFT/RTGS payments.");
            }

            if (dto.PaymentMode == "UPI")
            {
                if (string.IsNullOrWhiteSpace(dto.TransactionRef))
                    throw new InvalidOperationException("Transaction reference is required for UPI payments.");
            }

            // Generate receipt number
            var receiptNo = await GenerateReceiptNo();

            // Create payment receipt
            var receipt = new PaymentReceipt
            {
                ReceiptNo = receiptNo,
                CustomerId = dto.CustomerId,
                Date = DateTime.UtcNow,
                Amount = dto.Amount,
                PaymentMode = dto.PaymentMode,
                ChequeNo = dto.ChequeNo,
                BankName = dto.BankName,
                TransactionRef = dto.TransactionRef,
                InvoiceIds = dto.InvoiceIds != null ? JsonSerializer.Serialize(dto.InvoiceIds) : null,
                Remarks = dto.Remarks,
                Status = "Generated"
            };

            _db.PaymentReceipts.Add(receipt);
            await _db.SaveChangesAsync();

            // Build description
            var description = $"Payment received via {dto.PaymentMode}";
            if (dto.InvoiceIds != null && dto.InvoiceIds.Any())
            {
                var invoiceNos = await _db.TaxInvoices
                    .Where(x => dto.InvoiceIds.Contains(x.ID))
                    .Select(x => x.InvoiceNo)
                    .ToListAsync();

                if (invoiceNos.Any())
                    description += $" against Invoice(s): {string.Join(", ", invoiceNos)}";

                // Update invoice statuses
                decimal remainingAmount = dto.Amount;
                foreach (var invoiceId in dto.InvoiceIds)
                {
                    if (remainingAmount <= 0) break;

                    var invoice = await _db.TaxInvoices.FindAsync(invoiceId);
                    if (invoice != null && invoice.Status != "Paid")
                    {
                        var paidAmount = await _db.CustomerLedgers
                            .Where(l => l.InvoiceId == invoiceId && l.TransactionType == "Payment" && l.IsActive)
                            .SumAsync(l => l.CreditAmount);

                        var invoiceOutstanding = invoice.GrandTotal - paidAmount;
                        if (invoiceOutstanding > 0)
                        {
                            var adjustAmount = Math.Min(remainingAmount, invoiceOutstanding);
                            remainingAmount -= adjustAmount;

                            invoice.Status = adjustAmount >= invoiceOutstanding ? "Paid" : "PartiallyPaid";
                        }
                    }
                }
            }

            // Create credit entry in ledger
            await AddCreditEntry(
                dto.CustomerId,
                dto.Amount,
                receiptNo,
                description,
                dto.PaymentMode,
                dto.ChequeNo,
                dto.BankName,
                dto.TransactionRef,
                dto.InwardId
            );

            // Unified billing settlement — single source of truth
            if (dto.InwardId.HasValue)
            {
                var employeeId = LIMSApi.Helpers.LoggedInUserProvider.CurrentUser?.EmployeeID ?? 0;
                await _billingSettlement.RecalculateBillingStatusAsync(dto.InwardId.Value, employeeId);
            }

            // Notify current user
            var currentUser = LIMSApi.Helpers.LoggedInUserProvider.CurrentUser;
            await _notificationService.CreateNotificationAsync(new Notification
            {
                UserID = currentUser?.EmployeeID,
                Title = "Payment Recorded",
                Message = $"Payment of {dto.Amount:N2} from {customer.Name} via {dto.PaymentMode}. Receipt: {receiptNo}",
                Type = NotificationType.System,
                EntityID = receipt.Id,
                EntityType = "PaymentReceipt"
            });

            // Notify case creator if different from current user
            if (dto.InwardId.HasValue)
            {
                var inward = await _db.SampleInwards.FindAsync(dto.InwardId.Value);
                if (inward != null && inward.CreatedBy != currentUser?.EmployeeID)
                {
                    await _notificationService.CreateNotificationAsync(new Notification
                    {
                        UserID = inward.CreatedBy,
                        Title = "Payment Received",
                        Message = $"Payment of {dto.Amount:N2} received for Case {inward.CaseNo} via {dto.PaymentMode}",
                        Type = NotificationType.System,
                        EntityID = inward.ID,
                        EntityType = "SampleInward"
                    });
                }
            }

            return new PaymentReceiptDto
            {
                Id = receipt.Id,
                ReceiptNo = receipt.ReceiptNo,
                CustomerId = receipt.CustomerId,
                CustomerName = customer.Name,
                Date = receipt.Date,
                Amount = receipt.Amount,
                PaymentMode = receipt.PaymentMode,
                ChequeNo = receipt.ChequeNo,
                BankName = receipt.BankName,
                TransactionRef = receipt.TransactionRef,
                InvoiceIds = receipt.InvoiceIds,
                Remarks = receipt.Remarks,
                Status = receipt.Status
            };
        }

        public async Task<List<PaymentReceiptDto>> GetPaymentReceipts(long customerId)
        {
            return await _db.PaymentReceipts
                .Where(x => x.CustomerId == customerId && x.IsActive)
                .OrderByDescending(x => x.Date)
                .Select(x => new PaymentReceiptDto
                {
                    Id = x.Id,
                    ReceiptNo = x.ReceiptNo,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer!.Name,
                    Date = x.Date,
                    Amount = x.Amount,
                    PaymentMode = x.PaymentMode,
                    ChequeNo = x.ChequeNo,
                    BankName = x.BankName,
                    TransactionRef = x.TransactionRef,
                    InvoiceIds = x.InvoiceIds,
                    Remarks = x.Remarks,
                    Status = x.Status
                })
                .ToListAsync();
        }

        public async Task<PaymentReceiptDto?> GetReceipt(long receiptId)
        {
            var loggedInUser = LoggedInUserProvider.CurrentUser;

            return await _db.PaymentReceipts
                .Include(r => r.Customer)
                .Where(r => r.Id == receiptId && r.IsActive && r.CompanyCode == loggedInUser.CompanyCode)
                .Select(r => new PaymentReceiptDto
                {
                    Id = r.Id,
                    ReceiptNo = r.ReceiptNo,
                    CustomerId = r.CustomerId,
                    CustomerName = r.Customer!.Name,
                    Date = r.Date,
                    Amount = r.Amount,
                    PaymentMode = r.PaymentMode,
                    ChequeNo = r.ChequeNo,
                    BankName = r.BankName,
                    TransactionRef = r.TransactionRef,
                    InvoiceIds = r.InvoiceIds,
                    Remarks = r.Remarks,
                    Status = r.Status
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResponse<PaymentReceiptDto>> GetReceiptsPaged(long customerId, PageFilter filter)
        {
            var loggedInUser = LoggedInUserProvider.CurrentUser;

            var query = _db.PaymentReceipts
                .Include(r => r.Customer)
                .Where(r => r.CustomerId == customerId && r.IsActive && r.CompanyCode == loggedInUser.CompanyCode)
                .OrderByDescending(r => r.Date)
                .Select(r => new PaymentReceiptDto
                {
                    Id = r.Id,
                    ReceiptNo = r.ReceiptNo,
                    CustomerId = r.CustomerId,
                    CustomerName = r.Customer!.Name,
                    Date = r.Date,
                    Amount = r.Amount,
                    PaymentMode = r.PaymentMode,
                    ChequeNo = r.ChequeNo,
                    BankName = r.BankName,
                    TransactionRef = r.TransactionRef,
                    InvoiceIds = r.InvoiceIds,
                    Remarks = r.Remarks,
                    Status = r.Status
                });

            int totalRecords = await query.CountAsync();

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<PaymentReceiptDto>(items, totalRecords, filter.PageNumber, filter.PageSize);
        }

        // ===== REPORTS =====

        public async Task<List<AgingReportDto>> GetAgingReport(DateTime? asOfDate = null)
        {
            var refDate = asOfDate ?? DateTime.UtcNow;

            var customers = await _db.Customers
                .Where(c => c.IsActive)
                .Select(c => new { c.ID, c.Name })
                .ToListAsync();

            var result = new List<AgingReportDto>();

            foreach (var customer in customers)
            {
                var invoiceEntries = await _db.CustomerLedgers
                    .Where(x => x.CustomerId == customer.ID && x.IsActive && x.TransactionType == "Invoice" && x.Date <= refDate)
                    .ToListAsync();

                var totalCredits = await _db.CustomerLedgers
                    .Where(x => x.CustomerId == customer.ID && x.IsActive && x.DebitAmount == 0 && x.Date <= refDate)
                    .SumAsync(x => x.CreditAmount);

                var totalDebits = invoiceEntries.Sum(x => x.DebitAmount);
                var outstanding = totalDebits - totalCredits;

                if (outstanding <= 0) continue;

                decimal current = 0, days30 = 0, days60 = 0, days90 = 0, days90Plus = 0;
                var remainingCredit = totalCredits;

                var orderedInvoices = invoiceEntries.OrderBy(x => x.Date).ToList();

                foreach (var inv in orderedInvoices)
                {
                    var invoiceAmount = inv.DebitAmount;

                    if (remainingCredit > 0)
                    {
                        var creditToApply = Math.Min(remainingCredit, invoiceAmount);
                        invoiceAmount -= creditToApply;
                        remainingCredit -= creditToApply;
                    }

                    if (invoiceAmount <= 0) continue;

                    var daysDiff = (refDate - inv.Date).Days;

                    if (daysDiff <= 0) current += invoiceAmount;
                    else if (daysDiff <= 30) days30 += invoiceAmount;
                    else if (daysDiff <= 60) days60 += invoiceAmount;
                    else if (daysDiff <= 90) days90 += invoiceAmount;
                    else days90Plus += invoiceAmount;
                }

                result.Add(new AgingReportDto
                {
                    CustomerId = customer.ID,
                    CustomerName = customer.Name,
                    Current = current,
                    Days30 = days30,
                    Days60 = days60,
                    Days90 = days90,
                    Days90Plus = days90Plus,
                    Total = current + days30 + days60 + days90 + days90Plus
                });
            }

            return result.OrderByDescending(x => x.Total).ToList();
        }

        public async Task<OutstandingReportDto> GetOutstandingReport()
        {
            var customerOutstandings = await _db.CustomerLedgers
                .Where(x => x.IsActive)
                .GroupBy(x => x.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    TotalDebit = g.Sum(x => x.DebitAmount),
                    TotalCredit = g.Sum(x => x.CreditAmount),
                    LastPaymentDate = g
                        .Where(x => x.CreditAmount > 0)
                        .Max(x => (DateTime?)x.Date)
                })
                .Where(x => x.TotalDebit - x.TotalCredit > 0)
                .ToListAsync();

            var customerIds = customerOutstandings.Select(x => x.CustomerId).ToList();
            var customers = await _db.Customers
                .Where(c => customerIds.Contains(c.ID))
                .Select(c => new { c.ID, c.Name, c.CustomerType })
                .ToDictionaryAsync(c => c.ID);

            var items = customerOutstandings.Select(x => new CustomerOutstandingDto
            {
                CustomerId = x.CustomerId,
                CustomerName = customers.GetValueOrDefault(x.CustomerId)?.Name ?? "Unknown",
                TotalDebit = x.TotalDebit,
                TotalCredit = x.TotalCredit,
                Outstanding = x.TotalDebit - x.TotalCredit,
                LastPaymentDate = x.LastPaymentDate
            })
            .OrderByDescending(x => x.Outstanding)
            .ToList();

            return new OutstandingReportDto
            {
                Customers = items,
                TotalOutstanding = items.Sum(x => x.Outstanding)
            };
        }

        public async Task<CollectionSummaryDto> GetCollectionSummary(DateTime from, DateTime to)
        {
            var loggedInUser = LoggedInUserProvider.CurrentUser;

            var payments = await _db.CustomerLedgers
                .Where(l => l.TransactionType == "Payment" && l.IsActive && l.CompanyCode == loggedInUser.CompanyCode
                    && l.Date >= from && l.Date <= to)
                .ToListAsync();

            var dailyBreakdown = payments
                .GroupBy(p => p.Date.Date)
                .Select(g => new DailyCollectionDto
                {
                    Date = g.Key,
                    Amount = g.Sum(p => p.CreditAmount),
                    TransactionCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            return new CollectionSummaryDto
            {
                FromDate = from,
                ToDate = to,
                TotalCollected = payments.Sum(p => p.CreditAmount),
                CashCollection = payments.Where(p => p.PaymentMode == "Cash").Sum(p => p.CreditAmount),
                ChequeCollection = payments.Where(p => p.PaymentMode == "Cheque").Sum(p => p.CreditAmount),
                NEFTCollection = payments.Where(p => p.PaymentMode == "NEFT" || p.PaymentMode == "RTGS").Sum(p => p.CreditAmount),
                UPICollection = payments.Where(p => p.PaymentMode == "UPI").Sum(p => p.CreditAmount),
                RazorpayCollection = payments.Where(p => p.PaymentMode == "Razorpay").Sum(p => p.CreditAmount),
                TotalTransactions = payments.Count,
                DailyBreakdown = dailyBreakdown
            };
        }

        // ===== CREDIT MANAGEMENT =====

        public async Task<CreditStatusDto> GetCreditStatus(long customerId)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.ID == customerId && c.IsActive);
            if (customer == null)
                throw new InvalidOperationException("Customer not found.");

            var balance = await GetCustomerBalance(customerId);

            var oldestUnpaid = await _db.TaxInvoices
                .Where(i => i.CustomerID == customerId && i.Status != "Paid")
                .OrderBy(i => i.InvoiceDate)
                .Select(i => (DateTime?)i.InvoiceDate)
                .FirstOrDefaultAsync();

            int? oldestUnpaidDays = oldestUnpaid.HasValue
                ? (int)(DateTime.UtcNow - oldestUnpaid.Value).TotalDays
                : null;

            var creditLimit = customer.CreditLimitAmount ?? 0;

            return new CreditStatusDto
            {
                CustomerId = customerId,
                CustomerName = customer.Name,
                CustomerType = customer.CustomerType,
                CreditLimit = creditLimit,
                CreditLimitDays = customer.CreditLimitTime ?? 0,
                TotalOutstanding = balance.Outstanding,
                AvailableCredit = creditLimit - balance.Outstanding,
                IsOverLimit = creditLimit > 0 && balance.Outstanding > creditLimit,
                IsOverdue = customer.CreditLimitTime.HasValue && oldestUnpaidDays.HasValue && oldestUnpaidDays.Value > customer.CreditLimitTime.Value,
                OldestUnpaidDays = oldestUnpaidDays
            };
        }

        public async Task<bool> CheckCreditLimit(long customerId, decimal newChargeAmount)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.ID == customerId && c.IsActive);
            if (customer == null || customer.CreditLimitAmount == null || customer.CreditLimitAmount == 0)
                return true; // No credit limit set — allow

            var balance = await GetCustomerBalance(customerId);
            return (balance.Outstanding + newChargeAmount) <= customer.CreditLimitAmount;
        }

        // ===== HELPERS =====

        private async Task<decimal> GetLastBalance(long customerId)
        {
            var lastEntry = await _db.CustomerLedgers
                .Where(x => x.CustomerId == customerId && x.IsActive)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            return lastEntry?.Balance ?? 0;
        }

        private async Task<string> GenerateReceiptNo()
        {
            var year = DateTime.UtcNow.Year;
            var lastReceipt = await _db.PaymentReceipts
                .Where(r => r.ReceiptNo.StartsWith($"REC/{year}/"))
                .OrderByDescending(r => r.Id)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastReceipt != null)
            {
                var parts = lastReceipt.ReceiptNo.Split('/');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNum))
                    nextNumber = lastNum + 1;
            }

            return $"REC/{year}/{nextNumber:D6}";
        }
    }
}
