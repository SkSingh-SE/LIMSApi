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

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
    }
}
