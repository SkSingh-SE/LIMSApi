using System.Security.Claims;

namespace LIMSApi.Helpers
{
    public class LoggedInUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly AsyncLocal<LoggedInUserDTO?> _currentUser = new();

        public LoggedInUserProvider(IHttpContextAccessor httpContext)
        {
            _httpContextAccessor = httpContext;
        }

        public void Initialize()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                _currentUser.Value = null;
                return;
            }

            // Ensure cleanup when request finishes (CRITICAL)
            context.Response.OnCompleted(() =>
            {
                _currentUser.Value = null;
                return Task.CompletedTask;
            });

            var user = context.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                _currentUser.Value = null;
                return;
            }

            _currentUser.Value = new LoggedInUserDTO
            {
                UserId = long.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : 0,
                Name = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                Role = user.FindFirst(ClaimTypes.Role)?.Value,
                EmployeeID = long.TryParse(user.FindFirst("EmployeeID")?.Value, out var eid) ? eid : 0,
                CompanyCode = user.FindFirst("CompanyCode")?.Value
            };
        }

        // Signature unchanged
        public static LoggedInUserDTO? CurrentUser => _currentUser.Value;

        // Signature unchanged
        public static void ClearUser()
        {
            _currentUser.Value = null;
        }
    }

    public class LoggedInUserDTO
    {
        public long UserId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Role { get; set; }
        public long EmployeeID { get; set; }
        public string? CompanyCode { get; set; }
    }
}
