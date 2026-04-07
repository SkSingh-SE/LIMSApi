using LIMSApi.Dtos;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SamplePreparationController : ControllerBase
    {
        private readonly ISamplePreparationService _service;

        public SamplePreparationController(ISamplePreparationService service)
        {
            _service = service;
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetAll([FromBody] PageFilter filter)
        {
            return Ok(await _service.GetAllAsync(filter));
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpGet("by-sample/{sampleId}")]
        public async Task<IActionResult> GetOrCreateBySample(long sampleId)
        {
            return Ok(await _service.GetOrCreateBySampleAsync(sampleId));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] SamplePreparationCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(new { message = "Preparation record created successfully.", data = result });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] SamplePreparationUpdateDto dto)
        {
            var result = await _service.UpdateAsync(dto);
            return Ok(new { message = "Preparation record updated successfully.", data = result });
        }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] SamplePreparationStatusDto dto)
        {
            await _service.UpdateStatusAsync(id, dto);
            return Ok(new { message = $"Status updated to '{dto.Status}' successfully." });
        }

        [HttpPost("refresh-charges/{sampleId}")]
        public async Task<IActionResult> RefreshCharges(long sampleId)
        {
            await _service.RefreshChargeTotalsAsync(sampleId);
            return Ok(new { message = "Charge totals refreshed." });
        }
    }
}
