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
                    IsGranted = userOverrides.ContainsKey(p.ID) ? userOverrides[p.ID] : rolePermissionIds.Contains(p.ID)     // role baseline
                })
                .ToListAsync();
            //var userMenus =  await GetUserMenusWithPermissions(userId);

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
            var user = await _context.UserMasters
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.ID == userId);

            if (user == null)
                throw new Exception("User not found!");

            // 1️ Get SubMenu IDs assigned to User's Role
            var roleSubMenuIds = await _context.RoleMenuMappings
                .Where(rm => rm.RoleID == user.RoleID)
                .Select(rm => rm.MenuID)
                .Distinct()
                .ToListAsync();

            // 2️ Get additional Menu IDs from User Special Permissions (even if not in role)
            var userSpecialMenuIds = await _context.UserPermissions
                .Include(up => up.Permission)
                .ThenInclude(p => p.Menu)
                .Where(up => up.UserID == userId && up.IsGranted == true)
                .Select(up => (long)up.Permission.MenuID)
                .Distinct()
                .ToListAsync();

            // Combine both
            var effectiveMenuIds = roleSubMenuIds
                .Union(userSpecialMenuIds)
                .Distinct()
                .ToList();

            // 3️ Fetch SubMenus with their Parent Menu Info
            var subMenus = await _context.MenuMasters
                .Where(m => effectiveMenuIds.Contains(m.ID))
                .Select(m => new
                {
                    Menu = m,
                    ParentMenu = m.ParentID != null ? m.Parent : null
                })
                .ToListAsync();

            // 4️ Collect all unique Menu IDs (SubMenus + Parent Menus)
            var allMenuIds = subMenus
                .Select(m => m.Menu.ID)
                .Union(subMenus.Where(m => m.ParentMenu != null).Select(m => m.ParentMenu.ID))
                .Distinct()
                .ToList();

            // 5️ Fetch Permissions from PermissionMaster for effective menus
            var roleMenuPermissions = await _context.PermissionMasters
                .Include(pm => pm.Menu)
                 .Where(pm => effectiveMenuIds.Contains((long)pm.MenuID) && !userSpecialMenuIds.Contains((long)pm.MenuID))
                .Select(pm => new
                {
                    Menu = pm.Menu,
                    PermissionName = pm.Name
                })
                .ToListAsync();

            // 6️ Get User-specific Menu Permissions (Special Permissions)
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

            // 7️ Combine Role & User Permissions
            var combinedPermissions = roleMenuPermissions.Concat(userSpecialPermissions)
                .GroupBy(x => x.Menu.ID)
                .Select(g => new
                {
                    Menu = g.First().Menu,
                    Permissions = g.Select(x => x.PermissionName).Distinct().ToList()
                })
                .ToList();

            // 8️ Build Menu Dictionary for Hierarchy
            var menuMasters = await _context.MenuMasters
                .Where(m => allMenuIds.Contains(m.ID))
                .ToListAsync();

            var menuDict = new Dictionary<long, UserMenuDTO>();

            foreach (var menu in menuMasters)
            {
                bool isFromRole = roleSubMenuIds.Contains(menu.ID);
                menuDict[menu.ID] = new UserMenuDTO
                {
                    ID = menu.ID,
                    Title = menu.Title,
                    Route = menu.Route,
                    ParentMenuID = menu.ParentID,
                    Permissions = combinedPermissions
                        .FirstOrDefault(x => x.Menu.ID == menu.ID)?.Permissions
                        ?? new List<string>(),
                    IsFromRole = isFromRole
                };
            }

            // 9️ Build Parent-Child Structure
            foreach (var menu in menuDict.Values)
            {
                if (menu.ParentMenuID != null && menuDict.ContainsKey(menu.ParentMenuID.Value))
                {
                   // menuDict[menu.ParentMenuID.Value].Children.Add(menu);
                    var parent = menuDict[menu.ParentMenuID.Value];
                    parent.Children.Add(menu);

                    // If all children are role-based, mark parent as from role too
                    if (menu.IsFromRole && !parent.IsFromRole)
                        parent.IsFromRole = true;
                }
            }

            // 10 Return Only Root Menus
            var rootMenus = menuDict.Values
                .Where(m => m.ParentMenuID == null)
                .OrderBy(m => m.Title)
                .ToList();

            return rootMenus;
        }

        public async Task SaveUserPermissions(long userId, List<UserPermissionUpdateDto> updatedPermissions)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var userRoleId = await _context.UserMasters
                    .Where(u => u.ID == userId)
                    .Select(u => u.RoleID)
                    .FirstAsync();

                var rolePermissionIds = await (
                    from rm in _context.RoleMenuMappings
                    join pm in _context.PermissionMasters on rm.MenuID equals pm.MenuID
                    where rm.RoleID == userRoleId
                    select pm.ID
                ).ToHashSetAsync();

                var existingOverrides = await _context.UserPermissions
                    .Where(up => up.UserID == userId)
                    .ToDictionaryAsync(up => up.PermissionID);

                foreach (var dto in updatedPermissions)
                {
                    bool roleGrants = rolePermissionIds.Contains(dto.PermissionID);
                    existingOverrides.TryGetValue(dto.PermissionID, out var existing);

                    // Same as role → remove override
                    if (roleGrants && dto.IsGranted)
                    {
                        if (existing != null)
                            _context.UserPermissions.Remove(existing);

                        continue;
                    }

                    // Different from role → store override
                    if (existing == null)
                    {
                        _context.UserPermissions.Add(new UserPermission
                        {
                            UserID = userId,
                            PermissionID = dto.PermissionID,
                            IsGranted = dto.IsGranted
                        });
                    }
                    else
                    {
                        existing.IsGranted = dto.IsGranted;
                    }
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        public async Task<List<DropdwonSelector>> GetPermissionDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.PermissionMasters select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.DisplayName != null && x.DisplayName.ToLower().Contains(search))
                                      || (x.Name != null && x.Name.ToLower().Contains(search))
                                      || x.ID.ToString().Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.DisplayName,
            })).ToListAsync();

            return data;
        }
    }
}
