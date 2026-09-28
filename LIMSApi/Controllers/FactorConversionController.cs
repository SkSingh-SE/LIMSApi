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
    public class FactorConversionController : ControllerBase
    {
        private readonly IFactorConversionService _service;

        public FactorConversionController(IFactorConversionService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.FactorConversion.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> QueryFactors([FromBody] FactorConversionListRequest request)
        {
            return Ok(await _service.FetchFactorList(request ?? new FactorConversionListRequest()));
        }

        [RequirePermission(Permissions.FactorConversion.Read)]
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetFactorDetails(long id)
        {
            return Ok(await _service.GetFactorDetails(id));
        }

        [RequirePermission(Permissions.FactorConversion.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateFactor([FromBody] FactorConversionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _service.CreateFactor(dto);
            return Ok(new { status = "success", message = $"Factor/conversion '{dto.Name}' created successfully." });
        }

        [RequirePermission(Permissions.FactorConversion.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateFactor([FromBody] FactorConversionUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _service.ModifyFactor(dto);
            return Ok(new { status = "success", message = $"Factor/conversion '{dto.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.FactorConversion.Update)]
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleFactorStatus(long id)
        {
            var isActive = await _service.ToggleFactorStatus(id);
            return Ok(new
            {
                status = "success",
                isActive,
                message = $"Factor/conversion {(isActive ? "activated" : "deactivated")} successfully."
            });
        }

        [RequirePermission(Permissions.FactorConversion.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFactor(long id)
        {
            await _service.RemoveFactor(id);
            return Ok(new { status = "success", message = "Factor/conversion deactivated successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetFactorDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            return Ok(await _service.GetFactorDropdown(searchTerm, pageNo, pageSize));
        }

        [RequirePermission(Permissions.FactorConversion.Read)]
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateFactor([FromBody] FactorValidateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(await _service.ValidateFactor(request));
        }
    }
}
