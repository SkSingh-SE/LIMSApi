using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisTechniqueController : ControllerBase
    {
        private readonly IAnalysisTechniqueService _service;

        public AnalysisTechniqueController(IAnalysisTechniqueService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.AnalysisTechnique.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> AnalysisTechniqueList([FromBody] PageFilter filter)
        {
            return Ok(await _service.FetchAnalysisTechniqueList(filter));
        }

        [RequirePermission(Permissions.AnalysisTechnique.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<AnalysisTechniqueDetailDto>> GetAnalysisTechnique(long id)
        {
            var entity = await _service.GetAnalysisTechniqueDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [RequirePermission(Permissions.AnalysisTechnique.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> PostAnalysisTechnique([FromBody] AnalysisTechniqueCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.CreateAnalysisTechnique(dto);
            return Ok(new
            {
                status = "success",
                message = $"Analysis Technique '{dto.Name}' created successfully."
            });
        }

        [RequirePermission(Permissions.AnalysisTechnique.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutAnalysisTechnique([FromBody] AnalysisTechniqueUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.ModifyAnalysisTechnique(dto);
            return Ok(new
            {
                status = "success",
                message = $"Analysis Technique '{dto.Name}' updated successfully."
            });
        }

        [RequirePermission(Permissions.AnalysisTechnique.Update)]
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var isActive = await _service.ToggleAnalysisTechniqueStatus(id);
            return Ok(new
            {
                status = "success",
                isActive,
                message = $"Analysis Technique {(isActive ? "activated" : "deactivated")} successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetAnalysisTechniqueDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            var data = await _service.GetAnalysisTechniqueDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
