using System;
using System.Threading.Tasks;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SpecificationRequirementController : ControllerBase
    {
        private readonly ISpecificationRequirementService _requirementService;
        private readonly ILogger<SpecificationRequirementController> _logger;

        public SpecificationRequirementController(
            ISpecificationRequirementService requirementService,
            ILogger<SpecificationRequirementController> logger)
        {
            _requirementService = requirementService;
            _logger = logger;
        }

        private static string GetCurrentCompanyCode()
        {
            var user = LoggedInUserProvider.CurrentUser;
            return string.IsNullOrWhiteSpace(user?.CompanyCode) ? "LIMS" : user.CompanyCode;
        }

        private static long GetCurrentUserId()
        {
            return LoggedInUserProvider.CurrentUser?.UserId ?? 1;
        }

        [HttpGet("context")]
        [RequirePermission(Permissions.SpecificationMaster.Read)]
        public async Task<IActionResult> GetContext([FromQuery] long specId, [FromQuery] long versionId, [FromQuery] long gradeId)
        {
            if (specId <= 0 || versionId <= 0 || gradeId <= 0)
            {
                return BadRequest(new { message = "specId, versionId, and gradeId must all be positive integers." });
            }

            var context = await _requirementService.GetContextAsync(specId, versionId, gradeId, GetCurrentCompanyCode());
            return Ok(context);
        }

        [HttpGet("list")]
        [RequirePermission(Permissions.SpecificationMaster.Read)]
        public async Task<IActionResult> GetList([FromQuery] long versionId, [FromQuery] long gradeId)
        {
            if (versionId <= 0 || gradeId <= 0)
            {
                return BadRequest(new { message = "versionId and gradeId must be positive integers." });
            }

            var list = await _requirementService.GetRequirementsAsync(versionId, gradeId, GetCurrentCompanyCode());
            return Ok(list);
        }

        [HttpGet("details/{id}")]
        [RequirePermission(Permissions.SpecificationMaster.Read)]
        public async Task<IActionResult> GetDetails(long id)
        {
            var item = await _requirementService.GetRequirementByIdAsync(id, GetCurrentCompanyCode());
            if (item == null)
            {
                return NotFound(new { message = $"Specification requirement line with ID {id} not found." });
            }
            return Ok(item);
        }

        [HttpPost("create")]
        [RequirePermission(Permissions.SpecificationMaster.Create)]
        public async Task<IActionResult> Create([FromBody] SaveSpecificationRequirementDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = await _requirementService.CreateRequirementAsync(dto, GetCurrentCompanyCode(), GetCurrentUserId());
            return StatusCode(201, new { id, message = "Specification requirement created successfully." });
        }

        [HttpPut("update/{id}")]
        [RequirePermission(Permissions.SpecificationMaster.Update)]
        public async Task<IActionResult> Update(long id, [FromBody] SaveSpecificationRequirementDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _requirementService.UpdateRequirementAsync(id, dto, GetCurrentCompanyCode(), GetCurrentUserId());
            return Ok(new { message = "Specification requirement updated successfully." });
        }

        [HttpDelete("delete/{id}")]
        [RequirePermission(Permissions.SpecificationMaster.Delete)]
        public async Task<IActionResult> Delete(long id)
        {
            await _requirementService.DeleteRequirementAsync(id, GetCurrentCompanyCode());
            return Ok(new { message = "Specification requirement deleted successfully." });
        }

        [HttpPost("copy-version")]
        [RequirePermission(Permissions.SpecificationMaster.Create)]
        public async Task<IActionResult> CopyVersion([FromBody] CopyVersionRequirementsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int count = await _requirementService.CopyVersionRequirementsAsync(dto, GetCurrentCompanyCode(), GetCurrentUserId());
            return Ok(new { count, message = $"Successfully cloned {count} requirements to target version." });
        }

        [HttpPost("activate-version/{versionId}")]
        [RequirePermission(Permissions.SpecificationMaster.Update)]
        public async Task<IActionResult> ActivateVersion(long versionId)
        {
            bool success = await _requirementService.ActivateSpecificationVersionAsync(versionId, GetCurrentCompanyCode(), GetCurrentUserId());
            return Ok(new { success, message = "Specification version activated and previous versions superseded successfully." });
        }

        [HttpGet("conditions")]
        [HttpGet("condition-dimensions")]
        public async Task<IActionResult> GetConditions([FromServices] LIMSApi.Data.LIMSContext context)
        {
            var list = await context.ConditionMasters
                .Where(d => d.IsActive)
                .Include(d => d.ParameterUnit)
                .OrderBy(d => d.DisplayOrder)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    id = d.ID,
                    code = d.Code,
                    name = d.Name,
                    category = d.Category,
                    valueType = d.ValueType,
                    dataType = d.ValueType, // Backward-compat for legacy consumers
                    unit = d.ParameterUnit != null ? d.ParameterUnit.Symbol : null,
                    allowedOperators = d.AllowedOperators,
                    allowedValuesJson = d.AllowedValuesJson,
                    defaultValue = d.DefaultValue
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
