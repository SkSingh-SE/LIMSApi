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

        // ==================== Grade Management Endpoints ====================

        [RequirePermission(Permissions.SpecificationMaster.Read)]
        [HttpGet("{specId}/grades")]
        public async Task<ActionResult<List<SpecificationGradeDto>>> GetGrades(long specId, [FromQuery] bool includeInactive = false)
        {
            var grades = await _service.GetGradesBySpecification(specId, includeInactive);
            return Ok(grades);
        }

        [RequirePermission(Permissions.SpecificationMaster.Read)]
        [HttpGet("grades/{gradeId}")]
        public async Task<ActionResult<SpecificationGradeDto>> GetGrade(long gradeId)
        {
            var grade = await _service.GetGradeById(gradeId);
            return Ok(grade);
        }

        [RequirePermission(Permissions.SpecificationMaster.Create)]
        [HttpPost("{specId}/grades")]
        public async Task<IActionResult> CreateGrade(long specId, [FromBody] SpecificationGradeCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = await _service.CreateGrade(specId, dto);
            return Ok(new
            {
                status = "success",
                id,
                message = $"Grade '{dto.Grade}' created successfully."
            });
        }

        [RequirePermission(Permissions.SpecificationMaster.Update)]
        [HttpPut("grades/{gradeId}")]
        public async Task<IActionResult> UpdateGrade(long gradeId, [FromBody] SpecificationGradeUpdateDto dto)
        {
            if (gradeId > 0 && dto.ID <= 0)
            {
                dto.ID = gradeId;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.ModifyGrade(gradeId, dto);
            return Ok(new
            {
                status = "success",
                message = $"Grade '{dto.Grade}' updated successfully."
            });
        }

        [RequirePermission(Permissions.SpecificationMaster.Update)]
        [HttpPut("grades/{gradeId}/toggle-status")]
        [HttpPost("grades/{gradeId}/toggle-status")]
        public async Task<IActionResult> ToggleGradeStatus(long gradeId)
        {
            var isActive = await _service.ToggleGradeStatus(gradeId);
            return Ok(new
            {
                status = "success",
                isActive,
                message = $"Grade {(isActive ? "activated" : "deactivated")} successfully."
            });
        }
    }
}
