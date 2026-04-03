using LIMSApi.Dtos;
using LIMSApi.Middleware;
using LIMSApi.Models;
using LIMSApi.ServiceWORepo;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestResultsController : ControllerBase
    {
        private readonly ITestResultService _service;
        private readonly ITestPriceCalculationService _priceService;
        private readonly INablScopeValidationService _nablService;

        public TestResultsController(
            ITestResultService service,
            ITestPriceCalculationService priceService,
            INablScopeValidationService nablService)
        {
            _service = service;
            _priceService = priceService;
            _nablService = nablService;
        }

        // =============================================================
        // Dashboard List
        // =============================================================
        [HttpPost("list")]
        public async Task<IActionResult> List(PageFilter filter)
        {
            return Ok(await _service.GetTestingDashboardList(filter));
        }

        // =============================================================
        // Load Parameters from Specification
        // =============================================================
        [HttpPost("load-parameters/{headerId}")]
        public async Task<IActionResult> LoadParametersFromSpec(long headerId)
        {
            var result = await _service.LoadParametersFromSpecAsync(headerId);
            return Ok(result);
        }

        // =============================================================
        // Full Result Payload (Sample-wise)
        // =============================================================
        [HttpGet("full-result-payload/{sampleId}")]
        public async Task<IActionResult> GetFullResultPayload(long sampleId)
        {
            try
            {
                return Ok(await _service.GetSampleDetailsForResult(sampleId));
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace?.Split('\n').Take(10) });
            }
        }

        // =============================================================
        // Header + Parameters
        // =============================================================
        [HttpGet("{headerId}")]
        public async Task<IActionResult> Get(long headerId)
        {
            var header = await _service.GetHeaderAsync(headerId);
            if (header == null) return NotFound();
            return Ok(header);
        }

        // =============================================================
        // Save Result (Draft / Progress)
        // =============================================================
        [HttpPost("save-test-result")]
        [RequirePermission("TEST_RESULT_SAVE")]
        public async Task<IActionResult> SaveTestResult(TestResultSaveDto dto)
        {
            await _service.SaveTestResult(dto);
            return Ok(new { Success = true, Message = "Test saved successfully" });
        }

        // =============================================================
        // Orientation Mismatch Check
        // =============================================================
        [HttpGet("orientation-check/{headerId}")]
        public async Task<IActionResult> CheckOrientationMismatch(long headerId)
        {
            var result = await _service.CheckOrientationMismatch(headerId);
            return Ok(result);
        }

        // =============================================================
        // Start Test
        // =============================================================
        [HttpPost("start-test/{headerId}")]
        public async Task<IActionResult> StartTest(long headerId)
        {
            var response = await _service.StartTest(headerId);
            return Ok(response);
        }

        // =============================================================
        // Complete Test
        // =============================================================
        [HttpPost("complete-test/{headerId}")]
        public async Task<IActionResult> CompleteTest(long headerId)
        {
            var response = await _service.CompleteTest(headerId);
            return Ok(response);
        }

        // =============================================================
        // Update Parameter
        // =============================================================
        [HttpPost("update-parameter/{headerId}/parameter/{paramId}")]
        public async Task<IActionResult> UpdateParameter(long headerId, long paramId, TestResultParameterDto param)
        {
            var response = await _service.UpdateParameterAsync(headerId, paramId, param);
            return Ok(response);
        }

        // =============================================================
        // Move Test to Long-Term
        // =============================================================
        [HttpPost("move-to-long-term")]
        public async Task<IActionResult> MoveToLongTerm([FromBody] MoveToLongTermDto dto)
        {
            await _service.MoveToLongTerm(dto);
            return Ok(new { Success = true, Message = "Moved to long-term" });
        }

        [HttpPost("long-term/list")]
        public async Task<IActionResult> GetLongTermList(PageFilter filter)
        {
            var result = await _service.GetLongTermList(filter);
            return Ok(result);
        }

        [HttpGet("long-term/{id}")]
        public async Task<IActionResult> GetLongTermDetail(long id)
        {
            var result = await _service.GetLongTermDetail(id);
            if (result == null)
                return NotFound(new { message = "Long-term test not found." });

            return Ok(result);
        }

        [HttpGet("parameters/header/{headerId}")]
        public async Task<IActionResult> GetParametersForHeader(long headerId)
        {
            var result = await _service.GetParametersForHeader(headerId);

            if (result == null)
                return NotFound(new { message = "Header not found." });

            return Ok(result);
        }

        [HttpPost("long-term/record")]
        public async Task<IActionResult> RecordLongTermReading(LongTermRecordDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid payload.");

            await _service.RecordLongTermReading(dto);

            return Ok(new
            {
                success = true,
                message = "Reading recorded successfully."
            });
        }

        [HttpPost("{headerId}/upload-image")]
        public async Task<IActionResult> UploadImage(long headerId, IFormFile file, string? caption)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var imageUrl = await _service.UploadTestImageAsync(headerId, file, caption);
            return Ok(new { imageUrl });
        }

        [HttpPost("{headerId}/images")]
        public async Task<IActionResult> UploadImages(long headerId,[FromForm] List<IFormFile> files,[FromForm] List<string>? captions)
        {
            if (files == null || !files.Any())
                return BadRequest("No files uploaded");

            var uploadedImages = await _service.UploadTestImagesAsync(
                headerId,
                files,
                captions
            );

            return Ok(uploadedImages);
        }

        [HttpGet("{headerId}/images")]
        public async Task<IActionResult> UploadImages(long headerId)
        {

            var uploadedImages = await _service.UploadedTestImages(headerId);

            return Ok(uploadedImages);
        }

        // =============================================================
        // Calculate Parameters (trigger recalculation for all derived)
        // =============================================================
        [HttpPost("calculate-parameters/{headerId}")]
        public async Task<IActionResult> CalculateParameters(long headerId)
        {
            var result = await _service.CalculateParameters(headerId);
            return Ok(result);
        }

        // =============================================================
        // Add Standalone Parameter
        // =============================================================
        [HttpPost("add-standalone-parameter/{headerId}")]
        public async Task<IActionResult> AddStandaloneParameter(long headerId, [FromBody] AddStandaloneParameterDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid payload.");

            var result = await _service.AddStandaloneParameter(headerId, dto);
            return Ok(result);
        }

        // =============================================================
        // Add Parameter from Another Test Method
        // =============================================================
        [HttpPost("add-parameter-from-method/{headerId}")]
        public async Task<IActionResult> AddParameterFromMethod(long headerId, [FromBody] AddParameterFromMethodDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid payload.");

            var result = await _service.AddParameterFromMethod(headerId, dto);
            return Ok(result);
        }

        // =============================================================
        // Get Environment at Test Time
        // =============================================================
        [HttpGet("environment-at-time/{headerId}")]
        public async Task<IActionResult> GetEnvironmentAtTime(long headerId)
        {
            var result = await _service.GetEnvironmentAtTime(headerId);
            return Ok(result);
        }

        // =============================================================
        // Phase 2B: Test-Level Price Calculation
        // =============================================================

        /// <summary>
        /// Calculate test price based on selected parameters × InvoiceCasePrice
        /// </summary>
        [HttpPost("calculate-price/{headerId}")]
        public async Task<IActionResult> CalculatePrice(long headerId)
        {
            var result = await _priceService.CalculateTestPrice(headerId);
            return Ok(result);
        }

        /// <summary>
        /// Get price breakdown per parameter
        /// </summary>
        [HttpGet("price-breakdown/{headerId}")]
        public async Task<IActionResult> GetPriceBreakdown(long headerId)
        {
            var result = await _priceService.GetPriceBreakdown(headerId);
            return Ok(result);
        }

        /// <summary>
        /// Override the calculated price with a manual amount and reason
        /// </summary>
        [HttpPost("override-price/{headerId}")]
        [RequirePermission("TEST_PRICE_OVERRIDE")]
        public async Task<IActionResult> OverridePrice(long headerId, [FromBody] Dtos.PriceOverrideDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid payload.");

            // Use logged-in user ID for override tracking
            long overrideById = 0;
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("sub");
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var uid))
                overrideById = uid;

            var result = await _priceService.OverridePrice(headerId, dto.Amount, dto.Reason, overrideById);
            return Ok(result);
        }

        /// <summary>
        /// Get price summary: calculated, override, final price
        /// </summary>
        [HttpGet("price-summary/{headerId}")]
        public async Task<IActionResult> GetPriceSummary(long headerId)
        {
            var result = await _priceService.GetPriceSummary(headerId);
            return Ok(result);
        }

        [HttpPost("set-pricing-type/{headerId}")]
        public async Task<IActionResult> SetPricingType(long headerId, [FromBody] SetPricingTypeDto dto)
        {
            await _priceService.SetPricingTypeAsync(headerId, dto.PricingType);
            // Recalculate price with new pricing type (not just summary)
            var result = await _priceService.CalculateTestPrice(headerId);
            return Ok(result);
        }

        [HttpGet("pricing-recommendation/{headerId}")]
        public async Task<IActionResult> GetPricingRecommendation(long headerId)
        {
            var result = await _priceService.GetPricingRecommendation(headerId);
            return Ok(result);
        }

        [HttpPost("set-pricing-with-value/{headerId}")]
        public async Task<IActionResult> SetPricingWithValue(long headerId, [FromBody] Dtos.SetPricingWithValueDto dto)
        {
            var result = await _priceService.SetPricingTypeWithValueAsync(headerId, dto.PricingType, dto.DimensionValue);
            return Ok(result);
        }

        // =============================================================
        // Phase 1: NABL Scope Check
        // =============================================================
        [HttpGet("nabl-scope-check/{headerId}")]
        public async Task<IActionResult> GetNablScopeCheck(long headerId)
        {
            var result = await _nablService.CheckAllParameters(headerId);
            return Ok(result);
        }

        // =============================================================
        // Phase 2: Uncertainty Data
        // =============================================================
        [HttpGet("uncertainty/{headerId}")]
        public async Task<IActionResult> GetUncertainty(long headerId)
        {
            var result = await _nablService.GetAllUncertainties(headerId);
            return Ok(result);
        }

        // =============================================================
        // Phase 5: Test Verification Workflow
        // =============================================================
        [HttpPost("submit-for-verification/{headerId}")]
        public async Task<IActionResult> SubmitForVerification(long headerId)
        {
            await _service.SubmitForVerification(headerId);
            return Ok(new { Success = true, Message = "Submitted for verification" });
        }

        [HttpPost("verification-list")]
        public async Task<IActionResult> GetVerificationList(PageFilter filter)
        {
            var result = await _service.GetVerificationList(filter);
            return Ok(result);
        }

        [HttpPost("verify/{headerId}")]
        [RequirePermission("TEST_RESULT_VERIFY")]
        public async Task<IActionResult> VerifyTest(long headerId, [FromBody] VerificationActionDto dto)
        {
            await _service.VerifyTest(headerId, dto?.Comments);
            return Ok(new { Success = true, Message = "Test verified successfully" });
        }

        [HttpPost("reject-verification/{headerId}")]
        [RequirePermission("TEST_RESULT_VERIFY")]
        public async Task<IActionResult> RejectVerification(long headerId, [FromBody] VerificationActionDto dto)
        {
            await _service.RejectVerification(headerId, dto?.Comments);
            return Ok(new { Success = true, Message = "Verification rejected" });
        }

        // =============================================================
        // Phase 5: Preparation Status
        // =============================================================
        [HttpGet("preparation-status/{sampleId}")]
        public async Task<IActionResult> GetPreparationStatus(long sampleId)
        {
            var result = await _service.GetPreparationStatus(sampleId);
            return Ok(result);
        }

        // =============================================================
        // Phase 6: Unified Price Summary
        // =============================================================
        [HttpGet("unified-price-summary/{sampleId}")]
        public async Task<IActionResult> GetUnifiedPriceSummary(long sampleId)
        {
            var result = await _service.GetUnifiedPriceSummary(sampleId);
            return Ok(result);
        }

        // =============================================================
        // Machining Charge Line Items
        // =============================================================
        [HttpGet("machining-items/{sampleId}")]
        public async Task<IActionResult> GetMachiningItems(long sampleId)
        {
            return Ok(await _service.GetMachiningItems(sampleId));
        }

        [HttpPost("machining-items/{sampleId}")]
        public async Task<IActionResult> AddMachiningItem(long sampleId, [FromBody] MachiningChargeItemDto dto)
        {
            return Ok(await _service.AddMachiningItem(sampleId, dto));
        }

        [HttpPut("machining-items/{itemId}")]
        public async Task<IActionResult> UpdateMachiningItem(long itemId, [FromBody] MachiningChargeItemDto dto)
        {
            return Ok(await _service.UpdateMachiningItem(itemId, dto));
        }

        [HttpDelete("machining-items/{itemId}")]
        public async Task<IActionResult> DeleteMachiningItem(long itemId)
        {
            await _service.DeleteMachiningItem(itemId);
            return NoContent();
        }
    }
}
