using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly LoginRateLimiter _rateLimiter;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, LoginRateLimiter rateLimiter)
        {
            _authService = authService;
            _logger = logger;
            _rateLimiter = rateLimiter;
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
            // Get client IP for rate limiting
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Check rate limit BEFORE attempting login
            var (allowed, remaining, retryAfter) = _rateLimiter.Check(ip);
            if (!allowed)
            {
                Response.Headers.Append("Retry-After", retryAfter.ToString());
                return StatusCode(StatusCodes.Status429TooManyRequests, new
                {
                    success = false,
                    message = $"Too many login attempts. Please try again after {retryAfter} seconds.",
                    retryAfterSeconds = retryAfter
                });
            }

            try
            {
                var result = await _authService.Authenticate(loginCred);

                // SUCCESS → reset rate limit for this IP
                _rateLimiter.ResetOnSuccess(ip);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // FAILURE → record the failed attempt
                _rateLimiter.RecordFailure(ip);

                _logger.LogError(ex, "An error occurred while login the user.");
                return Unauthorized(new
                {
                    success = false,
                    message = ex?.Message ?? "Something went wrong"
                });
            }
        }

        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            return Ok(await _authService.GetRefreshToken());

        }

    }
}
