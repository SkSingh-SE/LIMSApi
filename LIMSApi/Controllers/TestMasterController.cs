using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestMasterController : ControllerBase
    {
        private readonly ITestMasterService _testMasterService;

        public TestMasterController(ITestMasterService testMasterService)
        {
            _testMasterService = testMasterService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> TestMasterList(PageFilter filter)
        {
            return Ok(await _testMasterService.FetchTestMasterList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<TestMaster>> GetTestMasterMaster(long id)
        {
            var entity = await _testMasterService.GetTestMasterDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutTestMasterMaster(TestMaster model)
        {
            await _testMasterService.ModifyTestMaster(model);
            return Ok($"TestMaster '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<TestMaster>> PostTestMasterMaster(TestMaster model)
        {
            await _testMasterService.CreateTestMaster(model);
            return Ok($"TestMaster '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTestMasterMaster(long id)
        {
            var entity = await _testMasterService.GetTestMasterDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("TestMaster not found!");
            }
            return Ok($"TestMaster '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetTestMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMasterService.GetTestMasterDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
