using LIMSApi.Dtos;
using LIMSApi.Models;
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
        public async Task<IActionResult> PostUserPermission(long userId, List<UserPermissionUpdateDto> updatedPermissions)
        {
            await _UserPermissionService.SaveUserPermissions(userId, updatedPermissions);
            return Ok(new
            {
                status = "success",
                message = $"UserPermission updated successfully."
            });
        }

        [HttpGet("user-permission")]
        public async Task<IActionResult> GetUserPermission(long Id)
        {
            var data = await _UserPermissionService.GetUserPermissions(Id);
            return data == null ? NoContent(): Ok(data);
        }

    }
}
