using LIMSApi.Dtos;
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

        [HttpPost("list")]
        public async Task<IActionResult> TestMethodSpecificationList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchTestMethodSpecificationList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<TestMethodSpecification>> GetTestMethodSpecificationMaster(long id)
        {
            var entity = await _testMethodService.GetTestMethodSpecificationDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutTestMethodSpecificationMaster([FromForm] TestMethodSpecification model)
        {
            await _testMethodService.ModifyTestMethodSpecification(model);
            return Ok(new
            {
                status = "success",
                message = $"TestMethodSpecification '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<TestMethodSpecification>> PostTestMethodSpecificationMaster([FromForm] TestMethodSpecificationDto model)
        {
            var versions = JsonConvert.DeserializeObject<List<VersionDto>>(model.Versions);
            var uploadedFiles = Request.Form.Files;
            TestMethodSpecification testMethodSpecification = new TestMethodSpecification
            {
                ID = model.ID,
                Name = model.Name,
                StandardOrganizationID = model.StandardOrganizationID,
                IsDisabled = model.IsDisabled,

            };
            if (versions != null && versions.Any())
            {
                testMethodSpecification.Versions = versions.Select(v => new TestMethodSpecificationVersion
                {
                    ID = v.ID,
                    Version = v.Version,
                    Year = v.Year,
                    StandardFile = v.StandardFile,
                    StandardFilePath = v.StandardFilePath,
                    Default = v.Default,
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

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetTestMethodSpecificationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetTestMethodSpecificationDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }

    }
}
