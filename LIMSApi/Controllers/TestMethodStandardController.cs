using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestMethodStandardController : ControllerBase
    {
        private readonly ITestMethodStandardService _testMethodService;

        public TestMethodStandardController(ITestMethodStandardService testMethodService)
        {
            _testMethodService = testMethodService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> TestMethodStandardList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchTestMethodStandardList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<TestMethodStandard>> GetTestMethodStandardMaster(long id)
        {
            var entity = await _testMethodService.GetTestMethodStandardDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutTestMethodStandardMaster(TestMethodStandard model)
        {
            await _testMethodService.ModifyTestMethodStandard(model);
            return Ok($"TestMethodStandard '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<TestMethodStandard>> PostTestMethodStandardMaster(TestMethodStandard model)
        {
            await _testMethodService.CreateTestMethodStandard(model);
            return Ok($"TestMethodStandard '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTestMethodStandardMaster(long id)
        {
            var entity = await _testMethodService.GetTestMethodStandardDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("TestMethodStandard not found!");
            }
            return Ok($"TestMethodStandard '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetTestMethodStandardDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetTestMethodStandardDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
