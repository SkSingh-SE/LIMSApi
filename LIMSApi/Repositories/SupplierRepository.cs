using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public SupplierRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddSupplier(SupplierMaster model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.SupplierMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSupplier(long id)
        {
            var existingSupplier = await _context.SupplierMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingSupplier != null)
            {
                existingSupplier.IsActive = false;
                existingSupplier.ModifiedOn = DateTime.UtcNow;
                _context.SupplierMasters.Update(existingSupplier);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SupplierMaster?> GetSupplierById(long id)
        {
            return await _context.SupplierMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateSupplier(SupplierMaster model)
        {
            _context.SupplierMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllSuppliers(PageFilter filter)
        {
            var _query = from c in _context.SupplierMasters where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode 
                         select new
                         {
                             c.ID,
                             c.Name,
                             c.ProductType,
                             c.ContactPerson1,
                             c.ContactNo1,
                             c.EmailId1,
                             c.Address,

                         };
            _query = _query.ApplyFilters(filter.Filter);

           

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)));
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

        public async Task<List<DropdwonSelector>> GetSupplierDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.SupplierMasters where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x =>  (x.Name != null && x.Name.ToLower().Contains(search)));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.SupplierMasters.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.SupplierMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
