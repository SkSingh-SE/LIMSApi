using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public UserPermissionRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }


        public async Task<List<MenuPermissionGroupDto>> GetUserPermissions(long userId)
        {
            var user = await _context.UserMasters.Include(u => u.Role).FirstOrDefaultAsync(u => u.ID == userId);
            if (user == null) throw new InvalidOperationException("User Not Found");

            var rolePermissionIds = await _context.RoleMenuMappings
                .Where(rm => rm.RoleID == user.RoleID)
                .SelectMany(rm => _context.PermissionMasters.Where(p => p.MenuID == rm.MenuID).Select(p => p.ID))
                .ToListAsync();

            var userOverrides = await _context.UserPermissions
                .Where(up => up.UserID == userId)
                .ToDictionaryAsync(up => up.PermissionID, up => up.IsGranted);

            var permissions = await _context.PermissionMasters
                .Include(p => p.Menu)
                .Select(p => new
                {
                    p.ID,
                    p.DisplayName,
                    p.Type,
                    p.MenuID,
                    MenuTitle = p.Menu.Title,
                    IsGranted = userOverrides.ContainsKey(p.ID) ? userOverrides[p.ID] : rolePermissionIds.Contains(p.ID),
                    IsOverride = userOverrides.ContainsKey(p.ID)
                })
                .ToListAsync();

            return permissions
                            .GroupBy(p => p.MenuTitle)
                            .Select(g => new MenuPermissionGroupDto
                            {
                                MenuTitle = g.Key,
                                Permissions = g.Select(p => new PermissionDto
                                {
                                    ID = p.ID,
                                    DisplayName = p.DisplayName,
                                    Type = p.Type,
                                    MenuID = p.MenuID,
                                    MenuTitle = p.MenuTitle,
                                    IsGranted = p.IsGranted,
                                    IsOverride = p.IsOverride
                                }).ToList()
                            }).ToList();


        }

        public async Task SaveUserPermissions(long userId, List<UserPermissionUpdateDto> updatedPermissions)
        {
            var existingOverrides = await _context.UserPermissions
                .Where(up => up.UserID == userId)
                .ToListAsync();

            foreach (var dto in updatedPermissions)
            {
                var existing = existingOverrides.FirstOrDefault(e => e.PermissionID == dto.PermissionID);

                if (dto.IsOverride)
                {
                    if (existing != null)
                    {
                        // Update existing override
                        existing.IsGranted = dto.IsGranted;
                        _context.UserPermissions.Update(existing);
                    }
                    else
                    {
                        // Add new override
                        _context.UserPermissions.Add(new UserPermission
                        {
                            UserID = userId,
                            PermissionID = dto.PermissionID,
                            IsGranted = dto.IsGranted,
                            CreatedBy = loggedInUser.EmployeeID,
                            CreatedOn = DateTime.UtcNow,
                            CompanyCode = loggedInUser.CompanyCode
                        });
                    }
                }
                else
                {
                    // Remove override if it exists (user falls back to role permission)
                    if (existing != null)
                    {
                        _context.UserPermissions.Remove(existing);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

    }
}
