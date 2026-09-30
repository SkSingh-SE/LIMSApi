using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ParameterUnitRepository : IParameterUnitRepository
    {
        private readonly LIMSContext _context;

        public ParameterUnitRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddParameterUnit(ParameterUnitMaster model)
        {
            await _context.ParameterUnitMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteParameterUnit(long id)
        {
            var existingParameterUnit = await _context.ParameterUnitMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingParameterUnit != null)
            {
                existingParameterUnit.IsActive = false;
                existingParameterUnit.ModifiedOn = DateTime.UtcNow;
                _context.ParameterUnitMasters.Update(existingParameterUnit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ParameterUnitMaster?> GetParameterUnitById(long id)
        {
            return await _context.ParameterUnitMasters
                .Include(x => x.Equivalents.Where(e => e.IsActive).OrderBy(e => e.DisplayOrder))
                .FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task UpdateParameterUnit(ParameterUnitMaster model)
        {
            _context.ParameterUnitMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllParameterUnits(PageFilter filter)
        {
            var _query = _context.ParameterUnitMasters.AsNoTracking().AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    (x.Code != null && x.Code.Contains(search))
                    || (x.Name != null && x.Name.Contains(search))
                    || (x.Symbol != null && x.Symbol.Contains(search))
                    || (x.QuantityType != null && x.QuantityType.Contains(search))
                    || (x.Description != null && x.Description.Contains(search))
                    || (x.ConversionFactor != null && x.ConversionFactor.ToString().Contains(search))
                    || x.Equivalents.Any(e => e.IsActive && e.Name.Contains(search))
                );
            }
            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            else
            {
                _query = _query.OrderBy(x => x.Name);
            }

            var projected = _query.Select(x => new
            {
                x.ID,
                x.Code,
                x.Name,
                x.Symbol,
                x.QuantityType,
                x.Description,
                x.ConversionFactor,
                x.CreatedBy,
                x.CreatedOn,
                x.ModifiedBy,
                x.ModifiedOn,
                x.IsActive,
                Equivalents = x.Equivalents
                    .Where(e => e.IsActive)
                    .OrderBy(e => e.DisplayOrder)
                    .Select(e => new
                    {
                        e.ID,
                        e.Name,
                        e.ConversionFactor,
                        e.DisplayOrder,
                        e.IsActive
                    }).ToList()
            });

            return await projected.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetParameterUnitDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.ParameterUnitMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => 
                        (x.Name != null && x.Name.Contains(search))
                        || (x.Code != null && x.Code.Contains(search))
                        || (x.Symbol != null && x.Symbol.Contains(search))
                    );
                }
            }

            var skip = pageNo * pageSize;

            var data = await (_query.OrderBy(x => x.Name).Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = !string.IsNullOrEmpty(x.Symbol) && x.Symbol != x.Name ? $"{x.Name} ({x.Symbol})" : x.Name,
            })).ToListAsync();

            return data;
        }

        public async Task<List<GroupedUnitDropdownOption>> GetGroupedParameterUnitDropdown(string? searchTerm, int pageNo = 0, int pageSize = 50)
        {
            if (pageNo < 0) pageNo = 0;

            var query = _context.ParameterUnitMasters
                .AsNoTracking()
                .Include(x => x.Equivalents)
                .Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    query = query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    query = query.Where(x =>
                        (x.Name != null && x.Name.Contains(search)) ||
                        x.Equivalents.Any(e => e.IsActive && e.Name != null && e.Name.Contains(search))
                    );
                }
            }

            var baseUnits = await query
                .OrderBy(x => x.Name)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<GroupedUnitDropdownOption>();

            foreach (var baseUnit in baseUnits)
            {
                var activeEquivalents = baseUnit.Equivalents
                    .Where(e => e.IsActive)
                    .OrderBy(e => e.DisplayOrder ?? int.MaxValue)
                    .ThenBy(e => e.Name)
                    .ToList();

                // 1. Group Header Item (Id = 0 so it never matches Unit ID lookups)
                result.Add(new GroupedUnitDropdownOption
                {
                    Id = 0,
                    Name = baseUnit.Name.ToUpper(),
                    GroupName = baseUnit.Name,
                    IsHeader = true,
                    IsChild = false,
                    IsBase = false
                });

                // 2. Base Unit Option
                result.Add(new GroupedUnitDropdownOption
                {
                    Id = baseUnit.ID,
                    EquivalentId = null,
                    Name = baseUnit.Name,
                    GroupName = baseUnit.Name,
                    IsHeader = false,
                    IsChild = true,
                    IsBase = true,
                    ConversionFactor = baseUnit.ConversionFactor
                });

                // 3. Child Equivalent Options
                foreach (var eq in activeEquivalents)
                {
                    result.Add(new GroupedUnitDropdownOption
                    {
                        Id = baseUnit.ID,
                        EquivalentId = eq.ID,
                        Name = eq.Name,
                        GroupName = baseUnit.Name,
                        IsHeader = false,
                        IsChild = true,
                        IsBase = false,
                        ConversionFactor = eq.ConversionFactor
                    });
                }
            }

            return result;
        }

        public async Task<bool> ExistsByCode(string code)
        {
            var c = code.Trim().ToLower();
            return await _context.ParameterUnitMasters.AnyAsync(x => x.Code.ToLower() == c && x.IsActive);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long id)
        {
            var c = code.Trim().ToLower();
            return await _context.ParameterUnitMasters.AnyAsync(x => x.Code.ToLower() == c && x.ID != id && x.IsActive);
        }

        public async Task<bool> ExistsByName(string name)
        {
            var n = name.Trim().ToLower();
            return await _context.ParameterUnitMasters.AnyAsync(x => x.Name.ToLower() == n && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            var n = name.Trim().ToLower();
            return await _context.ParameterUnitMasters.AnyAsync(x => x.Name.ToLower() == n && x.ID != Id && x.IsActive);
        }

        public Task<List<string>> GetQuantityTypes()
        {
            var types = new List<string>
            {
                "Length",
                "Mass",
                "Force",
                "Pressure",
                "Temperature",
                "Voltage",
                "Current",
                "Resistance",
                "Density",
                "Concentration",
                "Percentage",
                "Time",
                "Area",
                "Volume",
                "Dimensionless",
                "Speed",
                "Energy",
                "Hardness",
                "Other"
            };
            return Task.FromResult(types);
        }
    }
}
