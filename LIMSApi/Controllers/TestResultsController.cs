using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.ServiceWORepo;
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

        public TestResultsController(ITestResultService service)
        {
            _service = service;
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
        // Full Result Payload (Sample-wise)
        // =============================================================
        [HttpGet("full-result-payload/{sampleId}")]
        public async Task<IActionResult> GetFullResultPayload(long sampleId)
        {
            return Ok(await _service.GetSampleDetailsForResult(sampleId));
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
        public async Task<IActionResult> SaveTestResult(TestResultSaveDto dto)
        {
            await _service.SaveTestResult(dto);
            return Ok(new { Success = true, Message = "Test saved successfully" });
        }

        // =============================================================
        // Start Test
        // =============================================================
        [HttpPost("start-test/{headerId}")]
        public async Task<IActionResult> StartTest(long headerId)
        {
            await _service.StartTest(headerId);
            return Ok(new { Success = true, Message = "Test started" });
        }

        // =============================================================
        // Complete Test
        // =============================================================
        [HttpPost("complete-test/{headerId}")]
        public async Task<IActionResult> CompleteTest(long headerId)
        {
            await _service.CompleteTest(headerId);
            return Ok(new { Success = true, Message = "Test completed" });
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
    }
}
