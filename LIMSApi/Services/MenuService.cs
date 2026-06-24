using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _MenuRepository;
        private readonly ILogger<MenuService> _logger;

        public MenuService(IMenuRepository MenuRepo, ILogger<MenuService> logger)
        {
            _MenuRepository = MenuRepo;
            _logger = logger;
        }

        public async Task CreateMenu(MenuMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new ArgumentException("Menu title is required!");

            if (await _MenuRepository.ExistsByName(model.Title))
                throw new InvalidOperationException("Menu already exists!");

            await AddMenuRecursive(model, null);
            _logger.LogInformation("Menu '{MenuName}' created successfully.", model.Title);
        }


        public async Task ModifyMenu(MenuMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Menu ID is missing!");

            var existing = await _MenuRepository.GetMenuById(model.ID);

            if (existing == null)
                throw new InvalidOperationException("Menu not found!");

            // Update root menu fields
            existing.Title = model.Title;
            existing.Icon = model.Icon;
            existing.IsExpanded = model.SubMenu.Count > 0 ? true : false;
            existing.Route = model.Route;
            existing.Color = model.Color;

            // Upsert SubMenus
            await UpsertSubMenus(existing, model.SubMenu.ToList());

            await _MenuRepository.UpdateMenu(existing);

            _logger.LogInformation("Menu '{MenuName}' updated successfully.", model.Title);
        }

        private async Task UpsertSubMenus(MenuMaster parent, List<MenuMaster> newSubMenus)
        {
            // Existing children in DB
            var existingChildren = parent.SubMenu.ToList();

            // Process incoming SubMenus
            foreach (var newChild in newSubMenus)
            {
                var existingChild = existingChildren.FirstOrDefault(c => c.ID == newChild.ID);

                if (existingChild != null)
                {
                    // Update existing child
                    existingChild.Title = newChild.Title;
                    existingChild.Icon = newChild.Icon;
                    existingChild.IsExpanded = newChild?.SubMenu?.Count > 0 ? true : false;
                    existingChild.Route = newChild.Route;
                    existingChild.Color = newChild.Color;

                    // Recursive Upsert for SubMenus of this child
                    await UpsertSubMenus(existingChild, newChild.SubMenu?.Count > 0 ? newChild.SubMenu.ToList() : new List<MenuMaster>());

                    // Remove from existingChildren list to track which ones are left (for deletion)
                    existingChildren.Remove(existingChild);
                }
                else
                {
                    // New child, add it
                    var newMenu = new MenuMaster
                    {
                        Title = newChild.Title,
                        Icon = newChild.Icon,
                        IsExpanded = newChild?.SubMenu?.Count > 0 ? true : false,
                        Route = newChild.Route,
                        Color = newChild.Color,
                        ParentID = parent.ID,
                        SubMenu = new List<MenuMaster>()
                    };

                    parent.SubMenu.Add(newMenu);

                    // Recursive insert for its SubMenus
                    await UpsertSubMenus(newMenu, newChild.SubMenu?.Count > 0 ? newChild.SubMenu.ToList() : new List<MenuMaster>());
                }
            }

            // Delete remaining children that are not in the new list
            if (existingChildren.Any())
            {
                foreach (var item in existingChildren)
                {
                    parent.SubMenu?.Remove(item);
                }

            }
        }

        public async Task RemoveMenu(long id)
        {
            var existingMenu = await _MenuRepository.GetMenuById(id);
            if (existingMenu == null)
                throw new InvalidOperationException("Menu not found!");


            await _MenuRepository.UpdateMenu(existingMenu);
            _logger.LogInformation("Menu with ID '{MenuId}' deleted successfully.", id);
        }

        public async Task<MenuMaster> GetMenuDetails(long id)
        {
            var classification = await _MenuRepository.GetMenuById(id);
            if (classification == null)
                throw new InvalidOperationException("Menu not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchMenuList(PageFilter filter)
        {
            return await _MenuRepository.GetAllMenus(filter);
        }
        public async Task<PagedResponse<object>> FetchSubMenuList(PageFilter filter)
        {
            return await _MenuRepository.GetAllSubMenus(filter);
        }

        public async Task<List<CustomDropdown>> GetMenuDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _MenuRepository.GetMenuDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<List<CustomDropdown>> GetSubMenuDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _MenuRepository.GetSubMenuDropdown(searchTerm, pageNo, pageSize);
        }

        private async Task AddMenuRecursive(MenuMaster dto, long? parentId)
        {
            var entity = new MenuMaster
            {
                Title = dto.Title,
                Icon = dto.Icon,
                IsExpanded = dto.IsExpanded,
                Route = dto.Route,
                Color = dto.Color,
                ParentID = parentId
            };

            entity = await _MenuRepository.AddMenu(entity);

            foreach (var child in dto.SubMenu)
            {
                await AddMenuRecursive(child, entity.ID);
            }
        }

        public Task<bool> UpdateMenuPermission(long menuId, List<PermissionMaster> updatedPermissions)
        {
            _logger.LogInformation("Updating permissions for menu ID {MenuId}", menuId);
            return _MenuRepository.UpdateMenuPermission(menuId, updatedPermissions);
        }

        public Task<List<PermissionMaster>> GetMenuPermissions(long menuId)
        {
            _logger.LogInformation("Fetching permissions for menu ID {MenuId}", menuId);
            return _MenuRepository.GetMenuPermissions(menuId);
        }

        public Task<List<MenuMaster>> GetMenuTree()
        {
            return _MenuRepository.GetMenuTree();
        }
    }
}
