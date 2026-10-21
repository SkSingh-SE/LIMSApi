using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestLayoutTemplateController : ControllerBase
    {
        private readonly ITestLayoutTemplateService _service;

        public TestLayoutTemplateController(ITestLayoutTemplateService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.TestLayout.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> LayoutList(PageFilter filter)
        {
            return Ok(await _service.FetchLayoutTemplateList(filter));
        }

        [RequirePermission(Permissions.TestLayout.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<TestLayoutTemplate>> GetLayout(long id)
        {
            var entity = await _service.GetLayoutTemplateDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpGet("by-hierarchy")]
        public async Task<ActionResult<TestLayoutTemplate>> GetByHierarchy(
            [FromQuery] long? testId,
            [FromQuery] long? subGroupId,
            [FromQuery] long? analysisTypeId,
            [FromQuery] string? leafNode,
            [FromQuery] long? testMethodId,
            [FromQuery] string? testMethodName)
        {
            var entity = await _service.GetLayoutByHierarchy(testId, subGroupId, analysisTypeId, leafNode, testMethodId, testMethodName);
            return entity == null ? NoContent() : Ok(entity);
        }

        [RequirePermission(Permissions.TestLayout.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> PostLayout(TestLayoutTemplate model)
        {
            await _service.CreateLayoutTemplate(model);
            return Ok(new { status = "success", message = $"Layout '{model.LayoutName}' created successfully.", id = model.ID });
        }

        [RequirePermission(Permissions.TestLayout.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutLayout(TestLayoutTemplate model)
        {
            await _service.ModifyLayoutTemplate(model);
            return Ok(new { status = "success", message = $"Layout '{model.LayoutName}' updated successfully.", id = model.ID });
        }

        [RequirePermission(Permissions.TestLayout.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLayout(long id)
        {
            await _service.RemoveLayoutTemplate(id);
            return Ok(new { status = "success", message = "Layout deleted successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown(
            [FromQuery] long? testId,
            [FromQuery] string? searchTerm,
            [FromQuery] int pageNo = 1,
            [FromQuery] int pageSize = 20)
        {
            var data = await _service.GetLayoutTemplateDropdown(testId, searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
