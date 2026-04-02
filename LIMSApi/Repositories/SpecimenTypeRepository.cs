using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SpecimenTypeRepository : ISpecimenTypeRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;
        public SpecimenTypeRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<SpecimenTypeMaster> AddSpecimenType(SpecimenTypeMaster model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.SpecimenTypeMasters.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<SpecimenTypeMaster> DeleteSpecimenType(long id)
        {
            var existingSpecimenType = await _context.SpecimenTypeMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (existingSpecimenType != null)
            {
                existingSpecimenType.IsActive = false;
                _context.SpecimenTypeMasters.Update(existingSpecimenType);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Specimen Type not found!");
            }
            return existingSpecimenType;
        }

        public async Task<SpecimenTypeMaster?> GetSpecimenTypeById(long id)
        {
            return await _context.SpecimenTypeMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<SpecimenTypeMaster> UpdateSpecimenType(SpecimenTypeMaster model)
        {
            _context.SpecimenTypeMasters.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<PagedResponse<object>> GetAllSpecimenTypes(PageFilter filter)
        {
            var _query = from c in _context.SpecimenTypeMasters where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode select c;

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)));
            }
            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetSpecimenTypeDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.SpecimenTypeMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)) || x.ID.ToString().Contains(search));
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
            return await _context.SpecimenTypeMasters.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.SpecimenTypeMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive&& x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
