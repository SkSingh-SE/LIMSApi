using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/lab-test-analysistype")]
    [ApiController]
    public class LaboratoryTestAnalysisTypeController : ControllerBase
    {
        private readonly ILaboratoryTestAnalysisTypeService _service;

        public LaboratoryTestAnalysisTypeController(ILaboratoryTestAnalysisTypeService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.LaboratoryTestSubType.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> List(PageFilter filter)
        {
            return Ok(await _service.FetchList(filter));
        }

        [RequirePermission(Permissions.LaboratoryTestSubType.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<LaboratoryTestAnalysisType>> GetDetails(long id)
        {
            var entity = await _service.GetDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [RequirePermission(Permissions.LaboratoryTestSubType.Read)]
        [HttpGet("by-subgroup/{subGroupId}")]
        public async Task<IActionResult> GetBySubGroup(long subGroupId)
        {
            var list = await _service.GetBySubGroupId(subGroupId);
            return list == null ? NoContent() : Ok(list);
        }

        [RequirePermission(Permissions.LaboratoryTestSubType.Read)]
        [HttpGet("dropdown/{subGroupId}")]
        public async Task<IActionResult> Dropdown(long subGroupId)
        {
            var data = await _service.GetDropdown(subGroupId);
            return data == null ? NoContent() : Ok(data);
        }

        [RequirePermission(Permissions.LaboratoryTestSubType.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<LaboratoryTestAnalysisType>> Create(LaboratoryTestAnalysisType model)
        {
            await _service.Create(model);
            return Ok(new { status = "success", message = $"Analysis Type '{model.Name}' created successfully.", id = model.ID });
        }

        [RequirePermission(Permissions.LaboratoryTestSubType.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> Update(LaboratoryTestAnalysisType model)
        {
            await _service.Modify(model);
            return Ok(new { status = "success", message = $"Analysis Type '{model.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.LaboratoryTestSubType.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _service.GetDetails(id);
            if (entity == null)
                throw new InvalidOperationException("Analysis Type not found!");

            await _service.Remove(id);
            return Ok(new { status = "success", message = $"Analysis Type '{entity.Name}' deleted successfully." });
        }
    }
}
