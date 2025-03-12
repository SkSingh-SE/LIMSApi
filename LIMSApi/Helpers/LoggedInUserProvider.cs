using System.Security.Claims;

namespace LIMSApi.Helpers
{
    public class LoggedInUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly AsyncLocal<LoggedInUserDTO> _currentUser = new();
        public LoggedInUserProvider(IHttpContextAccessor httpContext)
        {
            _httpContextAccessor = httpContext;
        }

        public void Initialize()
        {
            var userClaims = _httpContextAccessor?.HttpContext?.User;
            if (userClaims == null || !userClaims.Identity.IsAuthenticated)
            {
                _currentUser.Value = null;
                return;
            }

            _currentUser.Value = new LoggedInUserDTO
            {
                UserId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                Name = userClaims.FindFirst(ClaimTypes.Name)?.Value ?? "",
                Email = userClaims.FindFirst(ClaimTypes.Email)?.Value ?? "",
                Role = userClaims.FindFirst(ClaimTypes.Role)?.Value ?? "",
                EmployeeID = int.Parse(userClaims.FindFirst("EmployeeID")?.Value ?? "0"),
                CompanyCode = userClaims.FindFirst("CompanyCode")?.Value ?? "",
            };
        }

        // Public property to get user without needing dependency injection
        public static LoggedInUserDTO CurrentUser => _currentUser.Value;

        public static void ClearUser() => _currentUser.Value = null;
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
