using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MeasurementUncertaintyController : ControllerBase
    {
        private readonly IMeasurementUncertaintyService _service;

        public MeasurementUncertaintyController(IMeasurementUncertaintyService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.MeasurementUncertainty.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> QueryUncertainties([FromBody] MeasurementUncertaintyListRequest request)
        {
            return Ok(await _service.FetchUncertaintyList(request ?? new MeasurementUncertaintyListRequest()));
        }

        [RequirePermission(Permissions.MeasurementUncertainty.Read)]
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetUncertaintyDetails(long id)
        {
            return Ok(await _service.GetUncertaintyDetails(id));
        }

        [RequirePermission(Permissions.MeasurementUncertainty.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUncertainty([FromBody] MeasurementUncertaintyCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _service.CreateUncertainty(dto);
            return Ok(new { status = "success", message = $"Measurement uncertainty '{dto.Name}' created successfully." });
        }

        [RequirePermission(Permissions.MeasurementUncertainty.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUncertainty([FromBody] MeasurementUncertaintyUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _service.ModifyUncertainty(dto);
            return Ok(new { status = "success", message = $"Measurement uncertainty '{dto.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.MeasurementUncertainty.Update)]
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleUncertaintyStatus(long id)
        {
            var isActive = await _service.ToggleUncertaintyStatus(id);
            return Ok(new
            {
                status = "success",
                isActive,
                message = $"Measurement uncertainty {(isActive ? "activated" : "deactivated")} successfully."
            });
        }

        [RequirePermission(Permissions.MeasurementUncertainty.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUncertainty(long id)
        {
            await _service.RemoveUncertainty(id);
            return Ok(new { status = "success", message = "Measurement uncertainty deactivated successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetUncertaintyDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            return Ok(await _service.GetUncertaintyDropdown(searchTerm, pageNo, pageSize));
        }

        [RequirePermission(Permissions.MeasurementUncertainty.Read)]
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateUncertainty([FromBody] MeasurementUncertaintyValidateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(await _service.ValidateUncertainty(request));
        }
    }
}
