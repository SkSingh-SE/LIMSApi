using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConditionMasterController : ControllerBase
    {
        private readonly IConditionMasterService _service;

        public ConditionMasterController(IConditionMasterService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.ConditionMaster.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> ConditionMasterList([FromBody] PageFilter filter)
        {
            return Ok(await _service.FetchConditionMasterList(filter));
        }

        [RequirePermission(Permissions.ConditionMaster.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<ConditionMasterDto>> GetConditionMaster(long id)
        {
            var entity = await _service.GetConditionMasterDetails(id);
            return entity == null ? NotFound(new { message = $"Condition Master with ID {id} not found." }) : Ok(entity);
        }

        [RequirePermission(Permissions.ConditionMaster.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> PostConditionMaster([FromBody] ConditionMasterCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.CreateConditionMaster(dto);
            return Ok(new
            {
                status = "success",
                message = $"Condition Master '{dto.Name}' created successfully."
            });
        }

        [RequirePermission(Permissions.ConditionMaster.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutConditionMaster([FromBody] ConditionMasterUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.ModifyConditionMaster(dto);
            return Ok(new
            {
                status = "success",
                message = $"Condition Master '{dto.Name}' updated successfully."
            });
        }

        [RequirePermission(Permissions.ConditionMaster.Update)]
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var isActive = await _service.ToggleConditionMasterStatus(id);
            return Ok(new
            {
                status = "success",
                isActive,
                message = $"Condition Master {(isActive ? "activated" : "deactivated")} successfully."
            });
        }

        [RequirePermission(Permissions.ConditionMaster.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteConditionMaster(long id)
        {
            await _service.DeleteConditionMaster(id);
            return Ok(new
            {
                status = "success",
                message = "Condition Master deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetConditionMasterDropdown(string? searchTerm, int pageNo = 0, int pageSize = 50)
        {
            var data = await _service.GetConditionMasterDropdown(searchTerm, pageNo, pageSize);
            return Ok(data);
        }
    }
}
