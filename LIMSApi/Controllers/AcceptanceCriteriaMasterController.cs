using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcceptanceCriteriaMasterController : ControllerBase
    {
        private readonly IAcceptanceCriteriaService _service;

        public AcceptanceCriteriaMasterController(IAcceptanceCriteriaService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.AcceptanceCriteria.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> AcceptanceCriteriaList([FromBody] PageFilter filter)
        {
            return Ok(await _service.FetchAcceptanceCriteriaList(filter));
        }

        [RequirePermission(Permissions.AcceptanceCriteria.Read)]
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetAcceptanceCriteria(long id)
        {
            var entity = await _service.GetAcceptanceCriteriaDetails(id);
            return Ok(entity);
        }

        [RequirePermission(Permissions.AcceptanceCriteria.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> PostAcceptanceCriteria([FromBody] AcceptanceCriteriaMasterCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.CreateAcceptanceCriteria(dto);
            return Ok(new
            {
                status = "success",
                message = $"Acceptance criteria '{dto.Name}' created successfully."
            });
        }

        [RequirePermission(Permissions.AcceptanceCriteria.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutAcceptanceCriteria([FromBody] AcceptanceCriteriaMasterUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.ModifyAcceptanceCriteria(dto);
            return Ok(new
            {
                status = "success",
                message = $"Acceptance criteria '{dto.Name}' updated successfully."
            });
        }

        [RequirePermission(Permissions.AcceptanceCriteria.Update)]
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var isActive = await _service.ToggleAcceptanceCriteriaStatus(id);
            return Ok(new
            {
                status = "success",
                isActive,
                message = $"Acceptance criteria {(isActive ? "activated" : "deactivated")} successfully."
            });
        }

        [RequirePermission(Permissions.AcceptanceCriteria.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAcceptanceCriteria(long id)
        {
            await _service.RemoveAcceptanceCriteria(id);
            return Ok(new
            {
                status = "success",
                message = "Acceptance criteria deactivated successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetAcceptanceCriteriaDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            var data = await _service.GetAcceptanceCriteriaDropdown(searchTerm, pageNo, pageSize);
            return Ok(data);
        }
    }
}
