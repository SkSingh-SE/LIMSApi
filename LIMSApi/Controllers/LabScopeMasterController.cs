using LIMSApi.Dtos;
using LIMSApi.Models;
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
    public class LabScopeMasterController : ControllerBase
    {
        private readonly ILabScopeService _labScopeService;

        public LabScopeMasterController(ILabScopeService labScopeServce)
        {
            _labScopeService = labScopeServce;
        }

        [RequirePermission(Permissions.LabScope.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> LabScopeList(PageFilter filter)
        {
            return Ok(await _labScopeService.FetchLabScopeList(filter));
        }


        [RequirePermission(Permissions.LabScope.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<LabScopeMaster>> GetLabScopeMaster(long id)
        {
            var entity = await _labScopeService.GetLabScopeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.LabScope.Update)]
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

        [RequirePermission(Permissions.LabScope.Create)]
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

        [RequirePermission(Permissions.LabScope.Delete)]
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
