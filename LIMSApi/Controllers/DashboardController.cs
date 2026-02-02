using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly LoggedInUserProvider _userProvider;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IDashboardService dashboardService, 
            LoggedInUserProvider userProvider,
            ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _userProvider = userProvider;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var userContext = LoggedInUserProvider.CurrentUser;

                if (userContext == null)
                {
                    _logger.LogWarning("Dashboard request with invalid or missing user context");
                    return Unauthorized(new DashboardErrorResponse
                    {
                        Error = "Unauthorized",
                        Message = "Invalid or missing user context",
                        StatusCode = 401,
                        Timestamp = DateTime.UtcNow,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                _logger.LogInformation("Dashboard request for user {UserId} with role {Role}", 
                    userContext.UserId, userContext.Role);

                var dashboardData = await _dashboardService.GetDashboardAsync(userContext);
                return Ok(dashboardData);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request parameters for dashboard");
                return BadRequest(new DashboardErrorResponse
                {
                    Error = "ValidationError",
                    Message = "Invalid request parameters",
                    StatusCode = 400,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized dashboard access attempt");
                return Unauthorized(new DashboardErrorResponse
                {
                    Error = "Unauthorized",
                    Message = "Access denied",
                    StatusCode = 401,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Service error while getting dashboard data");
                return StatusCode(503, new DashboardErrorResponse
                {
                    Error = "ServiceUnavailable",
                    Message = "Service temporarily unavailable. Please try again later.",
                    StatusCode = 503,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in dashboard endpoint");
                return StatusCode(500, new DashboardErrorResponse
                {
                    Error = "InternalServerError",
                    Message = "An unexpected error occurred",
                    StatusCode = 500,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpGet("cards")]
        public async Task<IActionResult> GetDashboardCards()
        {
            try
            {
                var userContext = LoggedInUserProvider.CurrentUser;

                if (userContext == null)
                {
                    _logger.LogWarning("Dashboard cards request with invalid or missing user context");
                    return Unauthorized(new DashboardErrorResponse
                    {
                        Error = "Unauthorized",
                        Message = "Invalid or missing user context",
                        StatusCode = 401,
                        Timestamp = DateTime.UtcNow,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                _logger.LogInformation("Dashboard cards request for user {UserId} with role {Role}", 
                    userContext.UserId, userContext.Role);

                var cards = await _dashboardService.GetDashboardCardsAsync(userContext);
                return Ok(cards);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request parameters for dashboard cards");
                return BadRequest(new DashboardErrorResponse
                {
                    Error = "ValidationError",
                    Message = "Invalid request parameters",
                    StatusCode = 400,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized dashboard cards access attempt");
                return Unauthorized(new DashboardErrorResponse
                {
                    Error = "Unauthorized",
                    Message = "Access denied",
                    StatusCode = 401,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Service error while getting dashboard cards");
                return StatusCode(503, new DashboardErrorResponse
                {
                    Error = "ServiceUnavailable",
                    Message = "Service temporarily unavailable. Please try again later.",
                    StatusCode = 503,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in dashboard cards endpoint");
                return StatusCode(500, new DashboardErrorResponse
                {
                    Error = "InternalServerError",
                    Message = "An unexpected error occurred",
                    StatusCode = 500,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpGet("charts")]
        public async Task<IActionResult> GetDashboardCharts()
        {
            try
            {
                var userContext = LoggedInUserProvider.CurrentUser;

                if (userContext == null)
                {
                    _logger.LogWarning("Dashboard charts request with invalid or missing user context");
                    return Unauthorized(new DashboardErrorResponse
                    {
                        Error = "Unauthorized",
                        Message = "Invalid or missing user context",
                        StatusCode = 401,
                        Timestamp = DateTime.UtcNow,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                _logger.LogInformation("Dashboard charts request for user {UserId} with role {Role}", 
                    userContext.UserId, userContext.Role);

                var charts = await _dashboardService.GetDashboardChartsAsync(userContext);
                return Ok(charts);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request parameters for dashboard charts");
                return BadRequest(new DashboardErrorResponse
                {
                    Error = "ValidationError",
                    Message = "Invalid request parameters",
                    StatusCode = 400,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized dashboard charts access attempt");
                return Unauthorized(new DashboardErrorResponse
                {
                    Error = "Unauthorized",
                    Message = "Access denied",
                    StatusCode = 401,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Service error while getting dashboard charts");
                return StatusCode(503, new DashboardErrorResponse
                {
                    Error = "ServiceUnavailable",
                    Message = "Service temporarily unavailable. Please try again later.",
                    StatusCode = 503,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in dashboard charts endpoint");
                return StatusCode(500, new DashboardErrorResponse
                {
                    Error = "InternalServerError",
                    Message = "An unexpected error occurred",
                    StatusCode = 500,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetDashboardNotifications()
        {
            try
            {
                var userContext = LoggedInUserProvider.CurrentUser;

                if (userContext == null)
                {
                    _logger.LogWarning("Dashboard notifications request with invalid or missing user context");
                    return Unauthorized(new DashboardErrorResponse
                    {
                        Error = "Unauthorized",
                        Message = "Invalid or missing user context",
                        StatusCode = 401,
                        Timestamp = DateTime.UtcNow,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                _logger.LogInformation("Dashboard notifications request for user {UserId} with role {Role}", 
                    userContext.UserId, userContext.Role);

                var notifications = await _dashboardService.GetDashboardNotificationsAsync(userContext);
                return Ok(notifications);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request parameters for dashboard notifications");
                return BadRequest(new DashboardErrorResponse
                {
                    Error = "ValidationError",
                    Message = "Invalid request parameters",
                    StatusCode = 400,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized dashboard notifications access attempt");
                return Unauthorized(new DashboardErrorResponse
                {
                    Error = "Unauthorized",
                    Message = "Access denied",
                    StatusCode = 401,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Service error while getting dashboard notifications");
                return StatusCode(503, new DashboardErrorResponse
                {
                    Error = "ServiceUnavailable",
                    Message = "Service temporarily unavailable. Please try again later.",
                    StatusCode = 503,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in dashboard notifications endpoint");
                return StatusCode(500, new DashboardErrorResponse
                {
                    Error = "InternalServerError",
                    Message = "An unexpected error occurred",
                    StatusCode = 500,
                    Timestamp = DateTime.UtcNow,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }
    }
}