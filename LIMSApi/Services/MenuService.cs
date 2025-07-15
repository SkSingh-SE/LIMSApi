using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

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

            // Delete existing children (optional: use soft delete instead)
            await _MenuRepository.DeleteMenuTree(model.ID);

            // Update root menu
            existing.Title = model.Title;
            existing.Icon = model.Icon;
            existing.IsExpanded = model.IsExpanded;
            existing.Route = model.Route;
            existing.Color = model.Color;
            await _MenuRepository.UpdateMenu(existing);

            // Add updated children
            foreach (var child in model.SubMenu)
            {
                await AddMenuRecursive(child, existing.ID);
            }

            _logger.LogInformation("Menu '{MenuName}' updated successfully.", model.Title);
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

        public async Task<List<DropdwonSelector>> GetMenuDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _MenuRepository.GetMenuDropdown(searchTerm, pageNo, pageSize);
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

            await _MenuRepository.AddMenu(entity);

            foreach (var child in dto.SubMenu)
            {
                await AddMenuRecursive(child, entity.ID);
            }
        }

    }
}
