using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/lab-test-subgroup")]
    [ApiController]
    public class LaboratoryTestSubGroupController : ControllerBase
    {
        private readonly ILaboratoryTestSubGroupService _service;

        public LaboratoryTestSubGroupController(ILaboratoryTestSubGroupService service)
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
        public async Task<ActionResult<LaboratoryTestSubGroup>> GetDetails(long id)
        {
            var entity = await _service.GetDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [RequirePermission(Permissions.LaboratoryTestSubType.Read)]
        [HttpGet("by-test/{labTestId}")]
        public async Task<IActionResult> GetByLabTest(long labTestId)
        {
            var list = await _service.GetByLabTestId(labTestId);
            return list == null ? NoContent() : Ok(list);
        }

        [RequirePermission(Permissions.LaboratoryTestSubType.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<LaboratoryTestSubGroup>> Create(LaboratoryTestSubGroup model)
        {
            await _service.Create(model);
            return Ok(new { status = "success", message = $"Sub-Group '{model.Name}' created successfully.", id = model.ID });
        }

        [RequirePermission(Permissions.LaboratoryTestSubType.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> Update(LaboratoryTestSubGroup model)
        {
            await _service.Modify(model);
            return Ok(new { status = "success", message = $"Sub-Group '{model.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.LaboratoryTestSubType.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _service.GetDetails(id);
            if (entity == null)
                throw new InvalidOperationException("Sub-Group not found!");

            await _service.Remove(id);
            return Ok(new { status = "success", message = $"Sub-Group '{entity.Name}' deleted successfully." });
        }
    }
}
