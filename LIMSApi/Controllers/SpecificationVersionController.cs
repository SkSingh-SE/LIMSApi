using System;
using System.Threading.Tasks;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SpecificationVersionController : ControllerBase
    {
        private readonly ISpecificationVersionService _versionService;
        private readonly ISpecificationMasterService _specificationMasterService;
        private readonly ILogger<SpecificationVersionController> _logger;

        public SpecificationVersionController(
            ISpecificationVersionService versionService,
            ISpecificationMasterService specificationMasterService,
            ILogger<SpecificationVersionController> logger)
        {
            _versionService = versionService;
            _specificationMasterService = specificationMasterService;
            _logger = logger;
        }

        [HttpPost("list")]
        [RequirePermission(Permissions.SpecificationMaster.Read)]
        public async Task<IActionResult> GetList([FromBody] SpecificationVersionFilterDto filter)
        {
            var response = await _versionService.GetPagedVersionsAsync(filter);
            return Ok(response);
        }

        [HttpGet("details/{id}")]
        [RequirePermission(Permissions.SpecificationMaster.Read)]
        public async Task<IActionResult> GetDetails(long id)
        {
            var result = await _versionService.GetVersionDetailsAsync(id);
            return Ok(result);
        }

        [HttpPost("create")]
        [RequirePermission(Permissions.SpecificationMaster.Create)]
        public async Task<IActionResult> Create([FromForm] SpecificationVersionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _versionService.CreateVersionAsync(dto);
            return StatusCode(201, created);
        }

        [HttpPut("update/{id}")]
        [RequirePermission(Permissions.SpecificationMaster.Update)]
        public async Task<IActionResult> Update(long id, [FromForm] SpecificationVersionUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _versionService.UpdateVersionAsync(id, dto);
            return Ok(updated);
        }

        [HttpPut("activate/{id}")]
        [HttpPost("activate/{id}")]
        [RequirePermission(Permissions.SpecificationMaster.Update)]
        public async Task<IActionResult> Activate(long id)
        {
            var success = await _versionService.ActivateVersionAsync(id);
            return Ok(new { success, message = "Specification version activated successfully." });
        }

        [HttpPut("supersede/{id}")]
        [HttpPost("supersede/{id}")]
        [RequirePermission(Permissions.SpecificationMaster.Update)]
        public async Task<IActionResult> Supersede(long id, [FromQuery] DateTime? supersededDate = null)
        {
            var success = await _versionService.SupersedeVersionAsync(id, supersededDate);
            return Ok(new { success, message = "Specification version superseded successfully." });
        }

        [HttpPut("withdraw/{id}")]
        [HttpPost("withdraw/{id}")]
        [RequirePermission(Permissions.SpecificationMaster.Update)]
        public async Task<IActionResult> Withdraw(long id)
        {
            var success = await _versionService.WithdrawVersionAsync(id);
            return Ok(new { success, message = "Specification version withdrawn successfully." });
        }

        [HttpPut("set-default/{id}")]
        [HttpPost("set-default/{id}")]
        [RequirePermission(Permissions.SpecificationMaster.Update)]
        public async Task<IActionResult> SetDefault(long id)
        {
            var success = await _versionService.SetDefaultVersionAsync(id);
            return Ok(new { success, message = "Specification version set as default successfully." });
        }

        [HttpGet("parameters/{id}")]
        [RequirePermission(Permissions.SpecificationMaster.Read)]
        public async Task<IActionResult> GetParameters(long id)
        {
            var list = await _versionService.GetVersionParametersAsync(id);
            return Ok(list);
        }

        [HttpPost("parameters/{id}")]
        [RequirePermission(Permissions.SpecificationMaster.Update)]
        public async Task<IActionResult> SaveParameters(long id, [FromBody] SaveSpecificationVersionParametersDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _versionService.SaveVersionParametersAsync(id, dto);
            return Ok(new { success, message = "Specification version parameters mapped successfully." });
        }

        [HttpGet("dropdown/{specId}")]
        [RequirePermission(Permissions.SpecificationMaster.Read)]
        public async Task<IActionResult> GetDropdown(long specId, [FromQuery] bool includeAll = false)
        {
            var list = await _versionService.GetDropdownBySpecificationIdAsync(specId, includeAll);
            return Ok(list);
        }

        [HttpGet("specifications")]
        public async Task<IActionResult> GetSpecifications([FromQuery] string? search = null)
        {
            var list = await _specificationMasterService.GetSpecificationMasterDropdown(search, 0, 100);
            return Ok(list);
        }

        [HttpDelete("delete/{id}")]
        [HttpDelete("{id}")]
        [RequirePermission(Permissions.SpecificationMaster.Delete)]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _versionService.DeleteVersionAsync(id);
            return Ok(new { success, message = "Specification version deleted successfully." });
        }
    }
}
