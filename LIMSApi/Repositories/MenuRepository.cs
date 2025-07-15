using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly LIMSContext _context;

        public MenuRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddMenu(MenuMaster model)
        {
            await _context.MenuMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMenu(long id)
        {
            var existingMenu = await _context.MenuMasters.FirstOrDefaultAsync(x => x.ID == id);
            if (existingMenu != null)
            {
                _context.MenuMasters.Update(existingMenu);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteMenuTree(long rootId)
        {
            var root = await _context.MenuMasters
                .Include(m => m.SubMenu)
                .FirstOrDefaultAsync(m => m.ID == rootId);

            if (root != null)
            {
                _context.MenuMasters.Remove(root);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<MenuMaster?> GetMenuById(long id)
        {
            return await _context.MenuMasters.FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task UpdateMenu(MenuMaster model)
        {
            _context.MenuMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllMenus(PageFilter filter)
        {
            var _query = from c in _context.MenuMasters where c.ParentID == null select c;

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Title != null && x.Title.ToLower().Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            // Total Records Count
            int totalRecords = await _query.CountAsync();

            // Apply Pagination
            var items = await _query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(items.Cast<object>().ToList(), totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<List<DropdwonSelector>> GetMenuDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.MenuMasters select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Title != null && x.Title.ToLower().Contains(search)));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Title,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.MenuMasters.AnyAsync(x => x.Title == name);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.MenuMasters.AnyAsync(x => x.Title == name && x.ID != Id);
        }
    }
}
