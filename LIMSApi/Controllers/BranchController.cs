using System;
using System.Collections.Generic;
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
    public class BranchController : ControllerBase
    {
        private readonly LIMSContext _context;
        private readonly IBranchContext _branchContext;

        public BranchController(LIMSContext context, IBranchContext branchContext)
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

        [HttpGet("user-branches")]
        public async Task<IActionResult> GetUserBranches()
        {
            var user = LoggedInUserProvider.CurrentUser;
            if (user == null || !user.OrganizationID.HasValue)
                return Unauthorized();

            var query = _context.Branches
                .Where(b => b.IsActive && b.OrganizationID == user.OrganizationID.Value);

            if (!user.CanViewAllBranches)
            {
                var authorizedIds = _branchContext.AuthorizedBranchIDs;
                query = query.Where(b => authorizedIds.Contains(b.ID));
            }

            var branches = await query
                .OrderBy(b => b.Name)
                .Select(b => new
                {
                    b.ID,
                    b.Code,
                    b.Name,
                    b.Address,
                    b.IsHeadOffice,
                    IsCurrent = user.BranchID.HasValue && b.ID == user.BranchID.Value
                })
                .ToListAsync();

            return Ok(branches);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetBranches()
        {
            var orgId = _branchContext.RequireOrganizationId();
            var branches = await _context.Branches
                .Where(b => b.IsActive && b.OrganizationID == orgId)
                .OrderBy(b => b.Name)
                .Select(b => new
                {
                    b.ID,
                    b.Code,
                    b.Name,
                    b.Address,
                    b.IsHeadOffice
                })
                .ToListAsync();

            return Ok(branches);
        }

        [HttpPost("admin-list")]
        [RequirePermission(Permissions.Settings.Read)]
        public async Task<IActionResult> GetAdminBranches([FromBody] BranchAdminListRequestDto request)
        {
            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != request.OrganizationId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to view branches for this organization." });
            }

            IQueryable<Branch> query = _context.Branches
                .IgnoreQueryFilters()
                .Include(b => b.Organization)
                .Where(b => b.OrganizationID == request.OrganizationId);

            if (request.IsActive.HasValue)
            {
                query = query.Where(b => b.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(b => b.Name.ToLower().Contains(term) ||
                                         b.Code.ToLower().Contains(term) ||
                                         (b.Address != null && b.Address.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(b => b.IsHeadOffice)
                .ThenBy(b => b.Name)
                .Skip((request.PageNo - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BranchAdminItemDto
                {
                    ID = b.ID,
                    OrganizationID = b.OrganizationID,
                    OrganizationName = b.Organization != null ? b.Organization.LabName : string.Empty,
                    Name = b.Name,
                    Code = b.Code,
                    IsHeadOffice = b.IsHeadOffice,
                    Address = b.Address,
                    ContactEmail = b.ContactEmail,
                    ContactPhone = b.ContactPhone,
                    IsActive = b.IsActive,
                    DisciplineCount = _context.BranchDisciplines.IgnoreQueryFilters().Count(d => d.BranchID == b.ID && d.IsActive),
                    AssignedUserCount = _context.UserBranches.IgnoreQueryFilters().Count(ub => ub.BranchID == b.ID && ub.IsActive),
                    CreatedOn = b.CreatedOn,
                    ModifiedOn = b.ModifiedOn
                })
                .ToListAsync();

            return Ok(new BranchAdminListResponseDto
            {
                TotalCount = totalCount,
                Items = items
            });
        }

        [HttpGet("details/{id}")]
        [RequirePermission(Permissions.Settings.Read)]
        public async Task<IActionResult> GetBranchDetails(long id)
        {
            var branch = await _context.Branches
                .IgnoreQueryFilters()
                .Include(b => b.Organization)
                .FirstOrDefaultAsync(b => b.ID == id);

            if (branch == null)
                return NotFound(new { message = "Branch not found." });

            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != branch.OrganizationID))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to view this branch." });
            }

            var dto = new BranchAdminItemDto
            {
                ID = branch.ID,
                OrganizationID = branch.OrganizationID,
                OrganizationName = branch.Organization?.LabName ?? string.Empty,
                Name = branch.Name,
                Code = branch.Code,
                IsHeadOffice = branch.IsHeadOffice,
                Address = branch.Address,
                ContactEmail = branch.ContactEmail,
                ContactPhone = branch.ContactPhone,
                IsActive = branch.IsActive,
                DisciplineCount = await _context.BranchDisciplines.IgnoreQueryFilters().CountAsync(d => d.BranchID == branch.ID && d.IsActive),
                AssignedUserCount = await _context.UserBranches.IgnoreQueryFilters().CountAsync(ub => ub.BranchID == branch.ID && ub.IsActive),
                CreatedOn = branch.CreatedOn,
                ModifiedOn = branch.ModifiedOn
            };

            return Ok(dto);
        }

        [HttpPost("create")]
        [RequirePermission(Permissions.Settings.Update)]
        public async Task<IActionResult> CreateBranch([FromBody] BranchCreateDto dto)
        {
            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != dto.OrganizationID))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to create a branch in this organization." });
            }

            var org = await _context.Organizations.FindAsync(dto.OrganizationID);
            if (org == null || !org.IsActive)
            {
                return BadRequest(new { message = "Target organization is invalid or inactive." });
            }

            var code = dto.Code.Trim();
            var duplicate = await _context.Branches.IgnoreQueryFilters().AnyAsync(b => b.OrganizationID == dto.OrganizationID && b.Code == code);
            if (duplicate)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { message = $"Branch code '{code}' already exists in this organization." });
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (dto.IsHeadOffice)
                {
                    var existingHeadOffices = await _context.Branches
                        .IgnoreQueryFilters()
                        .Where(b => b.OrganizationID == dto.OrganizationID && b.IsHeadOffice)
                        .ToListAsync();

                    foreach (var ho in existingHeadOffices)
                    {
                        ho.IsHeadOffice = false;
                        ho.ModifiedBy = currentUser?.EmployeeID;
                        ho.ModifiedOn = DateTime.UtcNow;
                    }
                    await _context.SaveChangesAsync();
                }

                var branch = new Branch
                {
                    OrganizationID = dto.OrganizationID,
                    Name = dto.Name.Trim(),
                    Code = code,
                    IsHeadOffice = dto.IsHeadOffice,
                    Address = dto.Address?.Trim(),
                    ContactEmail = dto.ContactEmail?.Trim(),
                    ContactPhone = dto.ContactPhone?.Trim(),
                    IsActive = true,
                    CreatedBy = currentUser?.EmployeeID ?? 1,
                    CreatedOn = DateTime.UtcNow
                };

                _context.Branches.Add(branch);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new
                {
                    status = "success",
                    message = "Branch created successfully.",
                    id = branch.ID
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to create branch: " + ex.Message });
            }
        }

        [HttpPut("update")]
        [RequirePermission(Permissions.Settings.Update)]
        public async Task<IActionResult> UpdateBranch([FromBody] BranchUpdateDto dto)
        {
            var branch = await _context.Branches.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.ID == dto.ID);
            if (branch == null)
                return NotFound(new { message = "Branch not found." });

            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != branch.OrganizationID))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to update this branch." });
            }

            var code = dto.Code.Trim();
            var duplicate = await _context.Branches.IgnoreQueryFilters().AnyAsync(b => b.OrganizationID == branch.OrganizationID && b.Code == code && b.ID != dto.ID);
            if (duplicate)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { message = $"Branch code '{code}' already exists in this organization." });
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (dto.IsHeadOffice && !branch.IsHeadOffice)
                {
                    var existingHeadOffices = await _context.Branches
                        .IgnoreQueryFilters()
                        .Where(b => b.OrganizationID == branch.OrganizationID && b.IsHeadOffice && b.ID != branch.ID)
                        .ToListAsync();

                    foreach (var ho in existingHeadOffices)
                    {
                        ho.IsHeadOffice = false;
                        ho.ModifiedBy = currentUser?.EmployeeID;
                        ho.ModifiedOn = DateTime.UtcNow;
                    }
                    await _context.SaveChangesAsync();
                }

                branch.Name = dto.Name.Trim();
                branch.Code = code;
                branch.IsHeadOffice = dto.IsHeadOffice;
                branch.Address = dto.Address?.Trim();
                branch.ContactEmail = dto.ContactEmail?.Trim();
                branch.ContactPhone = dto.ContactPhone?.Trim();
                branch.IsActive = dto.IsActive;
                branch.ModifiedBy = currentUser?.EmployeeID;
                branch.ModifiedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new
                {
                    status = "success",
                    message = "Branch updated successfully."
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to update branch: " + ex.Message });
            }
        }

        [HttpPost("{id}/toggle-status")]
        [RequirePermission(Permissions.Settings.Update)]
        public async Task<IActionResult> ToggleBranchStatus(long id)
        {
            var branch = await _context.Branches.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.ID == id);
            if (branch == null)
                return NotFound(new { message = "Branch not found." });

            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != branch.OrganizationID))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to toggle this branch status." });
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (branch.IsActive)
                {
                    // Check if it's the only active branch
                    var activeCount = await _context.Branches.IgnoreQueryFilters().CountAsync(b => b.OrganizationID == branch.OrganizationID && b.IsActive);
                    if (activeCount <= 1)
                    {
                        return BadRequest(new { message = "Cannot deactivate the only active branch in this organization." });
                    }

                    branch.IsActive = false;
                    branch.ModifiedBy = currentUser?.EmployeeID;
                    branch.ModifiedOn = DateTime.UtcNow;

                    // Reassign default branch for affected users (Rule 18)
                    var affectedUsers = await _context.UserMasters
                        .IgnoreQueryFilters()
                        .Where(u => u.BranchID == id && u.OrganizationID == branch.OrganizationID)
                        .ToListAsync();

                    foreach (var user in affectedUsers)
                    {
                        // Unset IsDefault on old user branches to satisfy unique filtered index
                        var userBranches = await _context.UserBranches
                            .IgnoreQueryFilters()
                            .Where(ub => ub.UserID == user.ID)
                            .ToListAsync();

                        foreach (var ub in userBranches)
                        {
                            ub.IsDefault = false;
                        }

                        var remainingActiveAuthorizedBranch = await _context.UserBranches
                            .IgnoreQueryFilters()
                            .Include(ub => ub.Branch)
                            .Where(ub => ub.UserID == user.ID && ub.BranchID != id && ub.IsActive && ub.Branch != null && ub.Branch.IsActive)
                            .OrderByDescending(ub => ub.Branch!.IsHeadOffice)
                            .ThenBy(ub => ub.ID)
                            .FirstOrDefaultAsync();

                        if (remainingActiveAuthorizedBranch != null)
                        {
                            // Promote remaining branch to default
                            remainingActiveAuthorizedBranch.IsDefault = true;
                            user.BranchID = remainingActiveAuthorizedBranch.BranchID;
                        }
                        else
                        {
                            // No other active authorized branches remain
                            user.BranchID = null;
                        }

                        user.ModifiedBy = currentUser?.EmployeeID;
                        user.ModifiedOn = DateTime.UtcNow;
                    }
                }
                else
                {
                    branch.IsActive = true;
                    branch.ModifiedBy = currentUser?.EmployeeID;
                    branch.ModifiedOn = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new
                {
                    status = "success",
                    isActive = branch.IsActive,
                    message = branch.IsActive ? "Branch activated successfully." : "Branch deactivated successfully."
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to toggle branch status: " + ex.Message });
            }
        }

        [HttpGet("{id}/dependencies")]
        [RequirePermission(Permissions.Settings.Read)]
        public async Task<IActionResult> GetBranchDependencies(long id)
        {
            var branch = await _context.Branches.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(b => b.ID == id);
            if (branch == null)
                return NotFound(new { message = "Branch not found." });

            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != branch.OrganizationID))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to view dependencies for this branch." });
            }

            var activeUsersCount = await _context.UserBranches.IgnoreQueryFilters().CountAsync(ub => ub.BranchID == id && ub.IsActive);
            var defaultUsersCount = await _context.UserMasters.IgnoreQueryFilters().CountAsync(u => u.BranchID == id && u.IsActive);
            var equipmentCount = await _context.EquipmentMasters.IgnoreQueryFilters().CountAsync(e => e.BranchID == id && e.IsActive);
            var departmentsCount = await _context.DepartmentMasters.IgnoreQueryFilters().CountAsync(d => d.BranchID == id && d.IsActive);
            var openSamplesCount = await _context.SampleInwards.IgnoreQueryFilters().CountAsync(s => s.BranchID == id && s.IsActive);

            var dto = new BranchDependencyDto
            {
                BranchId = branch.ID,
                BranchName = branch.Name,
                ActiveUsersCount = activeUsersCount,
                DefaultUsersCount = defaultUsersCount,
                EquipmentCount = equipmentCount,
                DepartmentsCount = departmentsCount,
                OpenSamplesCount = openSamplesCount,
                CanDeactivate = true
            };

            return Ok(dto);
        }

        // ==========================================
        // Branch Discipline Management
        // ==========================================

        [HttpGet("{branchId}/disciplines")]
        [RequirePermission(Permissions.Settings.Read)]
        public async Task<IActionResult> GetBranchDisciplines(long branchId)
        {
            var branch = await _context.Branches.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(b => b.ID == branchId);
            if (branch == null)
                return NotFound(new { message = "Branch not found." });

            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != branch.OrganizationID))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to view disciplines for this branch." });
            }

            var assigned = await _context.BranchDisciplines
                .IgnoreQueryFilters()
                .Include(bd => bd.Discipline)
                .Where(bd => bd.BranchID == branchId && bd.IsActive)
                .OrderBy(bd => bd.Discipline != null ? bd.Discipline.Name : string.Empty)
                .Select(bd => new BranchDisciplineItemDto
                {
                    Id = bd.ID,
                    BranchID = bd.BranchID,
                    DisciplineID = bd.DisciplineID,
                    DisciplineName = bd.Discipline != null ? bd.Discipline.Name : string.Empty,
                    DisciplineCode = bd.Discipline != null ? bd.Discipline.Code : string.Empty,
                    DisciplineDescription = bd.Discipline != null ? bd.Discipline.Description : string.Empty,
                    IsAccredited = bd.IsAccredited,
                    IsActive = bd.IsActive
                })
                .ToListAsync();

            var assignedDisciplineIds = assigned.Select(a => a.DisciplineID).ToHashSet();

            var available = await _context.DisciplineMasters
                .Where(d => d.IsActive && !assignedDisciplineIds.Contains(d.ID))
                .OrderBy(d => d.Name)
                .Select(d => new AvailableDisciplineItemDto
                {
                    Id = d.ID,
                    Name = d.Name,
                    Code = d.Code,
                    Description = d.Description
                })
                .ToListAsync();

            return Ok(new BranchDisciplinesResponseDto
            {
                BranchId = branch.ID,
                BranchName = branch.Name,
                AssignedDisciplines = assigned,
                AvailableDisciplines = available
            });
        }

        [HttpPost("{branchId}/disciplines/assign")]
        [RequirePermission(Permissions.Settings.Update)]
        public async Task<IActionResult> AssignDisciplineToBranch(long branchId, [FromBody] BranchDisciplineAssignDto dto)
        {
            var branch = await _context.Branches.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.ID == branchId);
            if (branch == null)
                return NotFound(new { message = "Branch not found." });

            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != branch.OrganizationID))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to manage disciplines for this branch." });
            }

            var discipline = await _context.DisciplineMasters.FindAsync(dto.DisciplineID);
            if (discipline == null || !discipline.IsActive)
            {
                return BadRequest(new { message = "Selected discipline is invalid or inactive." });
            }

            var exists = await _context.BranchDisciplines.IgnoreQueryFilters().AnyAsync(bd => bd.BranchID == branchId && bd.DisciplineID == dto.DisciplineID);
            if (exists)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { message = $"Discipline '{discipline.Name}' is already assigned to this branch." });
            }

            var branchDiscipline = new BranchDiscipline
            {
                BranchID = branchId,
                DisciplineID = dto.DisciplineID,
                IsAccredited = dto.IsAccredited,
                IsActive = true,
                CreatedBy = currentUser?.EmployeeID ?? 1,
                CreatedOn = DateTime.UtcNow
            };

            _context.BranchDisciplines.Add(branchDiscipline);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = "success",
                message = $"Discipline '{discipline.Name}' assigned to branch successfully.",
                id = branchDiscipline.ID
            });
        }

        [HttpDelete("{branchId}/disciplines/{disciplineId}")]
        [RequirePermission(Permissions.Settings.Update)]
        public async Task<IActionResult> RemoveDisciplineFromBranch(long branchId, long disciplineId)
        {
            var branch = await _context.Branches.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(b => b.ID == branchId);
            if (branch == null)
                return NotFound(new { message = "Branch not found." });

            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != branch.OrganizationID))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to manage disciplines for this branch." });
            }

            var record = await _context.BranchDisciplines.IgnoreQueryFilters().FirstOrDefaultAsync(bd => bd.BranchID == branchId && bd.DisciplineID == disciplineId);
            if (record == null)
                return NotFound(new { message = "Discipline assignment not found." });

            _context.BranchDisciplines.Remove(record);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = "success",
                message = "Discipline removed from branch successfully."
            });
        }

        [HttpPut("{branchId}/disciplines/{disciplineId}/accreditation")]
        [RequirePermission(Permissions.Settings.Update)]
        public async Task<IActionResult> ToggleDisciplineAccreditation(long branchId, long disciplineId, [FromBody] BranchDisciplineAccreditationDto dto)
        {
            var branch = await _context.Branches.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(b => b.ID == branchId);
            if (branch == null)
                return NotFound(new { message = "Branch not found." });

            var isSysAdmin = IsSystemAdmin();
            var currentUser = LoggedInUserProvider.CurrentUser;

            if (!isSysAdmin && (currentUser?.OrganizationID == null || currentUser.OrganizationID.Value != branch.OrganizationID))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to update discipline accreditation for this branch." });
            }

            var record = await _context.BranchDisciplines.IgnoreQueryFilters().FirstOrDefaultAsync(bd => bd.BranchID == branchId && bd.DisciplineID == disciplineId);
            if (record == null)
                return NotFound(new { message = "Discipline assignment not found." });

            record.IsAccredited = dto.IsAccredited;
            record.ModifiedBy = currentUser?.EmployeeID;
            record.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = "success",
                isAccredited = record.IsAccredited,
                message = record.IsAccredited ? "Branch discipline marked as NABL Accredited." : "Branch discipline marked as Non-Accredited."
            });
        }
    }
}
