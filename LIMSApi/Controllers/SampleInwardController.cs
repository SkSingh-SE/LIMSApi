using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services;
using LIMSApi.Services.Interface;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Voice;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SampleInwardController : ControllerBase
    {
        private readonly ISampleInwardService _SampleInwardService;
        private readonly IPaymentGatingService _paymentGatingService;

        public SampleInwardController(ISampleInwardService SampleInwardServce, IPaymentGatingService paymentGatingService)
        {
            _SampleInwardService = SampleInwardServce;
            _paymentGatingService = paymentGatingService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> SampleInwardList(PageFilter filter)
        {
            return Ok(await _SampleInwardService.FetchSampleInwardList(filter));
        }

        [HttpPost("plan-list")]
        public async Task<IActionResult> PlanList(PageFilter filter)
        {
            return Ok(await _SampleInwardService.FetchPlanList(filter));
        }

        [HttpPost("review-list")]
        public async Task<IActionResult> ReviewList(PageFilter filter)
        {
            return Ok(await _SampleInwardService.FetchReviewList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetSampleInward(long id)
        {
            var entity = await _SampleInwardService.GetSampleInwardDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpGet("details-with-plan/{id}")]
        public async Task<IActionResult> GetSampleInwardWithPlans(long id)
        {
            var entity = await _SampleInwardService.GetSampleInwardWithPlans(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutSampleInward([FromForm] SampleInwardDto model)
        {
            await _SampleInwardService.ModifySampleInward(model);
            return Ok(new
            {
                status = "success",
                message = $"SampleInward updated successfully."
            });
        }

        [HttpPut("plan")]
        public async Task<IActionResult> ModifySamplePlan(PlanDto model)
        {
            await _SampleInwardService.ModifySamplePlan(model);
            return Ok(new
            {
                status = "success",
                message = $"Plan has been updated successfully."
            });
        }

        [HttpPut("plan-for-review")]
        public async Task<IActionResult> SubmitPlanForReview(PlanDto model)
        {
            await _SampleInwardService.SubmitPlanForReview(model);
            return Ok(new
            {
                status = "success",
                message = $"Plan has been updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<SampleInward>> PostSampleInward([FromForm] SampleInwardDto model)
        {
            var id = await _SampleInwardService.CreateSampleInward(model);

            // Advisory credit limit check — never blocks inward creation
            string? creditWarning = null;
            try
            {
                var creditCheck = await _paymentGatingService.CheckCreditLimitAsync(model.CustomerID);
                creditWarning = creditCheck.Warning;
            }
            catch { /* silent — credit check failure should not affect inward */ }

            return Ok(new
            {
                status = "success",
                message = $"SampleInward created successfully.",
                id,
                creditWarning
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSampleInward(long id)
        {
            var entity = await _SampleInwardService.GetSampleInwardDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("SampleInward not found!");
            }
            await _SampleInwardService.RemoveSampleInward(id);
            return Ok(new
            {
                status = "success",
                message = $"SampleInward deleted successfully."
            });
        }

        [HttpGet("case-number")]
        public async Task<IActionResult> GetCaseNoAndSampleNo()
        {
            var caseNumber = await _SampleInwardService.GetCaseNoAndSampleNo();

            return caseNumber == null ? NoContent() : Ok(caseNumber);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> Dropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _SampleInwardService.GetSampleInwardDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);   
        }

        [HttpGet("preparation-inward-dropdown")]
        public async Task<IActionResult> PreprationInwardDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _SampleInwardService.GetSamplePreparationInwardDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);   
        }

        [HttpGet("{piId}/pdf")]
        public async Task<IActionResult> GetPIPdf(long piId)
        {
            try
            {
                var pdfBytes = await _SampleInwardService.GeneratePIPdfAsync(piId);

                if (pdfBytes == null || pdfBytes.Length == 0)
                    return NotFound("PDF could not be generated.");

                var fileName = $"ProformaInvoice_{piId}.pdf";

                return File(
                    pdfBytes,
                    "application/pdf",
                    fileName
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
