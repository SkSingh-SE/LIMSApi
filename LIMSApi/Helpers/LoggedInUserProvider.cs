using System.Security.Claims;
using LIMSApi.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Helpers
{
    public class UserBranchPermissionDTO
    {
        public long BranchID { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanCreate { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        public bool CanExecute { get; set; } = true;
        public bool CanApprove { get; set; } = false;
        public bool CanDelete { get; set; } = false;
    }

    public class LoggedInUserDTO
    {
        public long UserId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Role { get; set; }
        public long EmployeeID { get; set; }
        public string? CompanyCode { get; set; }
        public long? OrganizationID { get; set; }
        public long? BranchID { get; set; } // Default/Selected Branch
        public bool CanViewAllBranches { get; set; } = false;
        public List<UserBranchPermissionDTO> BranchPermissions { get; set; } = new();
    }

    public class LoggedInUserProvider : IBranchContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LIMSContext _context;
        private static readonly AsyncLocal<LoggedInUserDTO?> _currentUser = new();

        public LoggedInUserProvider(IHttpContextAccessor httpContext, LIMSContext context)
        {
            _httpContextAccessor = httpContext;
            _context = context;
        }

        public long? CurrentOrganizationID => ResolveCurrentUser()?.OrganizationID;
        public long? CurrentBranchID => ResolveCurrentUser()?.BranchID;
        public bool CanViewAllBranches => ResolveCurrentUser()?.CanViewAllBranches ?? false;

        public IReadOnlyList<long> AuthorizedBranchIDs
        {
            get
            {
                var user = ResolveCurrentUser();
                if (user == null) return Array.Empty<long>();
                return user.BranchPermissions.Where(p => p.CanView).Select(p => p.BranchID).Distinct().ToList();
            }
        }

        public IReadOnlyList<long> AuthorizedExecutionBranchIDs
        {
            get
            {
                var user = ResolveCurrentUser();
                if (user == null) return Array.Empty<long>();
                return user.BranchPermissions.Where(p => p.CanExecute || p.CanCreate || p.CanEdit).Select(p => p.BranchID).Distinct().ToList();
            }
        }

        public long RequireCurrentBranchId()
        {
            var branchId = CurrentBranchID;
            if (!branchId.HasValue || branchId.Value <= 0)
            {
                throw new UnauthorizedAccessException("An active operating Branch context is required to execute this operation.");
            }
            return branchId.Value;
        }

        public long RequireOrganizationId()
        {
            var orgId = CurrentOrganizationID;
            if (!orgId.HasValue || orgId.Value <= 0)
            {
                throw new UnauthorizedAccessException("An active Organization context is required to perform this operation.");
            }
            return orgId.Value;
        }

        public bool IsAuthorizedForBranch(long branchId, BranchAction action = BranchAction.View)
        {
            var user = ResolveCurrentUser();
            if (user == null || !user.OrganizationID.HasValue) return false;

            // Org-wide VIEW permission only grants View access
            if (action == BranchAction.View && user.CanViewAllBranches)
            {
                return true;
            }

            var perm = user.BranchPermissions.FirstOrDefault(p => p.BranchID == branchId);
            if (perm == null) return false;

            return action switch
            {
                BranchAction.View => perm.CanView,
                BranchAction.Create => perm.CanCreate,
                BranchAction.Edit => perm.CanEdit,
                BranchAction.Execute => perm.CanExecute,
                BranchAction.Approve => perm.CanApprove,
                BranchAction.Delete => perm.CanDelete,
                _ => false
            };
        }

        /// <summary>
        /// Authoritative tenant resolver. NEVER falls back to a default.
        /// Returns { OrganizationID, BranchID, CompanyCode } derived from the JWT cross-validated
        /// against the Branches table for the given execution branch.
        /// Throws InvalidOperationException if any tenant value cannot be resolved.
        /// </summary>
        public TenantContext ResolveTenantContext(long executionBranchId)
        {
            if (executionBranchId <= 0)
            {
                throw new InvalidOperationException(
                    "ResolveTenantContext requires a positive executionBranchId. Refusing to silently default to any organization/branch.");
            }

            var user = ResolveCurrentUser();
            if (user == null)
            {
                throw new InvalidOperationException(
                    "No authenticated user (JWT) is available to resolve tenant context. Refusing to silently default.");
            }

            // Pull the canonical branch row.
            var branch = _context.Branches.AsNoTracking()
                .FirstOrDefault(b => b.ID == executionBranchId && b.IsActive);

            if (branch == null)
            {
                throw new InvalidOperationException(
                    $"Execution branch {executionBranchId} is not found or is inactive in the Branches table. Refusing to silently default.");
            }

            // OrganizationID: prefer JWT (authoritative tenant), then cross-validate with branch.OrganizationID.
            long organizationId;
            if (user.OrganizationID.HasValue && user.OrganizationID.Value > 0)
            {
                organizationId = user.OrganizationID.Value;
                if (branch.OrganizationID > 0 && branch.OrganizationID != organizationId)
                {
                    throw new InvalidOperationException(
                        $"Cross-tenant integrity violation: JWT OrganizationID={organizationId} does not match Branch {executionBranchId} OrganizationID={branch.OrganizationID}. Refusing to silently default.");
                }
            }
            else if (branch.OrganizationID > 0)
            {
                organizationId = branch.OrganizationID;
            }
            else
            {
                throw new InvalidOperationException(
                    $"Cannot resolve OrganizationID for execution branch {executionBranchId}. Neither JWT nor Branches table contains a valid value. Refusing to silently default.");
            }

            // CompanyCode: prefer JWT claim; never invent a value.
            if (string.IsNullOrWhiteSpace(user.CompanyCode))
            {
                throw new InvalidOperationException(
                    "JWT does not contain a CompanyCode claim. Refusing to silently default to 'LIMS' or any other value.");
            }

            return new TenantContext
            {
                OrganizationID = organizationId,
                BranchID = executionBranchId,
                CompanyCode = user.CompanyCode
            };
        }

        public void Initialize()
        {
            ResolveCurrentUser();
        }

        public LoggedInUserDTO? ResolveCurrentUser()
        {
            var context = _httpContextAccessor?.HttpContext;
            if (context == null) return _currentUser.Value;

            if (context.Items.TryGetValue("CurrentUser", out var cached) && cached is LoggedInUserDTO cachedDto)
            {
                return cachedDto;
            }

            var user = context.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                _currentUser.Value = null;
                return null;
            }

            var canViewAll = bool.TryParse(user.FindFirst("CanViewAllBranches")?.Value, out var cva) && cva;
            var defaultBranchId = long.TryParse(user.FindFirst("BranchID")?.Value, out var bid) ? bid : (long?)null;

            // Deserialize granular branch permissions from JWT
            var branchPermissions = new List<UserBranchPermissionDTO>();
            var userBranchesClaim = user.FindFirst("UserBranches")?.Value;
            if (!string.IsNullOrWhiteSpace(userBranchesClaim))
            {
                try
                {
                    var parsed = System.Text.Json.JsonSerializer.Deserialize<List<UserBranchPermissionDTO>>(userBranchesClaim);
                    if (parsed != null)
                    {
                        branchPermissions.AddRange(parsed);
                    }
                }
                catch { }
            }

            // Fallback: If no explicit branch permissions but default branch exists, seed default view permission
            if (!branchPermissions.Any() && defaultBranchId.HasValue && defaultBranchId.Value > 0)
            {
                branchPermissions.Add(new UserBranchPermissionDTO
                {
                    BranchID = defaultBranchId.Value,
                    CanView = true,
                    CanCreate = true,
                    CanEdit = true,
                    CanExecute = true,
                    CanApprove = false,
                    CanDelete = false
                });
            }

            // Handle requested operating branch from header (X-Branch-ID)
            long? operatingBranchId = defaultBranchId;
            if (context.Request.Headers.TryGetValue("X-Branch-ID", out var headerVal))
            {
                if (long.TryParse(headerVal.ToString(), out var requestedBranchId) && requestedBranchId > 0)
                {
                    var isAuthorizedForRequested = branchPermissions.Any(p => p.BranchID == requestedBranchId && p.CanView) || canViewAll;
                    if (isAuthorizedForRequested)
                    {
                        operatingBranchId = requestedBranchId;
                    }
                    else
                    {
                        // Unauthorized requested branch -> Fail closed to impossible ID (-1)
                        operatingBranchId = -1;
                    }
                }
                else
                {
                    // Malformed or negative requested branch -> Fail closed to impossible ID (-1)
                    operatingBranchId = -1;
                }
            }

            var resolved = new LoggedInUserDTO
            {
                UserId = long.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : 0,
                Name = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                Role = user.FindFirst(ClaimTypes.Role)?.Value,
                EmployeeID = long.TryParse(user.FindFirst("EmployeeID")?.Value, out var eid) ? eid : 0,
                CompanyCode = user.FindFirst("CompanyCode")?.Value,
                OrganizationID = long.TryParse(user.FindFirst("OrganizationID")?.Value, out var oid) ? oid : (long?)null,
                BranchID = operatingBranchId,
                CanViewAllBranches = canViewAll,
                BranchPermissions = branchPermissions
            };

            context.Items["CurrentUser"] = resolved;
            _currentUser.Value = resolved;
            return resolved;
        }

        public static LoggedInUserDTO? CurrentUser => _currentUser.Value;

        public static void ClearUser()
        {
            _currentUser.Value = null;
        }
    }
}
