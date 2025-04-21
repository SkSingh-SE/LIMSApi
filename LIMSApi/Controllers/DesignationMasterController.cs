using LIMSApi.Dtos;
using LIMSApi.Models;
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

        [HttpPost("list")]
        public async Task<IActionResult> DesignationList(PageFilter filter)
        {
            return Ok(await _designationService.FetchDesignationList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<DesignationMaster>> GetDesignationMaster(long id)
        {
            var entity = await _designationService.GetDesignationDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutDesignationMaster(DesignationMaster model)
        {
            await _designationService.ModifyDesignation(model);
            return Ok(new { message = $"Designation '{model.Name}' updated successfully." });
        }

        [HttpPost("create")]
        public async Task<IActionResult> PostDesignationMaster(DesignationMaster model)
        {
            await _designationService.CreateDesignation(model);
            return Ok(new { message = $"Designation '{model.Name}' created successfully" });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDesignationMaster(long id)
        {
            var entity = await _designationService.GetDesignationDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Designation not found!");
            }
            return Ok(new { message = $"Designation '{entity.Name}' created successfully" });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDesignationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _designationService.GetDesignationDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
