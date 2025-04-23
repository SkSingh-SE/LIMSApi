using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyCategoryController : ControllerBase
    {
        private readonly ICompanyCategoryService _testMethodService;

        public CompanyCategoryController(ICompanyCategoryService testMethodService)
        {
            _testMethodService = testMethodService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> CustomerTypeList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchCustomerTypeList(filter));
        }


        [HttpGet("details/{id}")]
        public async Task<ActionResult<CompanyCategoryMaster>> GetCustomerTypeMaster(long id)
        {
            var entity = await _testMethodService.GetCustomerTypeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [HttpPut("update")]
        public async Task<IActionResult> PutCustomerTypeMaster(CompanyCategoryMaster model)
        {
            await _testMethodService.ModifyCustomerType(model);
            return Ok(new { message = $"CustomerTypeMaster '{model.Name}' updated successfully." });
        }

        [HttpPost("create")]
        public async Task<ActionResult<CompanyCategoryMaster>> PostCustomerTypeMaster(CompanyCategoryMaster model)
        {
            await _testMethodService.CreateCustomerType(model);
            return Ok(new { message = $"CustomerTypeMaster '{model.Name}' created successfully" });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCustomerTypeMaster(long id)
        {
            var entity = await _testMethodService.GetCustomerTypeDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("CustomerTypeMaster not found!");
            }
            return Ok(new { message = $"CustomerTypeMaster '{entity.Name}' created successfully" });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCustomerTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetCustomerTypeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
