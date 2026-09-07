using LIMSApi.Dtos;
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
    public class SpecificationMasterController : ControllerBase
    {
        private readonly ISpecificationMasterService _service;

        public SpecificationMasterController(ISpecificationMasterService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.SpecificationMaster.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> GetSpecificationMasterList([FromBody] PageFilter filter)
        {
            return Ok(await _service.FetchSpecificationMasterList(filter));
        }

        [RequirePermission(Permissions.SpecificationMaster.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<SpecificationMasterDetailDto>> GetSpecificationMaster(long id)
        {
            var entity = await _service.GetSpecificationMasterDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [RequirePermission(Permissions.SpecificationMaster.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> PostSpecificationMaster([FromBody] SpecificationMasterCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.CreateSpecificationMaster(dto);
            return Ok(new
            {
                status = "success",
                message = $"Specification '{dto.Name}' ({dto.Code}) created successfully."
            });
        }

        [RequirePermission(Permissions.SpecificationMaster.Update)]
        [HttpPut("update")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutSpecificationMaster([FromBody] SpecificationMasterUpdateDto dto, long? id)
        {
            if (id.HasValue && id.Value > 0 && dto.ID <= 0)
            {
                dto.ID = id.Value;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.ModifySpecificationMaster(dto);
            return Ok(new
            {
                status = "success",
                message = $"Specification '{dto.Name}' ({dto.Code}) updated successfully."
            });
        }

        [RequirePermission(Permissions.SpecificationMaster.Update)]
        [HttpPut("toggle-status/{id}")]
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var isActive = await _service.ToggleSpecificationMasterStatus(id);
            return Ok(new
            {
                status = "success",
                isActive,
                message = $"Specification {(isActive ? "activated" : "deactivated")} successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetSpecificationMasterDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            var data = await _service.GetSpecificationMasterDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("standard-organizations")]
        public async Task<IActionResult> GetStandardOrganizationsDropdown()
        {
            var data = await _service.GetStandardOrganizationsDropdown();
            return Ok(data);
        }
    }
}
