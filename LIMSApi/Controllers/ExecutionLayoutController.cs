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
    public class ExecutionLayoutController : ControllerBase
    {
        private readonly IExecutionLayoutService _service;

        public ExecutionLayoutController(IExecutionLayoutService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.ExecutionLayout.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] ExecutionLayoutListRequest request)
        {
            return Ok(await _service.FetchLayoutList(request ?? new ExecutionLayoutListRequest()));
        }

        [RequirePermission(Permissions.ExecutionLayout.Read)]
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            return Ok(await _service.GetLayoutDetails(id));
        }

        [RequirePermission(Permissions.ExecutionLayout.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ExecutionLayoutCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.CreateLayout(dto);
            return Ok(new { status = "success", message = $"Execution layout '{dto.Name}' created successfully." });
        }

        [RequirePermission(Permissions.ExecutionLayout.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] ExecutionLayoutUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.ModifyLayout(dto);
            return Ok(new { status = "success", message = $"Execution layout '{dto.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.ExecutionLayout.Update)]
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var isActive = await _service.ToggleLayoutStatus(id);
            return Ok(new { status = "success", isActive, message = $"Execution layout {(isActive ? "activated" : "deactivated")} successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> Dropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            return Ok(await _service.GetLayoutDropdown(searchTerm, pageNo, pageSize));
        }

        [RequirePermission(Permissions.ExecutionLayout.Read)]
        [HttpGet("validate/{id}")]
        public async Task<IActionResult> Validate(long id)
        {
            return Ok(await _service.ValidateLayout(id));
        }

        [RequirePermission(Permissions.ExecutionLayout.Read)]
        [HttpGet("metadata")]
        public async Task<IActionResult> Metadata()
        {
            return Ok(await _service.GetMetadataAsync());
        }
    }
}
