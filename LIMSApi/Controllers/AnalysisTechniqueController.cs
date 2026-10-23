using LIMSApi.Dtos;
using LIMSApi.Models;
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
        public async Task<IActionResult> AnalysisTechniqueList(PageFilter filter)
        {
            return Ok(await _service.FetchAnalysisTechniqueList(filter));
        }

        [RequirePermission(Permissions.AnalysisTechnique.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<AnalysisTechniqueMaster>> GetAnalysisTechnique(long id)
        {
            var entity = await _service.GetAnalysisTechniqueDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [RequirePermission(Permissions.AnalysisTechnique.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutAnalysisTechnique(AnalysisTechniqueMaster model)
        {
            await _service.ModifyAnalysisTechnique(model);
            return Ok(new { status = "success", message = $"Analysis Technique '{model.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.AnalysisTechnique.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<AnalysisTechniqueMaster>> PostAnalysisTechnique(AnalysisTechniqueMaster model)
        {
            await _service.CreateAnalysisTechnique(model);
            return Ok(new { status = "success", message = $"Analysis Technique '{model.Name}' created successfully." });
        }

        [RequirePermission(Permissions.AnalysisTechnique.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAnalysisTechnique(long id)
        {
            var entity = await _service.GetAnalysisTechniqueDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Analysis Technique not found!");
            }
            await _service.RemoveAnalysisTechnique(id);
            return Ok(new { status = "success", message = $"Analysis Technique '{entity.Name}' deleted successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetAnalysisTechniqueDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _service.GetAnalysisTechniqueDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
