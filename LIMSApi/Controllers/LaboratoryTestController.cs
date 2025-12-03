using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LaboratoryTestController : ControllerBase
    {
        private readonly ILaboratoryTestService _testMethodService;

        public LaboratoryTestController(ILaboratoryTestService testMethodService)
        {
            _testMethodService = testMethodService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> TestMethodList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchTestMethodList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<LaboratoryTest>> GetTestMethodMaster(long id)
        {
            var entity = await _testMethodService.GetTestMethodDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutTestMethodMaster(LaboratoryTest model)
        {
            await _testMethodService.ModifyTestMethod(model);
            return Ok(new
            {
                status = "success",
                message = $"TestMethod '{model.Name}' updated successfully."
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<LaboratoryTest>> PostTestMethodMaster(LaboratoryTest model)
        {
            await _testMethodService.CreateTestMethod(model);
            return Ok(new
            {
                status = "success",
                message = $"TestMethod '{model.Name}' created successfully."
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTestMethodMaster(long id)
        {
            var entity = await _testMethodService.GetTestMethodDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("TestMethod not found!");
            }
            await _testMethodService.RemoveTestMethod(id);
            return Ok(new
            {
                status = "success",
                message = $"TestMethod '{entity.Name}' updated successfully."
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetTestMethodDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetTestMethodDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }
        [HttpGet("test-cases/{testMethodId}")]
        public async Task<IActionResult> GetTestCases(long testMethodId)
        {
            var data = await _testMethodService.GetTestCases(testMethodId);
            return data == null ? NoContent() : Ok(data);

        }

    }
}
