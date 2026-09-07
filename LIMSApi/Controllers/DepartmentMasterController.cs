using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Models;
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
        public async Task<IActionResult> DepartmentList([FromBody] PageFilter filter)
        {
            return Ok(await _departmentService.FetchDepartmentList(filter));
        }

        [RequirePermission(Permissions.Department.Read)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<DepartmentDetailDto>> GetDepartmentMaster(long id)
        {
            var entity = await _departmentService.GetDepartmentDetails(id);
            return entity == null ? NoContent() : Ok(entity);
        }

        [RequirePermission(Permissions.Department.Update)]
        [HttpPut("update")]
        public async Task<IActionResult> PutDepartmentMaster([FromBody] DepartmentUpdateDto model)
        {
            await _departmentService.ModifyDepartment(model);
            return Ok(new
            {
                status = "success",
                message = $"Department '{model.Name}' updated successfully."
            });
        }

        [RequirePermission(Permissions.Department.Create)]
        [HttpPost("create")]
        public async Task<IActionResult> PostDepartmentMaster([FromBody] DepartmentCreateDto model)
        {
            await _departmentService.CreateDepartment(model);
            return Ok(new
            {
                status = "success",
                message = $"Department '{model.Name}' created successfully."
            });
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
            return Ok(new
            {
                status = "success",
                message = $"Department '{entity.Name}' deactivated successfully."
            });
        }

        [RequirePermission(Permissions.Department.Update)]
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleDepartmentStatus(long id)
        {
            var entity = await _departmentService.GetDepartmentDetails(id);
            if (entity == null)
            {
                throw new InvalidOperationException("Department not found!");
            }
            bool newStatus = await _departmentService.ToggleDepartmentStatus(id);
            return Ok(new
            {
                status = "success",
                isActive = newStatus,
                message = newStatus
                    ? $"Department '{entity.Name}' activated successfully."
                    : $"Department '{entity.Name}' deactivated successfully."
            });
        }

        [RequirePermission(Permissions.Department.Read)]
        [HttpGet("branch-disciplines/{branchId}")]
        public async Task<IActionResult> GetBranchDisciplines(long branchId)
        {
            var data = await _departmentService.GetBranchDisciplines(branchId);
            return Ok(data);
        }

        [RequirePermission(Permissions.Department.Read)]
        [HttpGet("branches")]
        public async Task<IActionResult> GetAuthorizedBranches()
        {
            var data = await _departmentService.GetAuthorizedBranches();
            return Ok(data);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDepartmentDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _departmentService.GetDepartmentDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
