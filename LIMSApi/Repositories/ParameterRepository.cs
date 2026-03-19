using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ParameterRepository : IParameterRepository
    {
        private readonly LIMSContext _context;

        public ParameterRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddParameter(ParameterMaster model)
        {
            await _context.ParameterMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteParameter(long id)
        {
            var existingParameter = await _context.ParameterMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingParameter != null)
            {
                existingParameter.IsActive = false;
                existingParameter.ModifiedOn = DateTime.UtcNow;
                _context.ParameterMasters.Update(existingParameter);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ParameterMaster?> GetParameterById(long id)
        {
            return await _context.ParameterMasters
                .Include(p => p.ParameterUnit)
                .Include(p => p.DefaultTestMethod)
                .Include(p => p.ParameterCategory)
                .Include(p => p.AllowedOrientations).ThenInclude(ao => ao.SpecimenOrientation)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateParameter(ParameterMaster model)
        {
            _context.ParameterMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllChemicalParameters(PageFilter filter)
        {
            var _query = (from c in _context.ParameterMasters
                          join u in _context.ParameterUnitMasters on c.ParameterUnitID equals u.ID
                          join cat in _context.ParameterCategoryMasters on c.ParameterCategoryID equals cat.ID into catGroup
                          from cat in catGroup.DefaultIfEmpty()
                          where c.IsActive && c.ParameterType == "Chemical"
                          select new
                          {
                              c.ID,
                              c.Name,
                              c.Code,
                              c.Symbol,
                              c.AliasName,
                              c.ElementType,
                              UnitName = u.Name,
                              Factor = u.ConversaionFactor,
                              CategoryName = cat != null ? cat.Name : "",
                              c.DecimalPrecision,
                              c.CreatedOn
                          }).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)) || (x.Code != null && x.Code.ToLower().Contains(search)) || (x.Symbol != null && x.Symbol.ToLower().Contains(search)));
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
        public async Task<PagedResponse<object>> GetAllMechanicalParameters(PageFilter filter)
        {
            var _query = (from c in _context.ParameterMasters
                          join u in _context.ParameterUnitMasters on c.ParameterUnitID equals u.ID
                          join cat in _context.ParameterCategoryMasters on c.ParameterCategoryID equals cat.ID into catGroup
                          from cat in catGroup.DefaultIfEmpty()
                          where c.IsActive && c.ParameterType == "Mechanical"
                          select new
                          {
                              c.ID,
                              c.Name,
                              c.Code,
                              c.AliasName,
                              c.ElementType,
                              UnitName = u.Name,
                              Factor = u.ConversaionFactor,
                              CategoryName = cat != null ? cat.Name : "",
                              c.DecimalPrecision,
                              c.CreatedOn
                          }).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)) || (x.Code != null && x.Code.ToLower().Contains(search)));
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
        public async Task<PagedResponse<object>> ParameterList(PageFilter filter)
        {
            var _query = (from c in _context.ParameterMasters
                          join u in _context.ParameterUnitMasters on c.ParameterUnitID equals u.ID
                          where c.IsActive
                          select new
                          {
                              c.ID,
                              c.Name,
                              c.AliasName,
                              c.ElementType,
                              ParameterType = c.ParameterType,
                              UnitName = u.Name,
                              Min = 0, // Placeholder for Min value, replace with actual value if available
                              Max = 0, // Placeholder for Max value, replace with actual value if available
                              Factor = u.ConversaionFactor,
                              c.CreatedOn
                          }).AsQueryable().ApplyFilters(filter.Filter);

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


        public async Task<List<DropdwonSelector>> GetParameterDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.ParameterMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)) || x.ID.ToString().Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.Name} - ({x.ParameterType})",
            })).ToListAsync();

            return data;
        }
        public async Task<List<DropdwonSelector>> GetChemicalParameterDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.ParameterMasters 
                         join u in _context.ParameterUnitMasters on a.ParameterUnitID equals u.ID
                         where a.IsActive && a.ParameterType == "Chemical" select new
                         {
                             a.Name,
                             a.ID,
                             a.ParameterType,
                             unitID = a.ParameterUnitID,
                             unit = u.Name
                         };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)) || x.ID.ToString().Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.Name}",
                AdditionalValues = new Dictionary<string, object>
                {
                    { "Unit", x.unit},
                    { "UnitID", x.unitID }
                }
            })).ToListAsync();

            return data;
        }
        public async Task<List<DropdwonSelector>> GetMechanicalParameterDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.ParameterMasters
                         join u in _context.ParameterUnitMasters on a.ParameterUnitID equals u.ID into uGroup
                         from u in uGroup.DefaultIfEmpty()
                         where a.IsActive && a.ParameterType == "Mechanical"
                         select new
                         {
                             a.Name,
                             a.ID,
                             unitID = a.ParameterUnitID,
                             unit = u != null ? u.Name : ""
                         };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.Name != null && x.Name.ToLower().Contains(search)) || x.ID.ToString().Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.Name}",
                AdditionalValues = new Dictionary<string, object>
                {
                    { "Unit", x.unit },
                    { "UnitID", x.unitID }
                }
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.ParameterMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.ParameterMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }

        public async Task<bool> ExistsByCode(string code)
        {
            return await _context.ParameterMasters.AnyAsync(x => x.Code == code && x.IsActive);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long Id)
        {
            return await _context.ParameterMasters.AnyAsync(x => x.Code == code && x.ID != Id && x.IsActive);
        }
    }
}
