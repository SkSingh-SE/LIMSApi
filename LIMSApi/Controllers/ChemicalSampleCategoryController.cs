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
    public class ChemicalSampleCategoryController : ControllerBase
    {
        private readonly IChemicalSampleCategoryService _service;

        public ChemicalSampleCategoryController(IChemicalSampleCategoryService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.ChemicalSampleCategory.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> List(PageFilter filter)
        {
            return Ok(await _service.FetchList(filter));
        }

        [RequirePermission(Permissions.ChemicalSampleCategory.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<ChemicalSampleCategory>> GetDetails(long id)
        {
            var entity = await _service.GetDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [RequirePermission(Permissions.ChemicalSampleCategory.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> Update(ChemicalSampleCategory model)
        {
            await _service.Modify(model);
            return Ok(new { status = "success", message = $"Chemical sample category '{model.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.ChemicalSampleCategory.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<ChemicalSampleCategory>> Create(ChemicalSampleCategory model)
        {
            await _service.Create(model);
            return Ok(new { status = "success", message = $"Chemical sample category '{model.Name}' created successfully." });
        }

        [RequirePermission(Permissions.ChemicalSampleCategory.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _service.GetDetails(id);
            if (entity == null)
                throw new InvalidOperationException("Chemical sample category not found!");
            await _service.Remove(id);
            return Ok(new { status = "success", message = $"Chemical sample category '{entity.Name}' deleted successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> Dropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _service.GetDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
