using LIMSApi.Dtos;
using LIMSApi.Middleware;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
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
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPaymentGatingService _paymentGatingService;
        private readonly CaseClosureService _caseClosureService;

        public AccountController(IAccountService accountService, IPriceCalculationService priceCalculationService, IPaymentGatingService paymentGatingService, CaseClosureService caseClosureService)
        {
            _accountService = accountService;
            _priceCalculationService = priceCalculationService;
            _paymentGatingService = paymentGatingService;
            _caseClosureService = caseClosureService;
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
        [RequirePermission("INVOICE_GENERATE")]
        public async Task<IActionResult> GenerateInvoice(long inwardId)
        {
            var invoiceId = await _accountService.GenerateInvoiceAsync(inwardId);
            return Ok(new { invoiceId });
        }

        // -----------------------------------
        // ZONE 4: INVOICE DETAILS & SEND
        // -----------------------------------
        [HttpGet("invoices/{invoiceId}")]
        public async Task<IActionResult> GetInvoiceDetails(long invoiceId)
        {
            var result = await _accountService.GetInvoiceDetailsAsync(invoiceId);
            return Ok(result);
        }

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
        [RequirePermission("INVOICE_GENERATE")]
        public async Task<IActionResult> GenerateProformaInvoice(long inwardId)
        {
            var invoiceId = await _accountService.GenerateProformaInvoiceAsync(inwardId);
            return Ok(new { invoiceId });
        }

        // -----------------------------------
        // VALIDATE PRICING (dry run)
        // -----------------------------------
        [HttpGet("cases/{inwardId}/validate-pricing")]
        public async Task<IActionResult> ValidatePricing(long inwardId)
        {
            var result = await _priceCalculationService.ValidatePricingAsync(inwardId);
            return Ok(result);
        }

        // -----------------------------------
        // CALCULATE PRICING
        // -----------------------------------
        [HttpPost("cases/{inwardId}/calculate-pricing")]
        public async Task<IActionResult> CalculatePricing(long inwardId)
        {
            var result = await _priceCalculationService.CalculateAndCreateChargeEventsAsync(inwardId);
            return Ok(result);
        }

        // -----------------------------------
        // PAYMENT GATE CHECK (Gap #39)
        // Checks if testing is blocked for walk-in customers pending payment
        // -----------------------------------
        [HttpGet("payment-gate/{sampleInwardId}")]
        public async Task<IActionResult> CheckPaymentGate(long sampleInwardId)
        {
            var result = await _paymentGatingService.CheckPaymentGateAsync(sampleInwardId);
            return Ok(result);
        }

        // -----------------------------------
        // CREDIT LIMIT CHECK (Gap #40)
        // Checks customer outstanding vs credit limit at inward time
        // -----------------------------------
        [HttpGet("credit-check/{customerId}")]
        public async Task<IActionResult> CheckCreditLimit(long customerId)
        {
            var result = await _paymentGatingService.CheckCreditLimitAsync(customerId);
            return Ok(result);
        }

        // -----------------------------------------------
        // LEDGER PERIOD-BASED SUMMARY (Gap #16)
        // -----------------------------------------------
        [HttpGet("ledger-summary/{customerId}")]
        public async Task<IActionResult> GetLedgerPeriodSummary(
            long customerId,
            [FromQuery] DateTime periodStart,
            [FromQuery] DateTime periodEnd)
        {
            var result = await _accountService.GetLedgerPeriodSummaryAsync(customerId, periodStart, periodEnd);
            return Ok(result);
        }

        // -----------------------------------------------
        // INVOICE LINE ITEMS (Ad-hoc / Miscellaneous Charges)
        // -----------------------------------------------

        [HttpGet("invoice-line-items/{proformaInvoiceHeaderId}")]
        public async Task<IActionResult> GetLineItems(long proformaInvoiceHeaderId)
        {
            var result = await _accountService.GetLineItemsAsync(proformaInvoiceHeaderId);
            return Ok(result);
        }

        [HttpGet("tax-invoice-line-items/{taxInvoiceId}")]
        public async Task<IActionResult> GetLineItemsByTaxInvoice(long taxInvoiceId)
        {
            var result = await _accountService.GetLineItemsByTaxInvoiceIdAsync(taxInvoiceId);
            return Ok(result);
        }

        [HttpPost("invoice-line-items")]
        public async Task<IActionResult> CreateLineItem([FromBody] InvoiceLineItemDto dto)
        {
            var result = await _accountService.CreateLineItemAsync(dto);
            return Ok(result);
        }

        [HttpPut("invoice-line-items/{id}")]
        public async Task<IActionResult> UpdateLineItem(long id, [FromBody] InvoiceLineItemDto dto)
        {
            var result = await _accountService.UpdateLineItemAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("invoice-line-items/{id}")]
        public async Task<IActionResult> DeleteLineItem(long id)
        {
            await _accountService.DeleteLineItemAsync(id);
            return Ok(new { message = "Line item deleted successfully" });
        }

        // -------------------------------
        // ZONE 7: CASE CLOSURE
        // -------------------------------
        [HttpGet("cases/{inwardId}/can-close")]
        public async Task<IActionResult> CanCloseCase(long inwardId)
        {
            var (canClose, reason) = await _caseClosureService.CanCloseCaseAsync(inwardId);
            return Ok(new { canClose, reason });
        }

        [HttpPost("cases/{inwardId}/close")]
        public async Task<IActionResult> CloseCase(long inwardId)
        {
            var employeeId = Helpers.LoggedInUserProvider.CurrentUser?.EmployeeID ?? 0;
            await _caseClosureService.CloseCaseAsync(inwardId, employeeId);
            return Ok(new { message = "Case closed successfully" });
        }
    }
}
