using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerLedgerController : ControllerBase
    {
        private readonly ICustomerLedgerService _ledgerService;

        public CustomerLedgerController(ICustomerLedgerService ledgerService)
        {
            _ledgerService = ledgerService;
        }

        // ===== LEDGER OPERATIONS =====

        // GET api/customerledger/{customerId}?from=&to=
        [HttpGet("{customerId}")]
        [RequirePermission(Permissions.Account.ReadCustomerLedger)]
        public async Task<IActionResult> GetLedger(long customerId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var result = await _ledgerService.GetCustomerLedger(customerId, from, to);
            return Ok(result);
        }

        // POST api/customerledger/{customerId}/list (paginated)
        [HttpPost("{customerId}/list")]
        [RequirePermission(Permissions.Account.ReadCustomerLedger)]
        public async Task<IActionResult> GetLedgerPaged(long customerId, [FromBody] PageFilter filter)
        {
            var result = await _ledgerService.GetLedgerEntries(customerId, filter);
            return Ok(result);
        }

        // GET api/customerledger/{customerId}/balance
        [HttpGet("{customerId}/balance")]
        [RequirePermission(Permissions.Account.ReadCustomerLedger)]
        public async Task<IActionResult> GetBalance(long customerId)
        {
            var result = await _ledgerService.GetCustomerBalance(customerId);
            return Ok(result);
        }

        // GET api/customerledger/{customerId}/statement?from=&to=
        [HttpGet("{customerId}/statement")]
        [RequirePermission(Permissions.Account.ReadCustomerLedger)]
        public async Task<IActionResult> GetStatement(long customerId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _ledgerService.GetCustomerStatement(customerId, from, to);
            return Ok(result);
        }

        // ===== PAYMENT RECORDING =====

        // POST api/customerledger/payment
        [HttpPost("payment")]
        [RequirePermission(Permissions.Account.RecordPayment)]
        public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentDto dto)
        {
            var result = await _ledgerService.RecordPayment(dto);
            return Ok(result);
        }

        // GET api/customerledger/receipt/{receiptId}
        [HttpGet("receipt/{receiptId}")]
        [RequirePermission(Permissions.Account.ReadReceipt)]
        public async Task<IActionResult> GetReceipt(long receiptId)
        {
            var result = await _ledgerService.GetReceipt(receiptId);
            if (result == null)
                return NotFound(new { message = "Receipt not found" });
            return Ok(result);
        }

        // GET api/customerledger/{customerId}/receipts
        [HttpGet("{customerId}/receipts")]
        [RequirePermission(Permissions.Account.ReadReceipt)]
        public async Task<IActionResult> GetReceipts(long customerId)
        {
            var result = await _ledgerService.GetPaymentReceipts(customerId);
            return Ok(result);
        }

        // POST api/customerledger/{customerId}/receipts-paged
        [HttpPost("{customerId}/receipts-paged")]
        [RequirePermission(Permissions.Account.ReadReceipt)]
        public async Task<IActionResult> GetReceiptsPaged(long customerId, [FromBody] PageFilter filter)
        {
            var result = await _ledgerService.GetReceiptsPaged(customerId, filter);
            return Ok(result);
        }

        // ===== REPORTS =====

        // GET api/customerledger/aging-report
        [HttpGet("aging-report")]
        [RequirePermission(Permissions.Account.ReadAging)]
        public async Task<IActionResult> GetAgingReport([FromQuery] DateTime? asOfDate = null)
        {
            var result = await _ledgerService.GetAgingReport(asOfDate);
            return Ok(result);
        }

        // GET api/customerledger/outstanding-report
        [HttpGet("outstanding-report")]
        [RequirePermission(Permissions.Account.ReadOutstanding)]
        public async Task<IActionResult> GetOutstandingReport()
        {
            var result = await _ledgerService.GetOutstandingReport();
            return Ok(result);
        }

        // GET api/customerledger/collection-summary?from=&to=
        [HttpGet("collection-summary")]
        [RequirePermission(Permissions.Account.ReadCollectionSummary)]
        public async Task<IActionResult> GetCollectionSummary([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _ledgerService.GetCollectionSummary(from, to);
            return Ok(result);
        }

        // ===== CREDIT MANAGEMENT =====

        // GET api/customerledger/{customerId}/credit-status
        [HttpGet("{customerId}/credit-status")]
        [RequirePermission(Permissions.Account.ReadCreditStatus)]
        public async Task<IActionResult> GetCreditStatus(long customerId)
        {
            var result = await _ledgerService.GetCreditStatus(customerId);
            return Ok(result);
        }

        // GET api/customerledger/{customerId}/check-credit?amount=
        [HttpGet("{customerId}/check-credit")]
        [RequirePermission(Permissions.Account.ReadCreditStatus)]
        public async Task<IActionResult> CheckCreditLimit(long customerId, [FromQuery] decimal amount)
        {
            var withinLimit = await _ledgerService.CheckCreditLimit(customerId, amount);
            return Ok(new { withinLimit });
        }
    }
}
