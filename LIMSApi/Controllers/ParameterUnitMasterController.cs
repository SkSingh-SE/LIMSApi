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
    public class ParameterUnitMasterController : ControllerBase
    {
        private readonly IParameterUnitService _ParameterUnitService;

        public ParameterUnitMasterController(IParameterUnitService ParameterUnitService)
        {
            _ParameterUnitService = ParameterUnitService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> ParameterUnitList(PageFilter filter)
        {
            return Ok(await _ParameterUnitService.FetchParameterUnitList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<ParameterUnitMaster>> GetParameterUnitMaster(long id)
        {
            var entity = await _ParameterUnitService.GetParameterUnitDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutParameterUnitMaster(ParameterUnitMaster model)
        {
            await _ParameterUnitService.ModifyParameterUnit(model);
            return Ok(new
            {
                status = "success",
                message = $"ParameterUnit '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<ParameterUnitMaster>> PostParameterUnitMaster(ParameterUnitMaster model)
        {
            await _ParameterUnitService.CreateParameterUnit(model);
            return Ok(new
            {
                status = "success",
                message = $"ParameterUnit '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteParameterUnitMaster(long id)
        {
            var entity = await _ParameterUnitService.GetParameterUnitDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("ParameterUnit not found!");
            }
            await _ParameterUnitService.RemoveParameterUnit(id);
            return Ok(new
            {
                status = "success",
                message = $"ParameterUnit '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetParameterUnitDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _ParameterUnitService.GetParameterUnitDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
