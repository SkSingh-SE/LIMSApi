using LIMSApi.Dtos;
using LIMSApi.Models;
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
    public class LaboratoryTestController : ControllerBase
    {
        private readonly ILaboratoryTestService _testMethodService;
        private readonly ILaboratoryTestLayoutService _layoutService;

        public LaboratoryTestController(ILaboratoryTestService testMethodService, ILaboratoryTestLayoutService layoutService)
        {
            _testMethodService = testMethodService;
            _layoutService = layoutService;
        }

        [RequirePermission(Permissions.LaboratoryTest.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> TestMethodList([FromBody] PageFilter filter)
        {
            return Ok(await _testMethodService.FetchTestMethodList(filter));
        }

        [RequirePermission(Permissions.LaboratoryTest.Read)]
        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedUniversalTests(
            [FromBody] PageFilter filter,
            [FromQuery] long? disciplineId = null,
            [FromQuery] long? departmentId = null,
            [FromQuery] bool? isActive = null)
        {
            var result = await _testMethodService.GetPagedTestsAsync(filter, disciplineId, departmentId, isActive);
            return Ok(result);
        }

        [RequirePermission(Permissions.LaboratoryTest.Read)]
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetUniversalTestDetails(long id)
        {
            var entity = await _testMethodService.GetUniversalTestByIdAsync(id);
            return Ok(entity);
        }

        [RequirePermission(Permissions.LaboratoryTest.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> PostUniversalTest([FromBody] LaboratoryTestCreateDto dto)
        {
            var id = await _testMethodService.CreateUniversalTestAsync(dto);
            return Ok(new
            {
                status = "success",
                message = $"Laboratory Test '{dto.Name}' created successfully.",
                id
            });
        }

        [RequirePermission(Permissions.LaboratoryTest.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutUniversalTest([FromBody] LaboratoryTestUpdateDto dto)
        {
            var id = await _testMethodService.UpdateUniversalTestAsync(dto);
            return Ok(new
            {
                status = "success",
                message = $"Laboratory Test '{dto.Name}' updated successfully.",
                id
            });
        }

        [RequirePermission(Permissions.LaboratoryTest.Update)]
        [HttpPatch("toggle-status/{id}")]
        public async Task<IActionResult> ToggleTestStatus(long id)
        {
            bool isActive = await _testMethodService.ToggleTestStatusAsync(id);
            return Ok(new
            {
                status = "success",
                message = $"Laboratory Test status updated to {(isActive ? "Active" : "Inactive")}.",
                isActive
            });
        }

        [RequirePermission(Permissions.LaboratoryTest.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUniversalTest(long id)
        {
            await _testMethodService.DeleteUniversalTestAsync(id);
            return Ok(new
            {
                status = "success",
                message = "Laboratory Test deleted successfully."
            });
        }

        [HttpGet("universal-dropdown")]
        public async Task<IActionResult> GetUniversalDropdown([FromQuery] long? disciplineId = null)
        {
            var data = await _testMethodService.GetUniversalDropdownAsync(disciplineId);
            return Ok(data);
        }

        [HttpGet("check-code-unique")]
        public async Task<IActionResult> CheckCodeUnique([FromQuery] string code, [FromQuery] long? excludeId = null)
        {
            var isUnique = await _testMethodService.CheckCodeUniqueAsync(code, excludeId);
            return Ok(new { isUnique });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetTestMethodDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetTestMethodDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

        [HttpGet("general-dropdown")]
        public async Task<IActionResult> GetGeneralTestMethodDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetGeneralTestMethodDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("chemical-dropdown")]
        public async Task<IActionResult> GetChemicalTestMethodDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetChemicalTestMethodDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

        [HttpGet("unified-dropdown")]
        public async Task<IActionResult> GetUnifiedTestMethodDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetUnifiedTestMethodDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("distinct-names")]
        public async Task<IActionResult> GetDistinctTestNames(string? searchTerm, int pageSize = 20)
        {
            var data = await _testMethodService.GetDistinctTestNames(searchTerm, pageSize);
            return Ok(data);
        }

        [HttpGet("test-cases/{testMethodId}")]
        public async Task<IActionResult> GetTestCases(long testMethodId)
        {
            var data = await _testMethodService.GetTestCases(testMethodId);
            return data == null ? NoContent() : Ok(data);
        }

        [RequirePermission(Permissions.LaboratoryTest.Create)]
        [HttpPost("duplicate/{id}")]
        public async Task<IActionResult> DuplicateTestMethod(long id)
        {
            var duplicatedId = await _testMethodService.DuplicateLaboratoryTest(id);
            return Ok(new
            {
                status = "success",
                message = "Laboratory Test duplicated successfully.",
                id = duplicatedId
            });
        }

        [RequirePermission(Permissions.LaboratoryTest.Read)]
        [HttpGet("pricing-template/{labTestId}")]
        public async Task<IActionResult> GetPricingTemplate(long labTestId, [FromQuery] long? analysisTypeId = null)
        {
            var data = await _testMethodService.GetPricingTemplate(labTestId, analysisTypeId);
            return Ok(data);
        }

        [HttpGet("test-method-specification/{labTestId}")]
        public async Task<IActionResult> GetTestMethodSpecificationByLabTest(long labTestId)
        {
            var data = await _testMethodService.GetTestMethodSpecificationByLabTestId(labTestId);
            return Ok(data);
        }

        [RequirePermission(Permissions.LaboratoryTest.Read)]
        [HttpGet("{id}/layouts")]
        public async Task<IActionResult> GetLayoutAssignments(long id)
        {
            return Ok(await _layoutService.GetLayoutsAsync(id));
        }

        [RequirePermission(Permissions.LaboratoryTest.Update)]
        [HttpPost("{id}/layouts")]
        public async Task<IActionResult> AddLayoutAssignment(long id, [FromBody] LaboratoryTestLayoutCreateDto dto)
        {
            var created = await _layoutService.AddLayoutAsync(id, dto);
            return Ok(new { status = "success", message = $"Layout '{created.LayoutCode}' assigned successfully.", id = created.ID });
        }

        [RequirePermission(Permissions.LaboratoryTest.Update)]
        [HttpPut("{id}/layouts/{assignmentId}")]
        public async Task<IActionResult> UpdateLayoutAssignment(long id, long assignmentId, [FromBody] LaboratoryTestLayoutUpdateDto dto)
        {
            var updated = await _layoutService.UpdateLayoutAsync(id, assignmentId, dto);
            return Ok(new { status = "success", message = $"Layout assignment '{updated.LayoutCode}' updated successfully.", id = updated.ID });
        }

        [RequirePermission(Permissions.LaboratoryTest.Update)]
        [HttpPut("{id}/layouts/{assignmentId}/toggle-status")]
        public async Task<IActionResult> ToggleLayoutAssignmentStatus(long id, long assignmentId)
        {
            bool isActive = await _layoutService.ToggleLayoutStatusAsync(id, assignmentId);
            return Ok(new { status = "success", message = $"Layout assignment {(isActive ? "activated" : "deactivated")} successfully.", isActive });
        }

        [RequirePermission(Permissions.LaboratoryTest.Read)]
        [HttpGet("{id}/effective-layout")]
        public async Task<IActionResult> GetEffectiveLayout(long id, [FromQuery] long? methodId = null, [FromQuery] long? versionId = null)
        {
            return Ok(await _layoutService.ResolveEffectiveLayoutAsync(id, methodId, versionId));
        }
    }
}
