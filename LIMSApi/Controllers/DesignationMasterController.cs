using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LIMSApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationMasterController : ControllerBase
    {
        private readonly IDesignationService _designationService;

        public DesignationMasterController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        [RequirePermission(Permissions.Designation.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> DesignationList(PageFilter filter)
        {
            return Ok(await _designationService.FetchDesignationList(filter));
        }


        [RequirePermission(Permissions.Designation.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<DesignationMaster>> GetDesignationMaster(long id)
        {
            var entity = await _designationService.GetDesignationDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.Designation.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutDesignationMaster(DesignationMaster model)
        {
            await _designationService.ModifyDesignation(model);
            return Ok(new { message = $"Designation '{model.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.Designation.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> PostDesignationMaster(DesignationMaster model)
        {
            await _designationService.CreateDesignation(model);
            return Ok(new { message = $"Designation '{model.Name}' created successfully" });
        }

        [RequirePermission(Permissions.Designation.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDesignationMaster(long id)
        {
            var entity = await _designationService.GetDesignationDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Designation not found!");
            }
            await _designationService.RemoveDesignation(id);
            return Ok(new { message = $"Designation '{entity.Name}' deleted successfully" });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDesignationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _designationService.GetDesignationDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
