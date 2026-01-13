using LIMSApi.Data;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.ServiceWORepo
{
    /// <summary>
    /// DispatchService: Handles report dispatch with payment/credit validation
    /// </summary>
    public class DispatchService
    {
        private readonly LIMSContext _db;
        private readonly ILogger<DispatchService> _logger;
        private readonly EmailService _emailService;
        private readonly WhatsAppService _whatsAppService;
        private readonly TemplateService _templateService;

        public DispatchService(LIMSContext db, ILogger<DispatchService> logger, EmailService emailService, WhatsAppService whatsAppService, TemplateService templateService)
        {
            _db = db;
            _logger = logger;
            _emailService = emailService;
            _whatsAppService = whatsAppService;
            _templateService = templateService;
        }

        /// <summary>
        /// Validates if report can be dispatched based on payment/credit rules
        /// </summary>
        public async Task<(bool CanDispatch, string Reason)> CanDispatchReportAsync(long sampleId)
        {
            var sample = await _db.SampleDetails
                .Include(s => s.SampleInward)
                    .ThenInclude(i => i.Customer)
                .FirstOrDefaultAsync(s => s.ID == sampleId);

            if (sample == null)
                return (false, "Sample not found");

            var inward = sample.SampleInward;
            if (inward == null)
                return (false, "Inward case not found");

            var customer = inward.Customer;
            if (customer == null)
                return (false, "Customer not found");

            // Check if report is already unlocked
            if (sample.IsReportUnlocked || inward.IsReportUnlocked)
                return (true, string.Empty);

            // Check if report is approved (sample status should be FINAL_REPORT_APPROVED or REPORT_DISPATCHED)
            if (sample.SampleStatus != SampleStatus.FINAL_REPORT_APPROVED.ToString() && 
                sample.SampleStatus != SampleStatus.REPORT_DISPATCHED.ToString())
            {
                return (false, "Report must be approved before dispatch");
            }

            // Dispatch rules based on customer type
            var customerType = customer.CustomerType ?? "";
            switch (customerType.ToLower())
            {
                case "walk-in":
                    // Walk-in: Payment mandatory
                    var hasPaidInvoice = await _db.PaymentOrders
                        .AnyAsync(p => (p.InwardID == inward.ID || p.SampleID == sampleId) &&
                                      p.PaymentType == PaymentType.Invoice &&
                                      p.Status == PaymentStatus.Paid);

                    if (!hasPaidInvoice)
                        return (false, "Payment required for walk-in customers before report dispatch");

                    return (true, string.Empty);

                case "credit":
                    // Credit: Invoice sufficient (no payment required)
                    var hasInvoice = await _db.TaxInvoices
                        .AnyAsync(i => i.InwardID == inward.ID);

                    if (!hasInvoice)
                        return (false, "Invoice must be generated before dispatch for credit customers");

                    // Check credit limit if configured
                    if (customer.CreditLimitAmount.HasValue)
                    {
                        var totalPending = await _db.TaxInvoices
                            .Where(i => i.CustomerID == customer.ID && 
                                       !_db.PaymentOrders.Any(p => p.TaxInvoiceID == i.ID && p.Status == PaymentStatus.Paid))
                            .SumAsync(i => (decimal?)i.GrandTotal) ?? 0;

                        if (totalPending + (inward.TotalTestCharges) > customer.CreditLimitAmount.Value)
                            return (false, $"Credit limit exceeded. Pending: {totalPending}, Limit: {customer.CreditLimitAmount.Value}");
                    }

                    return (true, string.Empty);

                default:
                    // Default: Payment required
                    var hasPayment = await _db.PaymentOrders
                        .AnyAsync(p => (p.InwardID == inward.ID || p.SampleID == sampleId) &&
                                      p.Status == PaymentStatus.Paid);

                    if (!hasPayment)
                        return (false, "Payment required before report dispatch");

                    return (true, string.Empty);
            }
        }

        /// <summary>
        /// Dispatches report: generates final PDF, sends notifications, updates status
        /// </summary>
        public async Task DispatchReportAsync(long sampleId, long employeeId)
        {
            var (canDispatch, reason) = await CanDispatchReportAsync(sampleId);
            if (!canDispatch)
                throw new InvalidOperationException($"Cannot dispatch report: {reason}");

            var sample = await _db.SampleDetails
                .Include(s => s.SampleInward)
                    .ThenInclude(i => i.Customer)
                .Include(s => s.SampleInward)
                    .ThenInclude(i => i.Contacts)
                .FirstOrDefaultAsync(s => s.ID == sampleId);

            if (sample == null)
                throw new Exception("Sample not found");

            var inward = sample.SampleInward!;
            var customer = inward.Customer!;

            // Get report header
            var reportHeader = await _db.ReportHeaders
                .Include(r => r.Reports.Where(rep => rep.Status == "Final"))
                .FirstOrDefaultAsync(r => r.SampleID == sampleId && r.IsActive);

            if (reportHeader == null)
                throw new Exception("Report not found or not finalized");

            var finalReport = reportHeader.Reports
                .OrderByDescending(r => r.Version)
                .FirstOrDefault();

            if (finalReport == null)
                throw new Exception("Final report not found");

            // Generate PDF if not exists
            if (string.IsNullOrEmpty(finalReport.PdfPath))
            {
                // TODO: Call report PDF generation service
                _logger.LogWarning("Report PDF not generated. PDF generation should be called separately.");
            }

            // Update status
            sample.IsReportUnlocked = true;
            inward.IsReportUnlocked = true;

            // Update sample status to REPORT_DISPATCHED
            sample.SampleStatus = SampleStatus.REPORT_DISPATCHED.ToString();
            sample.ModifiedBy = employeeId;
            sample.ModifiedOn = DateTime.UtcNow;

            // Update inward status if all samples are dispatched
            var allSamplesDispatched = await _db.SampleDetails
                .Where(s => s.InwardID == inward.ID)
                .AllAsync(s => s.SampleStatus == SampleStatus.REPORT_DISPATCHED.ToString());
            
            if (allSamplesDispatched)
            {
                // Use existing InwardStatus enum - mark as COMPLETED
                inward.InwardStatus = InwardStatus.COMPLETED.ToString();
                inward.ModifiedBy = employeeId;
                inward.ModifiedOn = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            // Send notifications
            var contact = inward.Contacts?.FirstOrDefault();
            if (contact != null && customer != null)
            {
                // Email: Report PDF
                if (!string.IsNullOrEmpty(contact.EmailId) && !string.IsNullOrEmpty(finalReport.PdfPath))
                {
                    var emailBody = await _templateService.GetTemplateAsync(MessageTemplateKey.REPORT_DISPATCHED, NotificationType.Email, new
                    {
                        CustomerName = customer.Name ?? "Customer",
                        SampleCode = sample.SampleNo,
                        CaseNo = inward.CaseNo
                    });

                    await _emailService.SendEmailWithAttachment(
                        contact.EmailId,
                        $"Test Report - {sample.SampleNo}",
                        emailBody,
                        finalReport.PdfPath,
                        $"Report_{sample.SampleNo}.pdf"
                    );
                }

                // WhatsApp: Report download link only
                if (!string.IsNullOrEmpty(contact.MobileNo))
                {
                    // TODO: Generate secure download link
                    var downloadLink = $"/api/reports/{finalReport.ID}/download";
                    var whatsAppBody = await _templateService.GetTemplateAsync(MessageTemplateKey.REPORT_READY_FOR_DOWNLOAD, NotificationType.WhatsApp, new
                    {
                        CustomerName = customer.Name ?? "Customer",
                        SampleCode = sample.SampleNo,
                        DownloadLink = downloadLink
                    });

                    await _whatsAppService.SendWhatsAppMessageAsync(contact.MobileNo, whatsAppBody);
                }
            }

            _logger.LogInformation("Report dispatched for Sample {SampleNo}", sample.SampleNo);
        }
    }
}

