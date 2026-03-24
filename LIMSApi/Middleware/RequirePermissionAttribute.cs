using LIMSApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LIMSApi.Middleware
{
    /// <summary>
    /// Enforces that the calling user has a specific permission granted in the database.
    /// Checks the UserPermissions table via PermissionMaster.Name match.
    /// Admin role bypasses all permission checks.
    ///
    /// Usage:
    ///   [RequirePermission("TEST_RESULT_SAVE")]              — single permission
    ///   [RequirePermission("REPORT_APPROVE")]                — on specific actions
    ///   [RequirePermission("INVOICE_GENERATE")]              — on financial operations
    ///
    /// The permission name must match PermissionMaster.Name exactly (case-sensitive).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _permissionName;

        public RequirePermissionAttribute(string permissionName)
        {
            _permissionName = permissionName;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Not authenticated — let [Authorize] handle 401
            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Admin bypasses all permission checks
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                return;

            // Extract user ID from JWT claims
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
            {
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = "Invalid user identity."
                })
                { StatusCode = StatusCodes.Status403Forbidden };
                return;
            }

            // Check permission in database
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<LIMSContext>();

            var hasPermission = await dbContext.UserPermissions
                .Include(up => up.Permission)
                .AnyAsync(up =>
                    up.UserID == userId &&
                    up.Permission != null &&
                    up.Permission.Name == _permissionName &&
                    up.IsGranted);

            if (!hasPermission)
            {
                var logger = context.HttpContext.RequestServices.GetService<ILogger<RequirePermissionAttribute>>();
                logger?.LogWarning(
                    "Permission denied: User {UserId} attempted action requiring '{Permission}'",
                    userId, _permissionName);

                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = $"You do not have permission to perform this action. Required: {_permissionName}"
                })
                { StatusCode = StatusCodes.Status403Forbidden };
            }
        }
    }
}
