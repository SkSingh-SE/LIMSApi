using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LabScopeMasterController : ControllerBase
    {
        private readonly ILabScopeService _labScopeService;

        public LabScopeMasterController(ILabScopeService labScopeServce)
        {
            _labScopeService = labScopeServce;
        }

        [HttpPost("list")]
        public async Task<IActionResult> LabScopeList(PageFilter filter)
        {
            return Ok(await _labScopeService.FetchLabScopeList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<LabScopeMaster>> GetLabScopeMaster(long id)
        {
            var entity = await _labScopeService.GetLabScopeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutLabScopeMaster(LabScopeMaster model)
        {
            await _labScopeService.ModifyLabScope(model);
            return Ok(new
            {
                status = "success",
                message = $"LabScope Master updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<LabScopeMaster>> PostLabScopeMaster(LabScopeMaster model)
        {
            await _labScopeService.CreateLabScope(model);
            return Ok(new
            {
                status = "success",
                message = $"LabScope Master created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLabScopeMaster(long id)
        {
            var entity = await _labScopeService.GetLabScopeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("LabScope not found!");
            }
            await _labScopeService.RemoveLabScope(id);
            return Ok(new
            {
                status = "success",
                message = $"LabScope Master deleted successfully."
            });
        }
    }
}
