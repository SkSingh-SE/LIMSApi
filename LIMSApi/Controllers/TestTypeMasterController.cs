using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestTypeMasterController : ControllerBase
    {
        private readonly ITestTypeService _testTypeService;

        public TestTypeMasterController(ITestTypeService testTypeServce)
        {
            _testTypeService = testTypeServce;
        }

        [HttpPost("list")]
        public async Task<IActionResult> TestTypeList(PageFilter filter)
        {
            return Ok(await _testTypeService.FetchTestTypeList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<TestTypeMaster>> GetTestTypeMaster(long id)
        {
            var entity = await _testTypeService.GetTestTypeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutTestTypeMaster(TestTypeMaster model)
        {
            await _testTypeService.ModifyTestType(model);
            return Ok($"TestType '{model.Name}' updated successfully.");
        }

        [HttpPost("create")]
        public async Task<ActionResult<TestTypeMaster>> PostTestTypeMaster(TestTypeMaster model)
        {
            await _testTypeService.CreateTestType(model);
            return Ok($"TestType '{model.Name}' created successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTestTypeMaster(long id)
        {
            var entity = await _testTypeService.GetTestTypeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("TestType not found!");
            }
            return Ok($"TestType '{entity.Name}' created successfully");
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetTestTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testTypeService.GetTestTypeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
