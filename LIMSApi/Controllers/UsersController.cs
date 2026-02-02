using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIMSApi.Data;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using LIMSApi.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        [HttpGet("getUser/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);

            if (user == null)
            {
                return NotFound($"User not found : {email}");
            }

            return Ok(user);
        }

        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateUser(UserMaster user)
        {
            await _userService.UpdateUser(user);
            return Ok(new
            {
                status = "success",
                message = $"User '{user.UserName}' updated successfully."
            });
        }


        [HttpDelete("deleteUser/{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _userService.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound($"User nit found {email}");
            }
            await _userService.DeleteUser(email);
            return Ok(new
            {
                status = "success",
                message = $"User '{user.UserName}' updated successfully."
            });

        }

        [HttpGet("getUserDropdown")]
        public async Task<IActionResult> GetUserDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;
            if (pageSize <= 0) pageSize = 20;
            var users = await _userService.GetUserDropdown(searchTerm, pageNo, pageSize);
            if (users == null || users.Count == 0)
            {
                return NotFound("No users found for the provided search term.");
            }
            return Ok(users);
        }

        [HttpGet("by-employee/{employeeId}")]
        public async Task<ActionResult<UserAccountDto>> GetByEmployee(long employeeId)
        {
            return await _userService.GetByEmployee(employeeId);
        }

        [HttpPut("by-employee/{employeeId}")]
        public async Task<IActionResult> UpdateByEmployee(long employeeId,UserAccountDto dto)
        {
            await _userService.UpdateByEmployee(employeeId, dto);
            return Ok(new
            {
                status = "success",
                message = $"User account for employee updated successfully."
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            await _userService.ResetPassword(dto);
            return Ok(new
            {
                status = "success",
                message = $"Password has been reset successfully"
            });
        }

        [HttpPost("2fa/send-otp")]
        public async Task<IActionResult> SendOtp(Send2FADto dto)
        {
            await _userService.SendTwoFactorOtp(dto);
            return Ok(new
            {
                status = "success",
                message = "OTP sent successfully"
            });
        }

        [HttpPost("2fa/verify")]
        public async Task<IActionResult> VerifyOtp(Verify2FADto dto)
        {
            await _userService.VerifyTwoFactorOtp(dto);
            return Ok(new
            {
                status = "success",
                message = "Two-factor authentication enabled"
            });
        }

    }
}
