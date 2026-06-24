using System.Security.Claims;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Models;
using LIMSApi.Services;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPermissionController : ControllerBase
    {
        private readonly IUserPermissionService _UserPermissionService;
        private readonly LIMSContext _context;

        public UserPermissionController(IUserPermissionService UserPermissionService, LIMSContext context)
        {
            _UserPermissionService = UserPermissionService;
            _context = context;
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
            var callerIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(callerIdStr, out var callerId))
                return Unauthorized();

            // Allow own menu access; other users require ReadUserPermission or Admin
            if (userId != callerId)
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                bool isAdmin = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
                if (!isAdmin)
                {
                    var hasPermission = await _context.UserPermissions
                        .Include(up => up.Permission)
                        .AnyAsync(up => up.UserID == callerId && up.IsGranted &&
                                        up.Permission != null &&
                                        up.Permission.Name == Permissions.Admin.ReadUserPermission);

                    if (!hasPermission)
                        return StatusCode(403, new { success = false, message = "You do not have permission to view another user's menu." });
                }
            }

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
