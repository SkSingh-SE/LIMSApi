using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services;
using LIMSApi.Services.Interface;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Voice;

public record CancelSampleRequest(long SampleDetailId, string Reason);

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

        [RequirePermission(Permissions.Inward.Read)]
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


        [RequirePermission(Permissions.Inward.Read)]
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


        [RequirePermission(Permissions.Inward.Update)]
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

        [RequirePermission(Permissions.Inward.Update)]
        [HttpPatch("{id}/payment")]
        public async Task<IActionResult> UpdatePaymentInfo(long id, [FromBody] PaymentInfoDto dto)
        {
            var result = await _SampleInwardService.UpdatePaymentInfoAsync(id, dto);
            return Ok(result);
        }

        [HttpPatch("update-prep/{sampleId}")]
        public async Task<IActionResult> UpdateSamplePrep(long sampleId, [FromBody] SamplePrepReviewDto dto)
        {
            await _SampleInwardService.UpdateSamplePrepAsync(sampleId, dto);
            return Ok(new { message = "Preparation details updated." });
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

        [RequirePermission(Permissions.Inward.Create)]
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

        [RequirePermission(Permissions.Inward.Delete)]
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

        [HttpPost("cancel-sample")]
        [RequirePermission(Permissions.Inward.Update)]
        public async Task<IActionResult> CancelSample([FromBody] CancelSampleRequest request)
        {
            await _SampleInwardService.CancelSampleAsync(request.SampleDetailId, request.Reason);
            return Ok(new { message = "Sample cancelled successfully." });
        }

        [HttpDelete("delete-sample/{id}")]
        [RequirePermission(Permissions.Inward.Update)]
        public async Task<IActionResult> DeleteSample(long id)
        {
            await _SampleInwardService.DeleteSampleAsync(id);
            return Ok(new { message = "Sample deleted successfully." });
        }

    }
}
