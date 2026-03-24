using LIMSApi.Dtos;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using System.Text;
using System;
using LIMSApi.Data;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using System.Security.Cryptography;
using LIMSApi.Helpers;
using LIMSApi.Services.Interface;

namespace LIMSApi.ServiceWORepo
{
    public class PaymentService : IPaymentService
    {
        private readonly LIMSContext _db;
        private readonly RazorpayClient _razorpay;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;
        private readonly WhatsAppService _whatsAppService;
        private readonly INotificationService _notificationService;
        private readonly ISampleStatusService _sampleStatusService;
        private readonly BillingSettlementService _billingSettlement;
        private readonly LoggedInUserDTO _loggedInUser;
        private readonly TemplateService _templateService;

        public PaymentService(LIMSContext db, IConfiguration config, EmailService emailService, WhatsAppService whatsAppService,
        INotificationService notificationService, ISampleStatusService sampleStatusService, TemplateService templateService,
        BillingSettlementService billingSettlement)
        {
            _db = db;
            _config = config;
            _emailService = emailService;
            _whatsAppService = whatsAppService;
            _notificationService = notificationService;
            _sampleStatusService = sampleStatusService;
            _templateService = templateService;
            _billingSettlement = billingSettlement;
            _loggedInUser = LoggedInUserProvider.CurrentUser;

            _razorpay = new RazorpayClient(
                _config["Razorpay:Key"],
                _config["Razorpay:Secret"]
            );
        }

        public async Task<object> ProcessPaymentAsync(PaymentRequestDto dto)
        {
            // =============================
            // VERIFY PAYMENT
            // =============================
            if (!string.IsNullOrEmpty(dto.RazorpayPaymentId))
            {
                var order = await _db.PaymentOrders.FirstOrDefaultAsync(x =>
                    x.RazorpayOrderId == dto.RazorpayOrderId &&
                    x.PaymentToken == dto.PaymentToken);

                if (order == null)
                    throw new Exception("Invalid payment session");

                VerifySignature(dto, order);

                order.Status = PaymentStatus.Paid;
                order.RazorpayPaymentId = dto.RazorpayPaymentId;
                order.RazorpaySignature = dto.RazorpaySignature;
                order.PaidOn = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                await SettlePaymentAsync(order);

                return new
                {
                    order.ID,
                    success = true,
                    message = "Payment successful",
                    order.OrderNo,
                    order.PaymentType
                };
            }

            // =============================
            // CREATE PAYMENT
            // =============================
            ValidatePaymentTarget(dto);

            var internalOrderNo = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}";

            // Create Razorpay order
            var razorpayOrder = CreateRazorpayOrder(dto, internalOrderNo);

            var paymentOrder = new PaymentOrder
            {
                OrderNo = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}",
                PaymentType = dto.PaymentType,
                TaxInvoiceID = dto.TaxInvoiceID,
                InwardID = dto.InwardID,
                CaseNo = dto.CaseNo,
                SampleID = dto.SampleID,
                ReportID = dto.ReportID,

                CustomerId = dto.CustomerID,
                Amount = dto.Amount,

                RazorpayOrderId = razorpayOrder["id"].ToString(),
                PaymentToken = Guid.NewGuid().ToString("N"),
                TokenExpiry = DateTime.UtcNow.AddHours(48),
                CreatedOn = DateTime.UtcNow,
                Status = PaymentStatus.Pending
            };

            _db.PaymentOrders.Add(paymentOrder);
            await _db.SaveChangesAsync();

            return new
            {
                success = true,
                paymentToken = paymentOrder.PaymentToken,
                razorpayOrderId = paymentOrder.RazorpayOrderId,
                amount = paymentOrder.Amount,
                key = _config["Razorpay:Key"]
            };
        }



        private Order CreateRazorpayOrder(PaymentRequestDto dto, string internalOrderNo)
        {
            var notes = new Dictionary<string, string>
                            {
                                { "payment_type", dto.PaymentType.ToString() },
                                { "case_no", dto.CaseNo ?? string.Empty },
                                { "inward_id", dto.InwardID?.ToString() ?? string.Empty },
                                { "sample_id", dto.SampleID?.ToString() ?? string.Empty },
                                { "report_id", dto.ReportID?.ToString() ?? string.Empty }
                            };

            var options = new Dictionary<string, object>
                            {
                                { "amount", Convert.ToInt32(dto.Amount * 100) }, // Razorpay expects paise
                                { "currency", "INR" },
                                { "receipt", internalOrderNo },                  // 👈 VERY IMPORTANT
                                { "notes", notes }
                            };

            return _razorpay.Order.Create(options);
        }


        private void VerifySignature(PaymentRequestDto dto, PaymentOrder order)
        {
            string payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";
            string secret = _config["Razorpay:Secret"];

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var generatedSignature = Convert.ToHexString(hash).ToLower();

            if (generatedSignature != dto.RazorpaySignature)
                throw new Exception("Payment verification failed");
        }

        public async Task<PaymentTokenValidationResponse> ValidateTokenAsync(string token)
        {
            var payment = await _db.PaymentOrders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PaymentToken == token);

            if (payment == null)
            {
                return new PaymentTokenValidationResponse
                {
                    IsValid = false,
                    Message = "Invalid payment link"
                };
            }

            if (payment.Status == PaymentStatus.Paid)
            {
                return new PaymentTokenValidationResponse
                {
                    IsValid = false,
                    Message = "Payment already completed"
                };
            }

            if (payment.TokenExpiry < DateTime.UtcNow)
            {
                return new PaymentTokenValidationResponse
                {
                    IsValid = false,
                    Message = "Payment link has expired"
                };
            }

            return new PaymentTokenValidationResponse
            {
                IsValid = true,
                OrderNo = payment.OrderNo,
                PaymentType = payment.PaymentType,
                Amount = payment.Amount,
                Currency = payment.Currency,
                RazorpayOrderId = payment.RazorpayOrderId,
                RazorpayKey = _config["Razorpay:Key"],
                CustomerName = await _db.Customers.Where(x => x.ID == payment.CustomerId).Select(y => y.Name).FirstOrDefaultAsync()
            };
        }

        public string GeneratePaymentLink(string token)
        {
            return $"{_config["PublicBaseUrl"]}/payment/{token}";
        }

        /// <summary>
        /// Sends payment link via Email and WhatsApp. Only Accounts role can send payment links.
        /// </summary>
        public async Task SendPaymentLinkAsync(long paymentOrderId)
        {
            // Role check: Only Accounts role can send payment links
            var user = LoggedInUserProvider.CurrentUser;
            if (user == null || (user.Role != "Accounts" && !user.Role.Contains("Admin", StringComparison.OrdinalIgnoreCase)))
            {
                throw new UnauthorizedAccessException("Only Accounts role can send payment links.");
            }

            var order = await _db.PaymentOrders
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.ID == paymentOrderId);

            if (order == null || order.Status != PaymentStatus.Pending)
                return;

            if (string.IsNullOrWhiteSpace(order.PaymentToken))
                return;

            string? email = null;
            string? mobileNo = null;
            string referenceText = string.Empty;
            string customerName = order.Customer?.Name ?? "Customer";

            // ---------------------------------
            // RESOLVE CONTACT & REFERENCE
            // ---------------------------------

            var paymentLink = GeneratePaymentLink(order.PaymentToken);

            // ---------------------------------
            // TEMPLATE MODEL (COMMON)
            // ---------------------------------
            var templateModel = new
            {
                CustomerName = customerName,
                Amount = order.Amount,
                ReferenceText = referenceText,
                PaymentLink = paymentLink
            };
            var emailBody = string.Empty;
            var whatsAppMsg = string.Empty;

            if (order.PaymentType == PaymentType.PIInvoice || order.PaymentType == PaymentType.Invoice)
            {
                if (order.InwardID.HasValue)
                {
                    var inward = await _db.SampleInwards
                        .Include(i => i.Contacts)
                        .FirstOrDefaultAsync(i => i.ID == order.InwardID.Value);

                    var contact = inward?.Contacts?.FirstOrDefault(x => x.Selected);
                    email = contact?.EmailId;
                    mobileNo = contact?.MobileNo;
                    referenceText = $"Case {order.CaseNo}";
                    customerName = inward?.Customer?.Name ?? customerName;
                }
                else if (order.SampleID.HasValue)
                {
                    var sample = await _db.SampleDetails
                        .Include(s => s.SampleInward)
                            .ThenInclude(i => i.Contacts)
                        .FirstOrDefaultAsync(s => s.ID == order.SampleID.Value);

                    var contact = sample?.SampleInward?.Contacts?.FirstOrDefault(x => x.Selected);
                    email = contact?.EmailId;
                    mobileNo = contact?.MobileNo;
                    referenceText = $"Sample {sample?.SampleNo}";
                    customerName = sample?.SampleInward?.Customer?.Name ?? customerName;
                }
                if (order.PaymentType == PaymentType.PIInvoice)
                {
                    emailBody = await _templateService.GetTemplateAsync(MessageTemplateKey.PAYMENT_LINK, NotificationType.Email, templateModel);
                    whatsAppMsg = await _templateService.GetTemplateAsync(MessageTemplateKey.PAYMENT_LINK, NotificationType.WhatsApp, templateModel);
                }
                else
                {
                    emailBody = await _templateService.GetTemplateAsync(MessageTemplateKey.PAYMENT_LINK, NotificationType.Email, templateModel);
                    whatsAppMsg = await _templateService.GetTemplateAsync(MessageTemplateKey.PAYMENT_LINK, NotificationType.WhatsApp, templateModel);
                }
            }
            else if (order.PaymentType == PaymentType.AmendmentInvoice)
            {
                var report = await _db.Reports
                    .Include(r => r.ReportHeader)
                        .ThenInclude(h => h.Sample)
                            .ThenInclude(s => s.SampleInward)
                                .ThenInclude(i => i.Contacts)
                    .FirstOrDefaultAsync(r => r.ID == order.ReportID);

                var contact = report?.ReportHeader?.Sample?.SampleInward?
                    .Contacts?.FirstOrDefault(x => x.Selected);

                email = contact?.EmailId;
                mobileNo = contact?.MobileNo;
                referenceText = $"Report {report?.ReportNo}";
                customerName = report?.ReportHeader?.CustomerName ?? customerName;

                emailBody = await _templateService.GetTemplateAsync(MessageTemplateKey.PAYMENT_LINK, NotificationType.Email, templateModel);
                whatsAppMsg = await _templateService.GetTemplateAsync(MessageTemplateKey.PAYMENT_LINK, NotificationType.WhatsApp, templateModel);
            }

            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(mobileNo))
                return;

            // ---------------------------------
            // EMAIL
            // ---------------------------------
            if (!string.IsNullOrWhiteSpace(email))
            {
                var subject = $"Payment Pending - {referenceText}";
               
                await _emailService.SendEmailAsync(email, subject, emailBody);
                order.EmailSent = true;
            }

            // ---------------------------------
            // WHATSAPP
            // ---------------------------------
            if (!string.IsNullOrWhiteSpace(mobileNo))
            {
                await _whatsAppService.SendWhatsAppMessageAsync(mobileNo, whatsAppMsg);
                order.WhatsAppSent = true;
            }

            order.LinkSentOn = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // ---------------------------------
            // INTERNAL NOTIFICATION
            // ---------------------------------
            await _notificationService.CreateNotificationAsync(new Notification
            {
                Title = "Payment Link Sent",
                Message = $"Payment link sent for {referenceText}",
                EntityType = order.PaymentType == PaymentType.AmendmentInvoice
                    ? "Report"
                    : "SampleInward",
                EntityID = order.PaymentType == PaymentType.AmendmentInvoice
                    ? order.ReportID!.Value
                    : order.InwardID!.Value
            });
        }


        public async Task SettlePaymentAsync(PaymentOrder order)
        {
            if (order.Status != PaymentStatus.Paid)
                return;

            // -----------------------------
            // 🔗 NORMALIZE INWARD ID
            // -----------------------------
            if (!order.InwardID.HasValue)
            {
                if (order.SampleID.HasValue)
                {
                    order.InwardID = await _db.SampleDetails
                        .Where(x => x.ID == order.SampleID.Value)
                        .Select(x => x.InwardID)
                        .FirstAsync();
                }
                else if (order.ReportID.HasValue)
                {
                    order.InwardID = await _db.Reports
                        .Where(x => x.ID == order.ReportID.Value)
                        .Include(x => x.ReportHeader).ThenInclude(x => x.Sample)
                        .Select(x => x.ReportHeader.Sample.InwardID)
                        .FirstAsync();
                }
                else
                {
                    throw new Exception("Unable to resolve InwardID for payment settlement");
                }
            }

            // -----------------------------
            // 🔐 BUSINESS SETTLEMENT
            // -----------------------------
            switch (order.PaymentType)
            {
                case PaymentType.PIInvoice:
                    var inwardPI = await _db.SampleInwards.FindAsync(order.InwardID);
                    inwardPI!.PIReceived = true;
                    // BillingStatus remains PI_GENERATED until final invoice
                    break;

                case PaymentType.Invoice:
                    if (order.SampleID.HasValue)
                    {
                        var sample = await _db.SampleDetails.FindAsync(order.SampleID);
                        sample!.IsReportUnlocked = true;
                    }
                    else
                    {
                        var inwardFinal = await _db.SampleInwards.FindAsync(order.InwardID);
                        inwardFinal!.IsReportUnlocked = true;
                    }

                    // Unified billing settlement — single source of truth
                    if (order.InwardID.HasValue)
                    {
                        await _billingSettlement.RecalculateBillingStatusAsync(
                            order.InwardID.Value, _loggedInUser.EmployeeID);
                    }
                    break;

                case PaymentType.AmendmentInvoice:
                    var report = await _db.Reports.FindAsync(order.ReportID);
                    report!.IsAmendmentAllowed = true;

                    var amendment = await _db.CustomerAmendments
                        .FirstOrDefaultAsync(x => x.PaymentOrderID == order.ID);

                    if (amendment != null)
                        amendment.Status = "Approved";
                    break;

                case PaymentType.Advance:
                    // Advance payment recording against PI
                    if (!order.InwardID.HasValue)
                        throw new Exception("Advance payment must be against Inward");

                    var inwardAdvance = await _db.SampleInwards
                        .Include(i => i.SampleDetails)
                        .FirstOrDefaultAsync(i => i.ID == order.InwardID.Value);

                    if (inwardAdvance == null)
                        throw new Exception("Inward not found for advance payment");

                    // Validate that PI exists for this inward
                    var piExists = await _db.ProformaInvoiceHeader
                        .AnyAsync(pi => pi.InwardID == inwardAdvance.ID && pi.IsActive);

                    if (!piExists)
                        throw new InvalidOperationException("Cannot record advance payment. Proforma Invoice must be generated first.");

                    // Record advance payment amount (accumulate if multiple advance payments)
                    inwardAdvance.AdvancePayment += order.Amount;

                    // Update status of all samples for this inward to ADVANCE_PAYMENT_COMPLETED
                    var samples = inwardAdvance.SampleDetails.Where(s => s.IsActive).ToList();
                    foreach (var sample in samples)
                    {
                        await _sampleStatusService.ForceAutoStatusAsync(
                            sample.ID,
                            SampleStatus.ADVANCE_PAYMENT_COMPLETED,
                            _loggedInUser.EmployeeID);
                    }

                    // Note: Advance does NOT close billing (BillingStatus remains unchanged)
                    // Note: Advance does NOT unlock report (IsReportUnlocked remains false)
                    
                    break;
            }

            await _db.SaveChangesAsync();
        }


        private void ValidatePaymentTarget(PaymentRequestDto dto)
        {
            if (dto.PaymentType == PaymentType.PIInvoice && dto.InwardID == null)
                throw new Exception("PI payment must be against Inward");

            if (dto.PaymentType == PaymentType.Invoice &&
                dto.InwardID == null && dto.SampleID == null)
                throw new Exception("Invoice payment must be against Inward or Sample");

            if (dto.PaymentType == PaymentType.AmendmentInvoice && dto.ReportID == null)
                throw new Exception("Amendment payment must be against Report");
        }


    }

}
