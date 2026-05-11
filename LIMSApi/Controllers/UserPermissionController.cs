using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Models;
using LIMSApi.Services;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPermissionController : ControllerBase
    {
        private readonly IUserPermissionService _UserPermissionService;

        public UserPermissionController(IUserPermissionService UserPermissionService)
        {
            _UserPermissionService = UserPermissionService;
        }

        

        [HttpPost("update")]
        [RequirePermission(Permissions.User.AssignPermission)]
        public async Task<IActionResult> PostUserPermission(long userId, List<UserPermissionUpdateDto> updatedPermissions)
        {
            await _UserPermissionService.SaveUserPermissions(userId, updatedPermissions);
            return Ok(new
            {
                status = "success", 
                message = $"UserPermission updated successfully."
            });
        }

        [HttpGet("user-permission/{userId}")]
        [RequirePermission(Permissions.Admin.ReadUserPermission)]
        public async Task<IActionResult> GetUserPermission(long userId)
        {
            var data = await _UserPermissionService.GetUserPermissions(userId);
            return data == null ? NoContent(): Ok(data);
        }
        [HttpGet("user-menu/{userId}")]
        public async Task<IActionResult> GetUserMenuWithPermission(long userId)
        {
            var data = await _UserPermissionService.GetUserMenusWithPermissions(userId);
            return data == null ? NoContent(): Ok(data);
        }

        [HttpGet("all-permission")]
        [RequirePermission(Permissions.Admin.ReadUserPermission)]
        public async Task<IActionResult> GetAllPermissions()
        {
            var data = await _UserPermissionService.GetAllPermissions();
            return data == null ? NoContent() : Ok(data);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetPermissionDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var data = await _UserPermissionService.GetPermissionDropdown(searchTerm, pageNo, pageSize);
            return data == null ? NoContent() : Ok(data);
        }
    }
}
