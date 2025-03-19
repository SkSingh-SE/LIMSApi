using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestGroupController : ControllerBase
    {
        private readonly ITestGroupService _testMethodService;

        public TestGroupController(ITestGroupService testMethodService)
        {
            _testMethodService = testMethodService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> TestGroupList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchTestGroupList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<TestGroup>> GetTestGroupMaster(long id)
        {
            var entity = await _testMethodService.GetTestGroupDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutTestGroupMaster(TestGroup model)
        {
            await _testMethodService.ModifyTestGroup(model);
            return Ok($"TestGroup '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<TestGroup>> PostTestGroupMaster(TestGroup model)
        {
            await _testMethodService.CreateTestGroup(model);
            return Ok($"TestGroup '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTestGroupMaster(long id)
        {
            var entity = await _testMethodService.GetTestGroupDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("TestGroup not found!");
            }
            return Ok($"TestGroup '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetTestGroupDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetTestGroupDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
