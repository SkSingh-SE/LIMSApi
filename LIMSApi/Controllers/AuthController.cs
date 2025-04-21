using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserMaster user)
        {
            await _authService.RegisterUser(user);
            return Ok("User registered successfully.");

        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LoginDTO loginCred)
        {
            try
            {
                return Ok(await _authService.Authenticate(loginCred));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while login the user.");
                return Unauthorized("Invalid credentials");
            }
        }

        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            return Ok(await _authService.GetRefreshToken());

        }

    }
}
