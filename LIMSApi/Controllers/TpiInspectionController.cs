using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TpiInspectionController : ControllerBase
    {
        private readonly ITpiService _tpiService;

        public TpiInspectionController(ITpiService tpiService)
        {
            _tpiService = tpiService;
        }

        [HttpGet("by-inward/{sampleInwardId}")]
        public async Task<IActionResult> GetInspectionsBySampleInward(long sampleInwardId)
        {
            var inspections = await _tpiService.GetInspectionsBySampleInward(sampleInwardId);
            return Ok(inspections);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetInspectionDetails(long id)
        {
            var inspection = await _tpiService.GetInspectionDetails(id);
            return inspection == null ? NoContent() : Ok(inspection);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateInspection([FromBody] CreateTpiInspectionDto dto)
        {
            var inspection = await _tpiService.CreateInspection(dto);
            return Ok(new { status = "success", message = "TPI Inspection created successfully.", data = inspection });
        }

        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateInspectionStatus(long id, [FromBody] UpdateTpiStatusDto dto)
        {
            var inspection = await _tpiService.UpdateInspectionStatus(id, dto);
            return Ok(new { status = "success", message = $"TPI Inspection status updated to '{dto.Status}'.", data = inspection });
        }

        [HttpGet("is-cleared/{sampleInwardId}/{stage}")]
        public async Task<IActionResult> IsTpiCleared(long sampleInwardId, TpiStage stage)
        {
            var isCleared = await _tpiService.IsTpiCleared(sampleInwardId, stage);
            return Ok(new { isCleared });
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingInspections()
        {
            var inspections = await _tpiService.GetPendingInspections();
            return Ok(inspections);
        }
    }
}
