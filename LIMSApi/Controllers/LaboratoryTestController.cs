using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
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

        [RequirePermission(Permissions.LaboratoryTest.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> TestMethodList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchTestMethodList(filter));
        }


        [RequirePermission(Permissions.LaboratoryTest.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<LaboratoryTest>> GetTestMethodMaster(long id)
        {
            var entity = await _testMethodService.GetTestMethodDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.LaboratoryTest.Update)]
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

        [RequirePermission(Permissions.LaboratoryTest.Create)]
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

        [RequirePermission(Permissions.LaboratoryTest.Delete)]
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
                message = $"TestMethod '{entity.Name}' deleted successfully."
            });
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

    }
}
