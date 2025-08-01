using System.Linq;
using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.AspNetCore.Http.Metadata;
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

        public async Task<List<MenuPermissionGroupDto>> GetAllPermissions()
        {
            var permissions = await _context.PermissionMasters
                .Include(p => p.Menu)
                .Select(p => new
                {
                    p.ID,
                    p.DisplayName,
                    p.Type,
                    p.MenuID,
                    MenuTitle = p.Menu.Title
                })
                .ToListAsync();

            return permissions.GroupBy(p => p.MenuTitle)
                            .Select(g => new MenuPermissionGroupDto
                            {
                                MenuTitle = g.Key,
                                Permissions = g.Select(p => new PermissionDto
                                {
                                    ID = p.ID,
                                    DisplayName = p.DisplayName,
                                    Type = p.Type,
                                    MenuID = p.MenuID,
                                    MenuTitle = p.MenuTitle
                                }).ToList()
                            }).ToList();
        }

        public async Task<List<MenuPermissionGroupDto>> GetUserPermissions(long userId)
        {
            var user = await _context.UserMasters
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.ID == userId);

            if (user == null)
                throw new InvalidOperationException("User Not Found");

            // Get menu IDs from role
            var roleMenuIds = await _context.RoleMenuMappings
                .Where(rm => rm.RoleID == user.RoleID)
                .Select(rm => rm.MenuID)
                .ToListAsync();

            // Get permission IDs from role menus
            var rolePermissionIds = await _context.PermissionMasters
                .Where(p => roleMenuIds.Contains((long)p.MenuID))
                .Select(p => p.ID)
                .ToListAsync();

            // Get permission overrides by user
            var userOverrides = await _context.UserPermissions
                .Where(up => up.UserID == userId)
                .ToDictionaryAsync(up => up.PermissionID, up => up.IsGranted);

            // Combine permission IDs: role-based + overrides
            var relevantPermissionIds = rolePermissionIds
                .Union(userOverrides.Keys)
                .Distinct()
                .ToList();

            // Fetch only those relevant permissions
            var permissions = await _context.PermissionMasters
                .Include(p => p.Menu)
                .Where(p => relevantPermissionIds.Contains(p.ID))
                .Select(p => new
                {
                    p.ID,
                    p.DisplayName,
                    p.Type,
                    p.MenuID,
                    MenuTitle = p.Menu.Title,
                    IsOverride = userOverrides.ContainsKey(p.ID),
                    IsGranted = userOverrides.ContainsKey(p.ID)
                                ? userOverrides[p.ID]
                                : true // role-based ones are granted by default
                })
                .ToListAsync();
           await GetUserMenusWithPermissions(userId);

            // Group by menu
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

        public async Task<List<UserMenuDTO>> GetUserMenusWithPermissions(long userId)
        {
            var user = await _context.UserMasters.Include(u => u.Role).FirstOrDefaultAsync(u => u.ID == userId);
            if (user == null) throw new Exception("User not found!");

            // 1. Get SubMenu IDs assigned to User's Role
            var roleSubMenuIds = await _context.RoleMenuMappings
                .Where(rm => rm.RoleID == user.RoleID)
                .Select(rm => rm.MenuID)
                .Distinct()
                .ToListAsync();

            // 2. Fetch SubMenus with their Parent Menu Info
            var subMenus = await _context.MenuMasters
                .Where(m => roleSubMenuIds.Contains(m.ID))
                .Select(m => new
                {
                    Menu = m,
                    ParentMenu = m.ParentID != null ? m.Parent : null
                })
                .ToListAsync();

            // 3. Collect all unique Menu IDs (SubMenus + Parent Menus)
            var allMenuIds = subMenus
                .Select(m => m.Menu.ID)
                .Union(subMenus.Where(m => m.ParentMenu != null).Select(m => m.ParentMenu.ID))
                .Distinct()
                .ToList();

            // 4. Fetch Permissions from PermissionMaster for SubMenus
            var roleMenuPermissions = await _context.PermissionMasters
                .Include(pm => pm.Menu)
                .Where(pm => roleSubMenuIds.Contains((long)pm.MenuID))
                .Select(pm => new
                {
                    Menu = pm.Menu,
                    PermissionName = pm.Name
                })
                .ToListAsync();

            // 5. Get User-specific Menu Permissions (Special Permissions)
            var userSpecialPermissions = await _context.UserPermissions
                .Include(up => up.Permission)
                .ThenInclude(p => p.Menu)
                .Where(up => up.UserID == userId)
                .Select(up => new
                {
                    Menu = up.Permission.Menu,
                    PermissionName = up.Permission.Name
                })
                .ToListAsync();

            // 6. Combine Role & User Permissions
            var combinedPermissions = roleMenuPermissions.Concat(userSpecialPermissions)
                .GroupBy(x => x.Menu.ID)
                .Select(g => new
                {
                    Menu = g.First().Menu,
                    Permissions = g.Select(x => x.PermissionName).Distinct().ToList()
                })
                .ToList();

            // 7. Build Menu Dictionary for Hierarchy
            var menuMasters = await _context.MenuMasters
                .Where(m => allMenuIds.Contains(m.ID))
                .ToListAsync();

            var menuDict = new Dictionary<long, UserMenuDTO>();

            foreach (var menu in menuMasters)
            {
                menuDict[menu.ID] = new UserMenuDTO
                {
                    ID = menu.ID,
                    Title = menu.Title,
                    Route = menu.Route,
                    ParentMenuID = menu.ParentID,
                    Permissions = combinedPermissions.FirstOrDefault(x => x.Menu.ID == menu.ID)?.Permissions ?? new List<string>()
                };
            }

            // 8. Build Parent-Child Structure
            foreach (var menu in menuDict.Values)
            {
                if (menu.ParentMenuID != null && menuDict.ContainsKey(menu.ParentMenuID.Value))
                {
                    menuDict[menu.ParentMenuID.Value].Children.Add(menu);
                }
            }

            // 9. Return Only Root Menus
            var rootMenus = menuDict.Values.Where(m => m.ParentMenuID == null).ToList();

            return rootMenus;
        }

        public async Task SaveUserPermissions(long userId, List<UserPermissionUpdateDto> updatedPermissions)
        {
            var permissionIds = updatedPermissions.Select(x => x.PermissionID).ToList();
            // Step 1: Get user's RoleID
            var userRoleId = await _context.UserMasters
                .Where(u => u.ID == userId)
                .Select(u => u.RoleID)
                .FirstOrDefaultAsync();

            // Step 2: Get permission IDs from that role
            var rolePermissionIds = await (
                from m in _context.RoleMenuMappings
                join p in _context.PermissionMasters on m.MenuID equals p.MenuID
                where m.RoleID == userRoleId
                select p.ID
            ).Distinct().ToListAsync();



            // Step 2: Get existing overrides
            var existingOverrides = await _context.UserPermissions
                .Where(up => up.UserID == userId)
                .ToListAsync();

            var currentUser = loggedInUser;
            var now = DateTime.UtcNow;

            // Step 3: Handle all possible overrides
            var allPermissionIds = permissionIds.Union(existingOverrides.Select(o => o.PermissionID)).Distinct();

            foreach (var permissionId in allPermissionIds)
            {
                var isInSubmittedList = permissionIds.Contains(permissionId);
                var isInRole = rolePermissionIds.Contains(permissionId);
                var existing = existingOverrides.FirstOrDefault(x => x.PermissionID == permissionId);

                if (isInSubmittedList)
                {
                    // Grant explicitly if role doesn't already grant it
                    if (!isInRole)
                    {
                        if (existing == null)
                        {
                            _context.UserPermissions.Add(new UserPermission
                            {
                                UserID = userId,
                                PermissionID = permissionId,
                                IsGranted = true,
                                IsOverride = true,
                                CreatedBy = currentUser.EmployeeID,
                                CreatedOn = now,
                                CompanyCode = currentUser.CompanyCode
                            });
                        }
                        else
                        {
                            existing.IsGranted = true;
                            existing.IsOverride = true;
                            existing.ModifiedBy = currentUser.EmployeeID;
                            existing.ModifiedOn = now;
                        }
                    }
                    else
                    {
                        // Role already grants it — no override needed
                        if (existing != null)
                        {
                            _context.UserPermissions.Remove(existing);
                        }
                    }
                }
                else
                {
                    // Permission removed in UI — check if override exists and needs revoking
                    if (isInRole)
                    {
                        // Role grants it — if override exists to grant, remove it
                        if (existing != null && existing.IsGranted)
                        {
                            _context.UserPermissions.Remove(existing);
                        }
                    }
                    else
                    {
                        // Role doesn't grant — so deny override
                        if (existing == null)
                        {
                            _context.UserPermissions.Add(new UserPermission
                            {
                                UserID = userId,
                                PermissionID = permissionId,
                                IsGranted = false,
                                IsOverride = true,
                                CreatedBy = currentUser.EmployeeID,
                                CreatedOn = now,
                                CompanyCode = currentUser.CompanyCode
                            });
                        }
                        else
                        {
                            existing.IsGranted = false;
                            existing.IsOverride = true;
                            existing.ModifiedBy = currentUser.EmployeeID;
                            existing.ModifiedOn = now;
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }


    }
}
