using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers.Enums;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LIMSApi.Controllers
{
    [ApiController]
    [Route("api/payments/webhook")]
    [AllowAnonymous]
    public class RazorpayWebhookController : ControllerBase
    {
        private readonly LIMSContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<RazorpayWebhookController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly CaseClosureService _caseClosureService;

        public RazorpayWebhookController(
            LIMSContext db,
            IConfiguration config,
            ILogger<RazorpayWebhookController> logger, IPaymentService paymentService,
            CaseClosureService caseClosureService)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _paymentService = paymentService;
            _caseClosureService = caseClosureService;
        }

        [HttpPost("razorpay")]
        public async Task<IActionResult> HandleRazorpayWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var signature = Request.Headers["X-Razorpay-Signature"].FirstOrDefault();

            if (!VerifySignature(body, signature))
            {
                _logger.LogWarning("Invalid Razorpay webhook signature");
                return Unauthorized();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var payload =
                JsonSerializer.Deserialize<RazorpayWebhookPayload>(body, options);
            if (payload == null)
                return BadRequest();

            await ProcessWebhookAsync(payload);

            return Ok();
        }
        private bool VerifySignature(string body, string? signature)
        {
            if (string.IsNullOrWhiteSpace(signature))
                return false;

            var secret = _config["Razorpay:WebhookSecret"];

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
            var expectedSignature = Convert.ToHexString(hash).ToLower();

            return expectedSignature == signature;
        }

        private async Task ProcessWebhookAsync(RazorpayWebhookPayload payload)
        {
            var payment = payload.Payload.Payment.Entity;

            var order = await _db.PaymentOrders
                .FirstOrDefaultAsync(x => x.RazorpayOrderId == payment.Order_Id);

            if (order == null)
            {
                _logger.LogWarning(
                    "Webhook received for unknown Razorpay order {OrderId}",
                    payment.Order_Id
                );
                return;
            }

            // 🛑 Idempotency
            if (order.Status == PaymentStatus.Paid)
                return;

            if (payload.Event == "payment.captured")
            {
                order.Status = PaymentStatus.Paid;
                order.RazorpayPaymentId = payment.Id;
                order.PaidOn = DateTime.UtcNow;

                // 🔗 Business settlement
                await _paymentService.SettlePaymentAsync(order);

                _logger.LogInformation(
                    "Payment captured via webhook for OrderNo {OrderNo}",
                    order.OrderNo
                );

                // Auto-close case if all conditions met
                if (order.InwardID.HasValue)
                {
                    try
                    {
                        var (canClose, reason) = await _caseClosureService.CanCloseCaseAsync(order.InwardID.Value);
                        if (canClose)
                        {
                            await _caseClosureService.CloseCaseAsync(order.InwardID.Value, 0);
                            _logger.LogInformation("Case {InwardId} auto-closed after payment", order.InwardID.Value);
                        }
                        else
                        {
                            _logger.LogInformation("Case {InwardId} cannot auto-close: {Reason}", order.InwardID.Value, reason);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during auto-close for InwardId {InwardId}", order.InwardID.Value);
                    }
                }
            }
            else if (payload.Event == "payment.failed")
            {
                order.Status = PaymentStatus.Failed;

                _logger.LogInformation(
                    "Payment failed via webhook for OrderNo {OrderNo}",
                    order.OrderNo
                );
            }

            await _db.SaveChangesAsync();
        }


    }
}
