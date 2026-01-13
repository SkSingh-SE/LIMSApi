using LIMSApi.Dtos;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // -------------------------------
        // ZONE 1: ACCOUNT DASHBOARD
        // -------------------------------
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _accountService.GetDashboardAsync();
            return Ok(result);
        }
        // -----------------------------------
        // ZONE 2: CASE ACCOUNT LIST
        // -----------------------------------
        [HttpPost("cases")]
        public async Task<IActionResult> GetCaseAccountList(PageFilter filter)
        {
            var result = await _accountService.GetCaseAccountListAsync(filter);
            return Ok(result);
        }
        // -----------------------------------
        // ZONE 3: CASE ACCOUNT SUMMARY
        // -----------------------------------
        [HttpGet("cases/{inwardId}/summary")]
        public async Task<IActionResult> GetCaseAccountSummary(long inwardId)
        {
            var result = await _accountService.GetCaseAccountSummaryAsync(inwardId);
            return Ok(result);
        }

        // -----------------------------------
        // ZONE 3: CASE PAYMENT LIST
        // -----------------------------------
        [HttpPost("cases/{inwardId}/payments")]
        public async Task<IActionResult> GetCasePaymentList(
            long inwardId,
            [FromBody] PageFilter filter)
        {
            var result = await _accountService.GetCasePaymentListAsync(inwardId, filter);
            return Ok(result);
        }
        // -----------------------------------
        // ZONE 4: CREATE PRICE SNAPSHOT
        // -----------------------------------
        [HttpPost("cases/{inwardId}/create-snapshot")]
        public async Task<IActionResult> CreatePriceSnapshot(long inwardId)
        {
            await _accountService.CreatePriceSnapshotAsync(inwardId);
            return Ok(new { message = "Price snapshot created successfully" });
        }

        // -----------------------------------
        // ZONE 4: GENERATE INVOICE
        // -----------------------------------
        [HttpPost("cases/{inwardId}/generate-invoice")]
        public async Task<IActionResult> GenerateInvoice(long inwardId)
        {
            var invoiceId = await _accountService.GenerateInvoiceAsync(inwardId);
            return Ok(new { invoiceId });
        }

        // -----------------------------------
        // ZONE 4: SEND INVOICE
        // -----------------------------------
        [HttpPost("invoices/{invoiceId}/send")]
        public async Task<IActionResult> SendInvoice(long invoiceId, bool email = true, bool whatsapp = false)
        {
            await _accountService.SendInvoiceAsync(invoiceId, email, whatsapp);
            return Ok(new { message = "Invoice sent successfully" });
        }
        // -----------------------------------
        // GENERATE PROFORMA INVOICE
        // -----------------------------------
        [HttpPost("cases/{inwardId}/generate-proforma-invoice")]
        public async Task<IActionResult> GenerateProformaInvoice(long inwardId)
        {
            var invoiceId = await _accountService.GenerateProformaInvoiceAsync(inwardId);
            return Ok(new { invoiceId });
        }
    }
}
