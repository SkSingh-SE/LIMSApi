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
    public class EquipmentRequirementController : ControllerBase
    {
        private readonly IEquipmentRequirementService _service;

        public EquipmentRequirementController(IEquipmentRequirementService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.EquipmentRequirement.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> QueryRequirements([FromBody] EquipmentRequirementListRequest request)
        {
            return Ok(await _service.FetchRequirementList(request ?? new EquipmentRequirementListRequest()));
        }

        [RequirePermission(Permissions.EquipmentRequirement.Read)]
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetRequirementDetails(long id)
        {
            return Ok(await _service.GetRequirementDetails(id));
        }

        [RequirePermission(Permissions.EquipmentRequirement.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateRequirement([FromBody] EquipmentRequirementCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _service.CreateRequirement(dto);
            return Ok(new { status = "success", message = $"Equipment requirement '{dto.Name}' created successfully." });
        }

        [RequirePermission(Permissions.EquipmentRequirement.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateRequirement([FromBody] EquipmentRequirementUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _service.ModifyRequirement(dto);
            return Ok(new { status = "success", message = $"Equipment requirement '{dto.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.EquipmentRequirement.Update)]
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleRequirementStatus(long id)
        {
            var isActive = await _service.ToggleRequirementStatus(id);
            return Ok(new
            {
                status = "success",
                isActive,
                message = $"Equipment requirement {(isActive ? "activated" : "deactivated")} successfully."
            });
        }

        [RequirePermission(Permissions.EquipmentRequirement.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequirement(long id)
        {
            await _service.RemoveRequirement(id);
            return Ok(new { status = "success", message = "Equipment requirement deactivated successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetRequirementDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            return Ok(await _service.GetRequirementDropdown(searchTerm, pageNo, pageSize));
        }

        [RequirePermission(Permissions.EquipmentRequirement.Read)]
        [HttpGet("eligible-equipment")]
        public async Task<IActionResult> GetEligibleEquipment([FromQuery] long requirementId, [FromQuery] long branchId)
        {
            return Ok(await _service.GetEligibleEquipment(requirementId, branchId));
        }
    }
}
