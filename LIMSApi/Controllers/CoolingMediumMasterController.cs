using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoolingMediumMasterController : ControllerBase
    {
        private readonly ICoolingMediumService _coolingMediumService;

        public CoolingMediumMasterController(ICoolingMediumService coolingMediumService)
        {
            _coolingMediumService = coolingMediumService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> CoolingMediumList(PageFilter filter)
        {
            return Ok(await _coolingMediumService.FetchCoolingMediumList(filter));
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<CoolingMediumMaster>> GetCoolingMediumMaster(long id)
        {
            var entity = await _coolingMediumService.GetCoolingMediumDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPut("update")]
        public async Task<IActionResult> PutCoolingMediumMaster(CoolingMediumMaster model)
        {
            await _coolingMediumService.ModifyCoolingMedium(model);
            return Ok(new
            {
                status = "success",
                message = $"CoolingMedium '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<CoolingMediumMaster>> PostCoolingMediumMaster(CoolingMediumMaster model)
        {
            await _coolingMediumService.CreateCoolingMedium(model);
            return Ok(new
            {
                status = "success",
                message = $"CoolingMedium '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCoolingMediumMaster(long id)
        {
            var entity = await _coolingMediumService.GetCoolingMediumDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("CoolingMedium not found!");
            }
            await _coolingMediumService.RemoveCoolingMedium(id);
            return Ok(new
            {
                status = "success",
                message = $"CoolingMedium '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCoolingMediumDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _coolingMediumService.GetCoolingMediumDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
