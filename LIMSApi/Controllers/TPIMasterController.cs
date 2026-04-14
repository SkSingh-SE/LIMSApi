using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TPIMasterController : ControllerBase
    {
        private readonly ITPIMasterService _testMethodService;

        public TPIMasterController(ITPIMasterService testMethodService)
        {
            _testMethodService = testMethodService;
        }

        [RequirePermission(Permissions.TPI.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> TPIMasterList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchTPIList(filter));
        }


        [RequirePermission(Permissions.TPI.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<TPIMaster>> GetTPIMaster(long id)
        {
            var entity = await _testMethodService.GetTPIDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.TPI.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutTPIMaster(TPIMaster model)
        {
            await _testMethodService.ModifyTPI(model);
            return Ok(new { status = "success", message = $"TPIMaster '{model.AgencyName}' updated successfully." });
        }

        [RequirePermission(Permissions.TPI.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<TPIMaster>> PostTPIMaster(TPIMaster model)
        {
            await _testMethodService.CreateTPI(model);
            return Ok(new { status = "success", message = $"TPIMaster '{model.AgencyName}' created successfully." });
        }

        [RequirePermission(Permissions.TPI.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTPIMaster(long id)
        {
            await _testMethodService.RemoveTPI(id);
            return Ok(new { message = "TPI deleted successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetTPIMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetTPIDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
