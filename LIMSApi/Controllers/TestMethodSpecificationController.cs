using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestMethodSpecificationController : ControllerBase
    {
        private readonly ITestMethodSpecificationService _testMethodService;

        public TestMethodSpecificationController(ITestMethodSpecificationService testMethodService)
        {
            _testMethodService = testMethodService;
        }

        [RequirePermission(Permissions.TestMethodSpecification.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> TestMethodSpecificationList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchTestMethodSpecificationList(filter));
        }


        [RequirePermission(Permissions.TestMethodSpecification.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<TestMethodSpecification>> GetTestMethodSpecificationMaster(long id)
        {
            var entity = await _testMethodService.GetTestMethodSpecificationDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.TestMethodSpecification.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutTestMethodSpecificationMaster([FromForm] TestMethodSpecificationDto model)
        {

            var versions = JsonConvert.DeserializeObject<List<VersionDto>>(model.Versions);
            var uploadedFiles = Request.Form.Files;
            TestMethodSpecification testMethodSpecification = new TestMethodSpecification
            {
                ID = model.ID,
                Name = model.Name,
                Part = model.Part,
                StandardOrganizationID = model.StandardOrganizationID,
                TestMethodStandard = model.TestMethodStandard,
                IsDisabled = model.IsDisabled,

            };
            if (versions != null && versions.Any())
            {
                testMethodSpecification.Versions = versions.Select(v => new TestMethodSpecificationVersion
                {
                    ID = v.ID,
                    Version = v.Version,
                    StandardFile = v.StandardFile,
                    StandardFilePath = v.StandardFilePath,
                    Status = (VersionStatus)v.Status,
                    Year = v.Year,
                    EffectiveDate = v.EffectiveDate,
                    SupersededDate = v.SupersededDate,
                    ReviewDate = v.ReviewDate,
                    ChangeReason = v.ChangeReason,
                    UploadReferenceID = v.UploadReferenceID,
                    file = uploadedFiles.FirstOrDefault(f => f.FileName == v.StandardFile)
                }).ToList();
            }

            await _testMethodService.ModifyTestMethodSpecification(testMethodSpecification);
            return Ok(new
            {
                status = "success",
                message = $"TestMethodSpecification '{model.Name}' updated successfully."
            });
        }

        [RequirePermission(Permissions.TestMethodSpecification.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<TestMethodSpecification>> PostTestMethodSpecificationMaster([FromForm] TestMethodSpecificationDto model)
        {
            var versions = JsonConvert.DeserializeObject<List<VersionDto>>(model.Versions);
            var uploadedFiles = Request.Form.Files;
            TestMethodSpecification testMethodSpecification = new TestMethodSpecification
            {
                ID = model.ID,
                Name = model.Name,
                Part = model.Part,
                StandardOrganizationID = model.StandardOrganizationID,
                TestMethodStandard = model.TestMethodStandard,
                IsDisabled = model.IsDisabled,

            };
            if (versions != null && versions.Any())
            {
                testMethodSpecification.Versions = versions.Select(v => new TestMethodSpecificationVersion
                {
                    ID = v.ID,
                    Version = v.Version,
                    StandardFile = v.StandardFile,
                    StandardFilePath = v.StandardFilePath,
                    Status = (VersionStatus)v.Status,
                    Year = v.Year,
                    EffectiveDate = v.EffectiveDate,
                    SupersededDate = v.SupersededDate,
                    ReviewDate = v.ReviewDate,
                    ChangeReason = v.ChangeReason,
                    UploadReferenceID = v.UploadReferenceID,
                    file = uploadedFiles.FirstOrDefault(f => f.FileName == v.StandardFile)
                }).ToList();
            }

            await _testMethodService.CreateTestMethodSpecification(testMethodSpecification);
            return Ok(new
            {
                status = "success",
                message = $"TestMethodSpecification '{model.Name}' created successfully."
            });
        }

        [RequirePermission(Permissions.TestMethodSpecification.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTestMethodSpecificationMaster(long id)
        {
            var entity = await _testMethodService.GetTestMethodSpecificationDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("TestMethodSpecification not found!");
            }
            await _testMethodService.RemoveTestMethodSpecification(id);
            return Ok(new
            {
                status = "success",
                message = $"TestMethodSpecification '{entity.Name}' deleted successfully."
            });
        }


        [HttpDelete("enable-disable/{id}")]
        public async Task<IActionResult> EnableDisableTestMethodSpecificationMaster(long id)
        {
            var entity = await _testMethodService.GetTestMethodSpecificationDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("TestMethodSpecification not found!");
            }
            await _testMethodService.EnableDisableTestMethodSpecification(id);
            return Ok(new
            {
                status = "success",
                message = $"TestMethodSpecification '{entity.Name}' {(entity.IsDisabled ? "enabled" : "disabled")} successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetTestMethodSpecificationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetTestMethodSpecificationDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("test-method-standard-wise/{standardId}")]
        public async Task<IActionResult> GetTestMethodsByStandard(long standardId)
        {
            var data = await _testMethodService.GetTestMethodsByStandard(standardId);
            return data == null ? NoContent() : Ok(data);
        }

        [HttpPost("activate-version")]
        public async Task<IActionResult> ActivateVersion([FromBody] VersionActionDto dto)
        {
            await _testMethodService.ActivateVersion(dto.SpecificationId, dto.VersionId);
            return Ok(new { status = "success", message = "Version activated successfully." });
        }

        [HttpPost("withdraw-version")]
        public async Task<IActionResult> WithdrawVersion([FromBody] VersionWithdrawDto dto)
        {
            await _testMethodService.WithdrawVersion(dto.SpecificationId, dto.VersionId, dto.Reason);
            return Ok(new { status = "success", message = "Version withdrawn successfully." });
        }

        [HttpGet("version-impact/{versionId}")]
        public async Task<IActionResult> GetVersionImpact(long versionId)
        {
            var count = await _testMethodService.GetVersionImpactCount(versionId);
            return Ok(new { linkedRecords = count });
        }

        [HttpGet("{specId}/versions/dropdown")]
        public async Task<IActionResult> GetActiveVersionsDropdown(long specId)
        {
            var data = await _testMethodService.GetActiveVersionsBySpecId(specId);
            return Ok(data);
        }
    }
}
