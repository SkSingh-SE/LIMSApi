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
    public class TestGroupController : ControllerBase
    {
        private readonly IUniversalTestGroupService _service;

        public TestGroupController(IUniversalTestGroupService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        [RequirePermission(Permissions.TestGroup.Read)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllForCurrentUserAsync();
            return Ok(result);
        }

        [HttpGet("list/by-inward/{inwardId:long}")]
        [RequirePermission(Permissions.TestGroup.Read)]
        public async Task<IActionResult> GetByInward(long inwardId)
        {
            var result = await _service.GetTestGroupsForInwardAsync(inwardId);
            return Ok(result);
        }

        [HttpGet("list/by-sample/{sampleId:long}")]
        [RequirePermission(Permissions.TestGroup.Read)]
        public async Task<IActionResult> GetBySample(long sampleId)
        {
            var result = await _service.GetTestGroupsForSampleAsync(sampleId);
            return Ok(result);
        }

        [HttpGet("list/by-plan/{planId:long}")]
        [RequirePermission(Permissions.TestGroup.Read)]
        public async Task<IActionResult> GetByPlan(long planId)
        {
            var result = await _service.GetTestGroupsForPlanAsync(planId);
            return Ok(result);
        }

        [HttpGet("details/{id:long}")]
        [RequirePermission(Permissions.TestGroup.Read)]
        public async Task<IActionResult> GetDetails(long id)
        {
            var result = await _service.GetTestGroupDetailAsync(id);
            return Ok(result);
        }

        [HttpGet("effective-configuration/{id:long}")]
        [RequirePermission(Permissions.TestGroup.Read)]
        public async Task<IActionResult> GetEffectiveConfiguration(long id)
        {
            var result = await _service.GetEffectiveConfigurationAsync(id);
            return Ok(result);
        }

        [HttpGet("validation/{id:long}")]
        [RequirePermission(Permissions.TestGroup.Read)]
        public async Task<IActionResult> GetValidation(long id)
        {
            var result = await _service.GetValidationAsync(id);
            return Ok(result);
        }
    }
}
