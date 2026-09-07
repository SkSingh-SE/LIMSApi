using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LIMSContext context;
        private LoggedInUserDTO loggedInUser;
        public UserRepository(LIMSContext _context)
        {
            this.context = _context;
            this.loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddUser(UserMaster user)
        {
            await context.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<UserMaster> GetUserByEmail(string email)
        {
            var user = await context.UserMasters
                .IgnoreQueryFilters()
                .Include(x => x.Employee)
                    .ThenInclude(e => e.Designation)
                        .ThenInclude(d => d.Role)
                .Include(x => x.Branch)
                    .ThenInclude(b => b.Organization)
                .Include(x => x.UserBranches)
                    .ThenInclude(ub => ub.Branch)
                .FirstOrDefaultAsync(x => x.EmailId == email);
            return user;
        }

        public async Task<List<UserMaster>> GetAllUserByRoleId(long Id)
        {
            return await context.UserMasters.Where(x => x.RoleID == Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode).ToListAsync();
        }
        public async Task UpdateUser(UserMaster user)
        {
            var userToUpdate = await context.UserMasters.FirstOrDefaultAsync(x => x.EmailId == user.EmailId);
            if (userToUpdate != null)
            {
                userToUpdate.UserName = user.UserName;
                await context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("User not found");
            }
        }

        public async Task UpdateUsers(List<UserMaster> users)
        {
            foreach (var user in users)
            {
                var userToUpdate = await context.UserMasters.FirstOrDefaultAsync(x => x.EmailId == user.EmailId);
                if (userToUpdate != null)
                {
                    userToUpdate.UserName = user.UserName;
                    userToUpdate.RoleID = user.RoleID;
                    userToUpdate.RoleName = user.RoleName;
                    userToUpdate.IsAdmin = user.IsAdmin;
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task<bool> DeleteUser(string email)
        {
            var user = await context.UserMasters.FirstOrDefaultAsync(x => x.EmailId == email);
            if (user != null)
            {
                user.IsActive = false;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<DropdwonSelector>> GetUserDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in context.UserMasters where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode && a.RoleName != "Super Admin" select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => (x.UserName != null && x.UserName.Contains(search)));
                }
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.UserName,
            })).ToListAsync();

            return data;
        }

        public async Task<UserMaster> GetByEmployee(long employeeId)
        {
            var user = await context.UserMasters
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.EmployeeID == employeeId);

            if (user is null) throw new InvalidOperationException("User not found for the given employee ID.");

            return user;
        }
        public async Task UpdateByEmployee(long employeeId, UserAccountDto dto)
        {
            var user = await context.UserMasters
                .FirstOrDefaultAsync(u => u.EmployeeID == employeeId);

            if (user is null) throw new InvalidOperationException("User not found for the given employee ID.");


            user.IsLoginEnabled = dto.IsLoginEnabled;
            user.SessionTimeout = dto.SessionTimeout;
            user.ForcePasswordChange = dto.ForcePasswordChange;
            user.UnlockMethod = dto.UnlockMethod;
            user.RemoteLogin = dto.AllowRemoteLogin;
            user.IpRestriction = dto.IpRestriction;
            user.WorkingHours = dto.WorkingHours;
            user.TwoFactorEnabled = dto.TwoFactorEnabled;

            await context.SaveChangesAsync();
        }

        public async Task<UserMaster?> GetUserWithBranchesById(long userId)
        {
            return await context.UserMasters
                .IgnoreQueryFilters()
                .Include(u => u.Branch)
                .Include(u => u.UserBranches)
                    .ThenInclude(ub => ub.Branch)
                .FirstOrDefaultAsync(u => u.ID == userId);
        }

        public async Task<UserMaster?> GetUserWithBranchesByEmployeeId(long employeeId)
        {
            return await context.UserMasters
                .IgnoreQueryFilters()
                .Include(u => u.Branch)
                .Include(u => u.UserBranches)
                    .ThenInclude(ub => ub.Branch)
                .FirstOrDefaultAsync(u => u.EmployeeID == employeeId);
        }

        public async Task<List<Branch>> GetActiveBranchesByOrganizationId(long organizationId)
        {
            return await context.Branches
                .IgnoreQueryFilters()
                .Where(b => b.OrganizationID == organizationId && b.IsActive)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task UpdateUserBranchAccess(long userId, UpdateUserBranchAccessDto dto, long? modifiedBy)
        {
            var user = await context.UserMasters
                .IgnoreQueryFilters()
                .Include(u => u.UserBranches)
                .FirstOrDefaultAsync(u => u.ID == userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var targetOrgId = user.OrganizationID ?? (user.Branch != null ? user.Branch.OrganizationID : 0);

            // Cross-organization validation
            if (dto.Branches != null && dto.Branches.Any())
            {
                var branchIds = dto.Branches.Select(b => b.BranchId).Distinct().ToList();
                var validBranches = await context.Branches
                    .IgnoreQueryFilters()
                    .Where(b => branchIds.Contains(b.ID) && b.IsActive)
                    .ToListAsync();

                if (validBranches.Count != branchIds.Count)
                {
                    throw new ArgumentException("One or more assigned branches do not exist or are inactive.");
                }

                if (targetOrgId > 0)
                {
                    var foreignBranch = validBranches.FirstOrDefault(b => b.OrganizationID != targetOrgId);
                    if (foreignBranch != null)
                    {
                        throw new ArgumentException($"Branch '{foreignBranch.Name}' (ID: {foreignBranch.ID}) does not belong to the user's organization (Org ID: {targetOrgId}). Cross-organization branch assignment is strictly rejected.");
                    }
                }
            }

            // Single default validation
            long? defaultBranchId = null;
            if (dto.Branches != null && dto.Branches.Any())
            {
                var defaultItems = dto.Branches.Where(b => b.IsDefault).ToList();
                if (defaultItems.Count == 0)
                {
                    // Auto-promote first branch as default if none explicitly checked
                    dto.Branches.First().IsDefault = true;
                    defaultBranchId = dto.Branches.First().BranchId;
                }
                else if (defaultItems.Count > 1)
                {
                    // If multiple checked, pick the first one and uncheck others
                    defaultBranchId = defaultItems.First().BranchId;
                    foreach (var item in dto.Branches)
                    {
                        item.IsDefault = (item.BranchId == defaultBranchId);
                    }
                }
                else
                {
                    defaultBranchId = defaultItems.First().BranchId;
                }
            }

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // 1. Demote all existing user branches IsDefault = false first to prevent unique filtered index violation
                var existingUserBranches = await context.UserBranches
                    .IgnoreQueryFilters()
                    .Where(ub => ub.UserID == user.ID)
                    .ToListAsync();

                foreach (var ub in existingUserBranches)
                {
                    ub.IsDefault = false;
                }
                await context.SaveChangesAsync();

                // 2. Process assigned branches
                var assignedBranchIds = dto.Branches != null
                    ? dto.Branches.Select(b => b.BranchId).ToHashSet()
                    : new HashSet<long>();

                if (dto.Branches != null)
                {
                    foreach (var item in dto.Branches)
                    {
                        var existing = existingUserBranches.FirstOrDefault(ub => ub.BranchID == item.BranchId);
                        if (existing != null)
                        {
                            existing.IsActive = true;
                            existing.IsDefault = item.IsDefault;
                            existing.CanView = item.CanView;
                            existing.CanCreate = item.CanCreate;
                            existing.CanEdit = item.CanEdit;
                            existing.CanExecute = item.CanExecute;
                            existing.CanApprove = item.CanApprove;
                            existing.CanDelete = item.CanDelete;
                            existing.ModifiedBy = modifiedBy;
                            existing.ModifiedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            var newUb = new UserBranch
                            {
                                UserID = user.ID,
                                BranchID = item.BranchId,
                                IsDefault = item.IsDefault,
                                CanView = item.CanView,
                                CanCreate = item.CanCreate,
                                CanEdit = item.CanEdit,
                                CanExecute = item.CanExecute,
                                CanApprove = item.CanApprove,
                                CanDelete = item.CanDelete,
                                IsActive = true,
                                CreatedBy = modifiedBy ?? 0,
                                CreatedOn = DateTime.UtcNow
                            };
                            await context.UserBranches.AddAsync(newUb);
                        }
                    }
                }

                // 3. Deactivate unassigned branches
                foreach (var ub in existingUserBranches)
                {
                    if (!assignedBranchIds.Contains(ub.BranchID))
                    {
                        ub.IsActive = false;
                        ub.IsDefault = false;
                        ub.ModifiedBy = modifiedBy;
                        ub.ModifiedOn = DateTime.UtcNow;
                    }
                }

                // 4. Update user master
                user.CanViewAllBranches = dto.CanViewAllBranches;
                user.BranchID = defaultBranchId;
                user.ModifiedBy = modifiedBy;
                user.ModifiedOn = DateTime.UtcNow;

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SetDefaultBranch(long userId, long branchId, long? modifiedBy)
        {
            var user = await context.UserMasters
                .IgnoreQueryFilters()
                .Include(u => u.UserBranches)
                .FirstOrDefaultAsync(u => u.ID == userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var targetUb = user.UserBranches.FirstOrDefault(ub => ub.BranchID == branchId && ub.IsActive);
            if (targetUb == null)
            {
                throw new InvalidOperationException("Cannot set an unassigned or inactive branch as default.");
            }

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Demote existing
                foreach (var ub in user.UserBranches)
                {
                    ub.IsDefault = false;
                    ub.ModifiedBy = modifiedBy;
                    ub.ModifiedOn = DateTime.UtcNow;
                }
                await context.SaveChangesAsync();

                // Promote target
                targetUb.IsDefault = true;
                user.BranchID = branchId;
                user.ModifiedBy = modifiedBy;
                user.ModifiedOn = DateTime.UtcNow;

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
