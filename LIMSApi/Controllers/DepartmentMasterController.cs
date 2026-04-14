using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentMasterController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentMasterController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [RequirePermission(Permissions.Department.Read)]
        [HttpPost("list")]
        public async Task<IActionResult> DepartmentList(PageFilter filter)
        {
            return Ok(await _departmentService.FetchDepartmentList(filter));
        }


        [RequirePermission(Permissions.Department.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<DepartmentMaster>> GetDepartmentMaster(long id)
        {
            var entity = await _departmentService.GetDepartmentDetails(id);

            return entity == null ? NoContent() : Ok(entity);
        }


        [RequirePermission(Permissions.Department.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutDepartmentMaster(DepartmentMaster model)
        {
            await _departmentService.ModifyDepartment(model);
            return Ok( new { message = $"Department '{model.Name}' updated successfully." });
        }

        [RequirePermission(Permissions.Department.Create)]
        [HttpPost("create")]
        public async Task<ActionResult<DepartmentMaster>> PostDepartmentMaster(DepartmentMaster model)
        {
            await _departmentService.CreateDepartment(model);
            return Ok(new { message =  $"Department '{model.Name}' created successfully" });
        }

        [RequirePermission(Permissions.Department.Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDepartmentMaster(long id)
        {
            var entity = await _departmentService.GetDepartmentDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Department not found!");
            }
            await _departmentService.RemoveDepartment(id);
            return Ok( new { message = $"Department '{entity.Name}' deleted successfully" });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDepartmentDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _departmentService.GetDepartmentDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
