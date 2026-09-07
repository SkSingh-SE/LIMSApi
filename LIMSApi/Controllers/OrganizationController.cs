using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly LIMSContext _context;
        private readonly IBranchContext _branchContext;

        public OrganizationController(LIMSContext context, IBranchContext branchContext)
        {
            _context = context;
            _branchContext = branchContext;
        }

        private bool IsSystemAdmin()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var isAdminClaim = User.FindFirst("IsAdmin")?.Value;
            var currentUser = LoggedInUserProvider.CurrentUser;

            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(isAdminClaim, "True", StringComparison.OrdinalIgnoreCase) ||
                   (currentUser != null && string.Equals(currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase));
        }

        [HttpPost("list")]
        [RequirePermission(Permissions.Settings.Read)]
        public async Task<IActionResult> GetOrganizations([FromBody] OrganizationAdminListRequestDto request)
        {
            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;
            var callerOrgId = currentUser?.OrganizationID;

            IQueryable<Organization> query = _context.Organizations.AsNoTracking();

            if (!isSysAdmin)
            {
                if (!callerOrgId.HasValue)
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "User is not assigned to an organization." });

                query = query.Where(o => o.Id == callerOrgId.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(o => o.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(o => o.LabName.ToLower().Contains(term) ||
                                         o.LabCode.ToLower().Contains(term) ||
                                         o.ContactEmail.ToLower().Contains(term) ||
                                         o.ContactPhone.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(o => o.LabName)
                .Skip((request.PageNo - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OrganizationAdminItemDto
                {
                    Id = o.Id,
                    LabName = o.LabName,
                    LabCode = o.LabCode,
                    LabAddress = o.LabAddress,
                    ContactEmail = o.ContactEmail,
                    ContactPhone = o.ContactPhone,
                    OrganizationLogo = o.OrganizationLogo,
                    CIN = o.CIN,
                    Website = o.Website,
                    MobileNo = o.MobileNo,
                    UlrPrefix = o.UlrPrefix,
                    LabLocationCode = o.LabLocationCode,
                    IsMultiBranch = o.IsMultiBranch,
                    IsActive = o.IsActive,
                    BranchCount = _context.Branches.IgnoreQueryFilters().Count(b => b.OrganizationID == o.Id && b.IsActive),
                    ActiveUserCount = _context.UserMasters.IgnoreQueryFilters().Count(u => u.OrganizationID == o.Id && u.IsActive),
                    CreatedOn = o.CreatedOn,
                    ModifiedOn = o.ModifiedOn
                })
                .ToListAsync();

            return Ok(new OrganizationAdminListResponseDto
            {
                TotalCount = totalCount,
                Items = items
            });
        }

        [HttpGet("details/{id}")]
        [RequirePermission(Permissions.Settings.Read)]
        public async Task<IActionResult> GetOrganizationDetails(long id)
        {
            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != id))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to view this organization." });
            }

            var org = await _context.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
            if (org == null)
                return NotFound(new { message = "Organization not found." });

            var dto = new OrganizationAdminItemDto
            {
                Id = org.Id,
                LabName = org.LabName,
                LabCode = org.LabCode,
                LabAddress = org.LabAddress,
                ContactEmail = org.ContactEmail,
                ContactPhone = org.ContactPhone,
                OrganizationLogo = org.OrganizationLogo,
                CIN = org.CIN,
                Website = org.Website,
                MobileNo = org.MobileNo,
                UlrPrefix = org.UlrPrefix,
                LabLocationCode = org.LabLocationCode,
                IsMultiBranch = org.IsMultiBranch,
                IsActive = org.IsActive,
                BranchCount = await _context.Branches.IgnoreQueryFilters().CountAsync(b => b.OrganizationID == org.Id && b.IsActive),
                ActiveUserCount = await _context.UserMasters.IgnoreQueryFilters().CountAsync(u => u.OrganizationID == org.Id && u.IsActive),
                CreatedOn = org.CreatedOn,
                ModifiedOn = org.ModifiedOn
            };

            return Ok(dto);
        }

        [HttpPost("create")]
        [RequirePermission(Permissions.Admin.ManageSettings)]
        public async Task<IActionResult> CreateOrganization([FromBody] OrganizationCreateDto dto)
        {
            if (!IsSystemAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only system administrators can create organizations." });
            }

            var code = dto.LabCode.Trim();
            var codeExists = await _context.Organizations.AnyAsync(o => o.LabCode == code);
            if (codeExists)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { message = $"Organization code '{code}' already exists." });
            }

            var currentUser = LoggedInUserProvider.CurrentUser;
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var org = new Organization
                {
                    LabName = dto.LabName.Trim(),
                    LabCode = code,
                    LabAddress = dto.LabAddress.Trim(),
                    ContactEmail = dto.ContactEmail.Trim(),
                    ContactPhone = dto.ContactPhone.Trim(),
                    OrganizationLogo = dto.OrganizationLogo,
                    CIN = dto.CIN,
                    Website = dto.Website,
                    MobileNo = dto.MobileNo,
                    UlrPrefix = dto.UlrPrefix,
                    LabLocationCode = dto.LabLocationCode,
                    IsMultiBranch = dto.IsMultiBranch,
                    IsActive = true,
                    CreatedBy = currentUser?.EmployeeID ?? 1,
                    CreatedOn = DateTime.UtcNow
                };

                _context.Organizations.Add(org);
                await _context.SaveChangesAsync();

                // Create initial default Head Office Branch for the organization
                var defaultBranch = new Branch
                {
                    OrganizationID = org.Id,
                    Name = $"{org.LabName} - Main",
                    Code = $"{org.LabCode}-MAIN",
                    IsHeadOffice = true,
                    Address = org.LabAddress,
                    ContactEmail = org.ContactEmail,
                    ContactPhone = org.ContactPhone,
                    IsActive = true,
                    CreatedBy = currentUser?.EmployeeID ?? 1,
                    CreatedOn = DateTime.UtcNow
                };
                _context.Branches.Add(defaultBranch);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new
                {
                    status = "success",
                    message = "Organization created successfully.",
                    id = org.Id
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to create organization: " + ex.Message });
            }
        }

        [HttpPut("update")]
        [RequirePermission(Permissions.Settings.Update)]
        public async Task<IActionResult> UpdateOrganization([FromBody] OrganizationUpdateDto dto)
        {
            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != dto.Id))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to update this organization." });
            }

            var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == dto.Id);
            if (org == null)
                return NotFound(new { message = "Organization not found." });

            var code = dto.LabCode.Trim();
            var codeExists = await _context.Organizations.AnyAsync(o => o.LabCode == code && o.Id != dto.Id);
            if (codeExists)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { message = $"Organization code '{code}' is already used by another organization." });
            }

            org.LabName = dto.LabName.Trim();
            org.LabCode = code;
            org.LabAddress = dto.LabAddress.Trim();
            org.ContactEmail = dto.ContactEmail.Trim();
            org.ContactPhone = dto.ContactPhone.Trim();
            org.OrganizationLogo = dto.OrganizationLogo;
            org.CIN = dto.CIN;
            org.Website = dto.Website;
            org.MobileNo = dto.MobileNo;
            org.UlrPrefix = dto.UlrPrefix;
            org.LabLocationCode = dto.LabLocationCode;
            org.IsMultiBranch = dto.IsMultiBranch;
            org.ModifiedBy = currentUser?.EmployeeID;
            org.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = "success",
                message = "Organization updated successfully."
            });
        }

        [HttpPost("{id}/toggle-status")]
        [RequirePermission(Permissions.Admin.ManageSettings)]
        public async Task<IActionResult> ToggleOrganizationStatus(long id)
        {
            if (!IsSystemAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only system administrators can activate/deactivate organizations." });
            }

            var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id);
            if (org == null)
                return NotFound(new { message = "Organization not found." });

            if (org.IsActive)
            {
                var activeCount = await _context.Organizations.CountAsync(o => o.IsActive);
                if (activeCount <= 1)
                {
                    return BadRequest(new { message = "Cannot deactivate the only active organization in the system." });
                }
            }

            org.IsActive = !org.IsActive;
            org.ModifiedBy = LoggedInUserProvider.CurrentUser?.EmployeeID;
            org.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = "success",
                isActive = org.IsActive,
                message = org.IsActive ? "Organization activated successfully." : "Organization deactivated successfully."
            });
        }

        [HttpGet("{id}/dependencies")]
        [RequirePermission(Permissions.Settings.Read)]
        public async Task<IActionResult> GetOrganizationDependencies(long id)
        {
            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != id))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to view this organization's dependencies." });
            }

            var org = await _context.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
            if (org == null)
                return NotFound(new { message = "Organization not found." });

            var activeBranchesCount = await _context.Branches.IgnoreQueryFilters().CountAsync(b => b.OrganizationID == id && b.IsActive);
            var activeUsersCount = await _context.UserMasters.IgnoreQueryFilters().CountAsync(u => u.OrganizationID == id && u.IsActive);
            var openSamplesCount = await _context.SampleInwards.IgnoreQueryFilters().CountAsync(s => s.Branch != null && s.Branch.OrganizationID == id && s.IsActive);

            var dto = new OrganizationDependencyDto
            {
                OrganizationId = org.Id,
                LabName = org.LabName,
                ActiveBranchesCount = activeBranchesCount,
                ActiveUsersCount = activeUsersCount,
                OpenSamplesCount = openSamplesCount,
                CanDeactivate = true
            };

            return Ok(dto);
        }
    }
}
