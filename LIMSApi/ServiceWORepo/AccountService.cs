using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
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

        public AccountService(LIMSContext db, EmailService emailService, WhatsAppService whatsAppService, InvoicePdfService invoicePdfService)
        {
            _db = db;
            _emailService = emailService;
            _whatsAppService = whatsAppService;
            _invoicePdfService = invoicePdfService;
        }

        public async Task<AccountDashboardDto> GetDashboardAsync()
        {
            var piPending = await _db.SampleInwards
                .Where(x => !x.PIReceived && x.AdvancePIRequired)
                .CountAsync();

            var invoicePending = await _db.SampleInwards
                .Where(x =>!x.IsInvoiceGenerated)
                .CountAsync();

            var paymentPending = await _db.PaymentOrders
                .Where(x => x.Status == PaymentStatus.Pending)
                .Select(x => x.InwardID)
                .Distinct()
                .CountAsync();

            var fullySettled = await _db.SampleInwards
                .Where(x =>
                    x.IsInvoiceGenerated &&
                    !_db.PaymentOrders.Any(p =>
                        p.InwardID == x.ID &&
                        p.Status != PaymentStatus.Paid
                    ))
                .CountAsync();

            return new AccountDashboardDto
            {
                PiPendingCount = piPending,
                InvoicePendingCount = invoicePending,
                PaymentPendingCount = paymentPending,
                FullySettledCount = fullySettled
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
                     i.CaseNo,
                     CustomerName = c.Name,
                     CustomerType = c.CustomerType, // Walk-in / Credit

                     PIStatus = i.AdvancePIRequired ? i.PIReceived ? "Completed" : "Pending" : "Completed",
                     InvoiceStatus = i.IsInvoiceGenerated ? "Completed" : "Pending",

                     PaymentStatus =
                         _db.PaymentOrders.Any(p => p.InwardID == i.ID && p.Status == PaymentStatus.Pending)
                             ? "Pending"
                             : _db.PaymentOrders.Any(p => p.InwardID == i.ID && p.Status == PaymentStatus.Failed)
                                 ? "Failed"
                                 : "Paid",

                     Action =
                         _db.PaymentOrders.Any(p => p.InwardID == i.ID && p.Status != PaymentStatus.Paid)
                             ? "Open"
                             : "View"
                 })
                .AsQueryable()
                .ApplyFilters(filter.Filter);

            // ---------------- SEARCH ----------------
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();

                query = query.Where(x =>
                    x.CaseNo.ToLower().Contains(search) ||
                    x.CustomerName.ToLower().Contains(search) ||
                    x.CustomerType.ToLower().Contains(search)
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
                .FirstOrDefaultAsync(x => x.ID == inwardId);

            if (inward == null)
                throw new Exception("Case not found");

            var hasPendingPayment = await _db.PaymentOrders
                .AnyAsync(p => p.InwardID == inwardId && p.Status != PaymentStatus.Paid);

            return new CaseAccountSummaryDto
            {
                InwardID = inward.ID,
                CaseNo = inward.CaseNo,
                CustomerName = inward.Customer!.Name,
                CustomerType = inward.Customer.CustomerType,

                PIStatus = inward.PIReceived ? "Completed" : "Pending",
                InvoiceStatus = inward.IsInvoiceGenerated ? "Completed" : "Pending",
                HasPendingPayment = hasPendingPayment
            };
        }

        public async Task<PagedResponse<object>> GetCasePaymentListAsync(long inwardId, PageFilter filter)
        {
            var query =
                (from p in _db.PaymentOrders
                 where p.InwardID == inwardId

                 select new
                 {
                     p.ID,
                     p.CreatedOn,
                     PaymentType = p.PaymentType.ToString(),

                     Against =
                         p.SampleID != null ? "Sample" :
                         p.ReportID != null ? "Report" : "Case",

                     Reference =
                         p.SampleID != null
                             ? _db.SampleDetails
                                 .Where(s => s.ID == p.SampleID)
                                 .Select(s => s.SampleNo)
                                 .FirstOrDefault()
                             : p.ReportID != null
                                 ? _db.Reports
                                     .Where(r => r.ID == p.ReportID)
                                     .Select(r => r.ReportNo)
                                     .FirstOrDefault()
                                 : p.CaseNo,

                     p.Amount,
                     Status = p.Status.ToString(),

                     Action =
                         p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Failed
                             ? "SendLink"
                             : "-"
                 })
                .AsQueryable()
                .ApplyFilters(filter.Filter);

            // ------------- SEARCH -------------
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();

                query = query.Where(x =>
                    x.PaymentType.ToLower().Contains(search) ||
                    x.Reference.ToLower().Contains(search) ||
                    x.Status.ToLower().Contains(search)
                );
            }

            // ------------- SORT -------------
            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                query = query.OrderBy(
                    $"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}"
                );
            }

            // ------------- PAGINATION -------------
            int totalRecords = await query.CountAsync();

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

        public async Task<long> GenerateInvoiceAsync(long inwardId)
        {
            var inward = await _db.SampleInwards
                .Include(x => x.Customer)
                .FirstAsync(x => x.ID == inwardId);

            if (inward.IsInvoiceGenerated)
                throw new Exception("Invoice already generated");

            // 🔹 Calculate charges (already available via challans/tests)
            var subTotal = inward.TotalTestCharges;
            var cgst = subTotal * 0.09m;
            var sgst = subTotal * 0.09m;

            var invoice = new TaxInvoice
            {
                InvoiceNo = $"TI-{DateTime.UtcNow:yyyyMMddHHmmss}",
                InvoiceDate = DateTime.UtcNow,
                InwardID = inward.ID,
                CustomerID = inward.CustomerID,
                SubTotal = subTotal,
                CGST = cgst,
                SGST = sgst,
                IGST = 0,
                GrandTotal = subTotal + cgst + sgst
            };

            _db.TaxInvoices.Add(invoice);
            inward.IsInvoiceGenerated = true;

            await _db.SaveChangesAsync();
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
                var body = EmailTemplateBuilder.Build("FINAL_INVOICE_POST_TESTING", modelBody);
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
                var body = WhatsAppTemplateBuilder.Build("FINAL_INVOICE_POST_TESTING", modelBody);
                await _whatsAppService.SendWhatsAppMessageAsync(
                    invoice.Inward.Contacts.First().MobileNo,
                    body
                );
            }
        }

        private TaxInvoicePdfModelDto MapToPdfModel(TaxInvoice invoice)
        {
            return new TaxInvoicePdfModelDto
            {
                InvoiceNo = invoice.InvoiceNo,
                InvoiceDate = invoice.InvoiceDate,
                CustomerName = invoice.Customer!.Name,
                CustomerAddress = invoice.Customer.Address,
                CustomerGst = invoice.Customer.GSTNo,
                SubTotal = invoice.SubTotal,
                CGST = invoice.CGST,
                SGST = invoice.SGST,
                IGST = invoice.IGST,
                GrandTotal = invoice.GrandTotal
            };
        }
    }

}
