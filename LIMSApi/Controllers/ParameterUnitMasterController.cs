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
        public async Task<ActionResult<ParameterUnitDetailDto>> GetParameterUnitMaster(long id)
        {
            var entity = await _ParameterUnitService.GetParameterUnitDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [HttpPost("create")]
        public async Task<IActionResult> PostParameterUnitMaster([FromBody] ParameterUnitCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _ParameterUnitService.CreateParameterUnit(dto);
            return Ok(new
            {
                status = "success",
                message = $"Parameter Unit '{dto.Name}' ({dto.Code.ToUpper()}) created successfully."
            });
        }

        [HttpPut("update")]
        public async Task<IActionResult> PutParameterUnitMaster([FromBody] ParameterUnitUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _ParameterUnitService.ModifyParameterUnit(dto);
            return Ok(new
            {
                status = "success",
                message = $"Parameter Unit '{dto.Name}' ({dto.Code.ToUpper()}) updated successfully."
            });
        }

        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var isActive = await _ParameterUnitService.ToggleParameterUnitStatus(id);
            return Ok(new
            {
                status = "success",
                isActive,
                message = $"Parameter Unit {(isActive ? "activated" : "deactivated")} successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteParameterUnitMaster(long id)
        {
            var entity = await _ParameterUnitService.GetParameterUnitDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Parameter Unit not found!");
            }
            await _ParameterUnitService.RemoveParameterUnit(id);
            return Ok(new
            {
                status = "success",
                message = $"Parameter Unit '{entity.Name}' deleted successfully."
            });
        }

        [HttpGet("quantity-types")]
        public async Task<IActionResult> GetQuantityTypes()
        {
            return Ok(await _ParameterUnitService.GetQuantityTypes());
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetParameterUnitDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _ParameterUnitService.GetParameterUnitDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("grouped-dropdown")]
        public async Task<IActionResult> GetGroupedParameterUnitDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _ParameterUnitService.GetGroupedParameterUnitDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        // Equivalent units for a parameter's default unit (base unit + matching SimilarUnit1-7).
        [HttpGet("equivalents/{unitId}")]
        public async Task<IActionResult> GetEquivalentUnits(long unitId)
        {
            return Ok(await _ParameterUnitService.GetEquivalentUnits(unitId));
        }
    }
}
