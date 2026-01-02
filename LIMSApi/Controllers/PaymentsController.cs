using LIMSApi.Dtos;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // ---------------- PROCESS PAYMENT ----------------
        [HttpPost("process")]
        public async Task<IActionResult> Process(PaymentRequestDto dto)
        {
            var result = await _paymentService.ProcessPaymentAsync(dto);
            return Ok(result);
        }

        // ---------------- VALIDATE TOKEN ----------------
        [HttpGet("validate/{token}")]
        public async Task<IActionResult> ValidateToken(string token)
        {
            var result = await _paymentService.ValidateTokenAsync(token);

            if (!result.IsValid)
                return BadRequest(result);

            return Ok(result);
        }

        // ---------------- SEND PAYMENT LINK ----------------
        // -----------------------------------
        // ZONE 4: SEND / RESEND PAYMENT LINK (Refrence Account Controller)
        // -----------------------------------
        [HttpPost("send-link/{paymentOrderId}")]
        public async Task<IActionResult> SendPaymentLink(long paymentOrderId)
        {
            await _paymentService.SendPaymentLinkAsync(paymentOrderId);
            return Ok(new { success = true, message = "Payment link sent successfully" });
        }
    }
}
