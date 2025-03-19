using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestMethodMasterController : ControllerBase
    {
        private readonly ITestMethodService _testMethodService;

        public TestMethodMasterController(ITestMethodService testMethodService)
        {
            _testMethodService = testMethodService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> TestMethodList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchTestMethodList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<TestMethodMaster>> GetTestMethodMaster(long id)
        {
            var entity = await _testMethodService.GetTestMethodDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutTestMethodMaster(TestMethodMaster model)
        {
            await _testMethodService.ModifyTestMethod(model);
            return Ok($"TestMethod '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<TestMethodMaster>> PostTestMethodMaster(TestMethodMaster model)
        {
            await _testMethodService.CreateTestMethod(model);
            return Ok($"TestMethod '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTestMethodMaster(long id)
        {
            var entity = await _testMethodService.GetTestMethodDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("TestMethod not found!");
            }
            return Ok($"TestMethod '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetTestMethodDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetTestMethodDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
