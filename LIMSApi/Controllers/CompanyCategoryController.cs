using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
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

        [RequirePermission(Permissions.CompanyCategory.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> CustomerTypeList(PageFilter filter)
        {
            return Ok(await _testMethodService.FetchCustomerTypeList(filter));
        }


        [RequirePermission(Permissions.CompanyCategory.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<CompanyCategoryMaster>> GetCustomerTypeMaster(long id)
        {
            var entity = await _testMethodService.GetCustomerTypeDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.CompanyCategory.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutCustomerTypeMaster(CompanyCategoryMaster model)
        {
            await _testMethodService.ModifyCustomerType(model);
            return Ok(new { message = $"CustomerTypeMaster '{model.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.CompanyCategory.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<CompanyCategoryMaster>> PostCustomerTypeMaster(CompanyCategoryMaster model)
        {
            await _testMethodService.CreateCustomerType(model);
            return Ok(new { message = $"CustomerTypeMaster '{model.Name}' created successfully" });
        }

        [RequirePermission(Permissions.CompanyCategory.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCustomerTypeMaster(long id)
        {
            await _testMethodService.RemoveCustomerType(id);
            return Ok(new { message = "Company Category deleted successfully." });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCustomerTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _testMethodService.GetCustomerTypeDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
