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
    public class LabScopeController : ControllerBase
    {
        private readonly ILabScopeConfigurationService _service;

        public LabScopeController(ILabScopeConfigurationService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.LabScope.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> QueryScopes([FromBody] ScopeListRequest request)
        {
            return Ok(await _service.QueryScopesAsync(request ?? new ScopeListRequest()));
        }

        [RequirePermission(Permissions.LabScope.Read)]
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetScopeDetails(long id)
        {
            return Ok(await _service.GetScopeDetailsAsync(id));
        }

        [RequirePermission(Permissions.LabScope.Read)]
        [HttpGet("accreditation-context")]
        public async Task<IActionResult> GetAccreditationContext([FromQuery] long? branchId)
        {
            return Ok(await _service.GetAccreditationContextAsync(branchId));
        }

        [RequirePermission(Permissions.LabScope.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateScope([FromBody] ScopeCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var id = await _service.CreateScopeAsync(dto);
            return Ok(new { status = "success", id, message = "Lab scope created successfully." });
        }

        [RequirePermission(Permissions.LabScope.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateScope([FromBody] ScopeUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _service.ModifyScopeAsync(dto);
            return Ok(new { status = "success", message = "Lab scope updated successfully." });
        }

        [RequirePermission(Permissions.LabScope.Update)]
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleScopeStatus(long id, [FromQuery] bool activate = true)
        {
            var isActive = await _service.ToggleScopeStatusAsync(id, activate);
            return Ok(new
            {
                status = "success",
                isActive,
                message = $"Lab scope {(isActive ? "activated" : "deactivated")} successfully."
            });
        }

        [RequirePermission(Permissions.LabScope.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScope(long id)
        {
            await _service.RemoveScopeAsync(id);
            return Ok(new { status = "success", message = "Lab scope deactivated successfully." });
        }

        [RequirePermission(Permissions.LabScope.Read)]
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateScope([FromBody] ScopeValidateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(await _service.ValidateAsync(request));
        }

        [RequirePermission(Permissions.LabScope.Read)]
        [HttpPost("preview")]
        public async Task<IActionResult> PreviewScope([FromBody] ScopePreviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(await _service.PreviewAsync(request));
        }
    }
}
