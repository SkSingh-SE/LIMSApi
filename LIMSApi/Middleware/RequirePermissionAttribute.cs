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

            // Resolution order:
            //   1. User-specific override (UserPermission) — highest priority, can grant OR revoke
            //   2. Role default (RolePermission) — inherited from user's role
            //
            // Note: UserPermission.IsGranted = false REVOKES a role-granted permission
            //       UserPermission absence → fall back to RolePermission

            // Step 1: Check user override
            var userOverride = await dbContext.UserPermissions
                .Include(up => up.Permission)
                .Where(up =>
                    up.IsActive &&
                    up.UserID == userId &&
                    up.Permission != null &&
                    up.Permission.Name == _permissionName)
                .Select(up => (bool?)up.IsGranted)
                .FirstOrDefaultAsync();

            bool hasPermission;
            if (userOverride.HasValue)
            {
                // User override takes precedence (grant or revoke)
                hasPermission = userOverride.Value;
            }
            else
            {
                // Step 2: Fall back to role default via RolePermission join
                var roleIdClaim = user.FindFirst("RoleID")?.Value
                    ?? user.FindFirst(ClaimTypes.GroupSid)?.Value;

                if (!long.TryParse(roleIdClaim, out var roleId))
                {
                    // Role claim missing — look up from DB
                    roleId = await dbContext.UserMasters
                        .Where(u => u.ID == userId && u.IsActive)
                        .Select(u => u.RoleID ?? 0)
                        .FirstOrDefaultAsync();
                }

                if (roleId == 0)
                {
                    hasPermission = false;
                }
                else
                {
                    hasPermission = await dbContext.RolePermissions
                        .Include(rp => rp.Permission)
                        .AnyAsync(rp =>
                            rp.IsActive &&
                            rp.RoleID == roleId &&
                            rp.Permission != null &&
                            rp.Permission.Name == _permissionName &&
                            rp.IsGranted);
                }
            }

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
