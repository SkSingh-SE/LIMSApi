using System.Threading.Tasks;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [Route("api/TestMethod")]
    [ApiController]
    public class TestMethodMasterController : ControllerBase
    {
        private readonly ITestMethodSpecificationService _service;

        public TestMethodMasterController(ITestMethodSpecificationService service)
        {
            _service = service;
        }

        [RequirePermission(Permissions.TestMethodSpecification.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> TestMethodList(
            [FromBody] PageFilter filter, 
            [FromQuery] string? code = null, 
            [FromQuery] string? name = null, 
            [FromQuery] long? techniqueId = null, 
            [FromQuery] string? status = null)
        {
            var result = await _service.FetchTestMethodList(filter, code, name, techniqueId, status);
            return Ok(result);
        }

        [RequirePermission(Permissions.TestMethodSpecification.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<TestMethodDetailDto>> GetTestMethod(long id)
        {
            var entity = await _service.GetTestMethodDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [RequirePermission(Permissions.TestMethodSpecification.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> PostTestMethod([FromBody] TestMethodCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdId = await _service.CreateTestMethod(dto);
            return StatusCode(201, new
            {
                status = "success",
                id = createdId,
                message = $"Test Method '{dto.Name}' created successfully."
            });
        }

        [RequirePermission(Permissions.TestMethodSpecification.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutTestMethod([FromBody] TestMethodUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.UpdateTestMethod(dto);
            return Ok(new
            {
                status = "success",
                message = $"Test Method '{dto.Name}' updated successfully."
            });
        }

        [RequirePermission(Permissions.TestMethodSpecification.Update)]
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleTestMethodStatus(long id)
        {
            await _service.ToggleTestMethodStatus(id);
            return Ok(new
            {
                status = "success",
                message = "Test Method status toggled successfully."
            });
        }

        [RequirePermission(Permissions.TestMethodSpecification.Read)]
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetActiveTestMethodDropdown()
        {
            var list = await _service.GetActiveTestMethodDropdown();
            return Ok(list);
        }
    }
}
