using LIMSApi.Dtos;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestMethodVersionController : ControllerBase
    {
        private readonly ITestMethodVersionService _versionService;
        private readonly ILogger<TestMethodVersionController> _logger;

        public TestMethodVersionController(
            ITestMethodVersionService versionService,
            ILogger<TestMethodVersionController> logger)
        {
            _versionService = versionService;
            _logger = logger;
        }

        [HttpPost("list")]
        [RequirePermission("CanReadTestMethodSpecification")]
        public async Task<IActionResult> GetList([FromBody] TestMethodVersionFilterDto filter)
        {
            var response = await _versionService.GetPagedVersionsAsync(filter);
            return Ok(response);
        }

        [HttpGet("details/{id}")]
        [RequirePermission("CanReadTestMethodSpecification")]
        public async Task<IActionResult> GetDetails(long id)
        {
            var result = await _versionService.GetVersionDetailsAsync(id);
            return Ok(result);
        }

        [HttpPost("create")]
        [RequirePermission("CanAddTestMethodSpecification")]
        public async Task<IActionResult> Create([FromForm] TestMethodVersionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _versionService.CreateVersionAsync(dto);
            return StatusCode(201, created);
        }

        [HttpPut("update/{id}")]
        [RequirePermission("CanEditTestMethodSpecification")]
        public async Task<IActionResult> Update(long id, [FromForm] TestMethodVersionUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _versionService.UpdateVersionAsync(id, dto);
            return Ok(updated);
        }

        [HttpPost("set-default/{id}")]
        [RequirePermission("CanEditTestMethodSpecification")]
        public async Task<IActionResult> SetDefault(long id)
        {
            var success = await _versionService.SetDefaultVersionAsync(id);
            return Ok(new { success, message = "Version set as default successfully." });
        }

        [HttpGet("parameters/{id}")]
        [RequirePermission("CanReadTestMethodSpecification")]
        public async Task<IActionResult> GetParameters(long id)
        {
            var list = await _versionService.GetVersionParametersAsync(id);
            return Ok(list);
        }

        [HttpPost("parameters/{id}")]
        [RequirePermission("CanEditTestMethodSpecification")]
        public async Task<IActionResult> SaveParameters(long id, [FromBody] SaveVersionParametersDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _versionService.SaveVersionParametersAsync(id, dto);
            return Ok(new { success, message = "Parameters mapped successfully." });
        }

        [HttpGet("dropdown/{methodId}")]
        [RequirePermission("CanReadTestMethodSpecification")]
        public async Task<IActionResult> GetDropdown(long methodId)
        {
            var list = await _versionService.GetDropdownByMethodIdAsync(methodId);
            return Ok(list);
        }
    }
}
