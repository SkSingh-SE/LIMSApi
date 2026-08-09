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
                .Include(p => p.ParameterUnitEquivalent)
                .Include(p => p.DropdownOptions.Where(o => o.IsActive))
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        public async Task UpdateParameter(ParameterMaster model)
        {
            _context.ParameterMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        // ─── Chemical Parameter List ───────────────────────
        public async Task<PagedResponse<object>> GetAllChemicalParameters(PageFilter filter)
        {
            var _query = (from c in _context.ParameterMasters
                          join u in _context.ParameterUnitMasters on c.ParameterUnitID equals u.ID into unitGroup
                          from u in unitGroup.DefaultIfEmpty()
                          join eq in _context.ParameterUnitEquivalents on c.ParameterUnitEquivalentID equals eq.ID into eqGroup
                          from eq in eqGroup.DefaultIfEmpty()
                          where c.IsActive && c.ParameterType == "Chemical"
                          select new
                          {
                              c.ID,
                              c.Name,
                              c.Symbol,
                              c.ElementType,
                              c.InputType,
                              c.IsCalculated,
                              c.FormulaDisplay,
                              c.ParameterUnitID,
                              c.ParameterUnitEquivalentID,
                              c.UnitConversionFactor,
                              UnitName = eq != null ? eq.Name : (u != null ? u.Name : ""),
                              Factor = c.UnitConversionFactor.HasValue ? c.UnitConversionFactor.Value.ToString() : (eq != null && eq.ConversionFactor.HasValue ? eq.ConversionFactor.Value.ToString() : (u != null && u.ConversionFactor.HasValue ? u.ConversionFactor.Value.ToString() : "1")),
                              c.DecimalPrecision,
                              c.CreatedOn,
                              c.ModifiedOn
                          }).AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    (x.Name != null && x.Name.Contains(search)) ||
                    (x.Symbol != null && x.Symbol.Contains(search)) ||
                    (x.UnitName != null && x.UnitName.Contains(search)));
            }

            if (filter.SortByColumn != null)
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        // ─── Mechanical / Observation Parameter List ───────
        public async Task<PagedResponse<object>> GetAllMechanicalParameters(PageFilter filter)
        {
            var _query = (from c in _context.ParameterMasters
                          join u in _context.ParameterUnitMasters on c.ParameterUnitID equals u.ID into unitGroup
                          from u in unitGroup.DefaultIfEmpty()
                          join eq in _context.ParameterUnitEquivalents on c.ParameterUnitEquivalentID equals eq.ID into eqGroup
                          from eq in eqGroup.DefaultIfEmpty()
                          where c.IsActive && (c.ParameterType == "Mechanical" || c.ParameterType == "Observation")
                          select new
                          {
                              c.ID,
                              c.Name,
                              c.Symbol,
                              c.ParameterType,
                              c.ElementType,
                              c.InputType,
                              c.IsCalculated,
                              c.FormulaDisplay,
                              c.ParameterUnitID,
                              c.ParameterUnitEquivalentID,
                              c.UnitConversionFactor,
                              UnitName = eq != null ? eq.Name : (u != null ? u.Name : ""),
                              Factor = c.UnitConversionFactor.HasValue ? c.UnitConversionFactor.Value.ToString() : (eq != null && eq.ConversionFactor.HasValue ? eq.ConversionFactor.Value.ToString() : (u != null && u.ConversionFactor.HasValue ? u.ConversionFactor.Value.ToString() : "1")),
                              c.DecimalPrecision,
                              c.CreatedOn,
                              c.ModifiedOn
                          }).AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    (x.Name != null && x.Name.Contains(search)) ||
                    (x.Symbol != null && x.Symbol.Contains(search)) ||
                    (x.ElementType != null && x.ElementType.Contains(search)) ||
                    (x.ParameterType != null && x.ParameterType.Contains(search)) ||
                    (x.UnitName != null && x.UnitName.Contains(search)));
            }

            if (filter.SortByColumn != null)
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        // ─── All Parameters List ───────────────────────────
        public async Task<PagedResponse<object>> ParameterList(PageFilter filter)
        {
            var _query = (from c in _context.ParameterMasters
                          join u in _context.ParameterUnitMasters on c.ParameterUnitID equals u.ID into unitGroup
                          from u in unitGroup.DefaultIfEmpty()
                          where c.IsActive
                          select new
                          {
                              c.ID,
                              c.Name,
                              c.Symbol,
                              c.ElementType,
                              c.InputType,
                              ParameterType = c.ParameterType,
                              UnitName = u != null ? u.Name : "",
                              Factor = u != null && u.ConversionFactor.HasValue ? u.ConversionFactor.Value.ToString() : "1",
                              c.CreatedOn,
                              c.ModifiedOn
                          }).AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    (x.Name != null && x.Name.Contains(search))
                    || (x.Symbol != null && x.Symbol.Contains(search))
                    || (x.ElementType != null && x.ElementType.Contains(search))
                    || (x.ParameterType != null && x.ParameterType.Contains(search))
                    || (x.UnitName != null && x.UnitName.Contains(search))
                );
            }

            if (filter.SortByColumn != null)
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        // ─── Dropdown: All Parameters ───────────────────────
        public async Task<List<DropdwonSelector>> GetParameterDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20, string? elementTypes = null)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.ParameterMasters
                         join u in _context.ParameterUnitMasters on a.ParameterUnitID equals u.ID into unitGroup
                         from u in unitGroup.DefaultIfEmpty()
                         where a.IsActive
                         select new
                         {
                             a.ID,
                             a.Name,
                             a.ParameterType,
                             a.Symbol,
                             a.InputType,
                             a.DecimalPrecision,
                             a.ElementType,
                             a.IsCalculated,
                             a.Formula,
                             a.FormulaDisplay,
                             unitID = a.ParameterUnitID,
                             unit = u != null ? u.Name : "",
                             DropdownOptions = a.DropdownOptions
                                 .Where(o => o.IsActive)
                                 .OrderBy(o => o.DisplayOrder)
                                 .Select(o => new { o.DisplayText, o.Value, o.IsDefault })
                         };

            if (!string.IsNullOrWhiteSpace(elementTypes))
            {
                var typesList = elementTypes.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                           .Select(t => t.Trim().ToLower())
                                           .ToList();
                if (typesList.Any())
                    _query = _query.Where(x => x.ElementType != null && typesList.Contains(x.ElementType));
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                    _query = _query.Where(x => x.ID == exactId);
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.Name != null && x.Name.Contains(search));
                }
            }

            var skip = pageNo * pageSize;
            var data = await _query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.Name} - ({x.ParameterType})",
                AdditionalValues = new Dictionary<string, object>
                {
                    { "UnitID", x.unitID! },
                    { "Unit", x.unit ?? "" },
                    { "ParameterType", x.ParameterType ?? "" },
                    { "Symbol", x.Symbol ?? "" },
                    { "InputType", x.InputType ?? "Decimal" },
                    { "DecimalPrecision", x.DecimalPrecision },
                    { "ElementType", x.ElementType ?? "" },
                    { "IsCalculated", x.IsCalculated },
                    { "Formula", x.Formula ?? "" },
                    { "FormulaDisplay", x.FormulaDisplay ?? "" },
                    { "DropdownOptions", x.DropdownOptions.ToList() }
                }
            }).ToListAsync();

            return data;
        }

        // ─── Dropdown: Chemical ───────────────────────────
        public async Task<List<DropdwonSelector>> GetChemicalParameterDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.ParameterMasters
                         join u in _context.ParameterUnitMasters on a.ParameterUnitID equals u.ID into unitGroup
                         from u in unitGroup.DefaultIfEmpty()
                         where a.IsActive && a.ParameterType == "Chemical"
                         select new
                         {
                             a.ID,
                             a.Name,
                             a.ParameterType,
                             a.Symbol,
                             a.InputType,
                             a.DecimalPrecision,
                             a.ElementType,
                             a.IsCalculated,
                             a.Formula,
                             a.FormulaDisplay,
                             unitID = a.ParameterUnitID,
                             unit = u != null ? u.Name : "",
                              DropdownOptions = a.DropdownOptions.Where(o => o.IsActive).OrderBy(o => o.DisplayOrder).Select(o => new { o.DisplayText, o.Value, o.IsDefault })
                          };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                    _query = _query.Where(x => x.ID == exactId);
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.Name != null && x.Name.Contains(search));
                }
            }

            var skip = pageNo * pageSize;
            var data = await _query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.Name}",
                AdditionalValues = new Dictionary<string, object>
                {
                    { "UnitID", x.unitID! },
                    { "Unit", x.unit ?? "" },
                    { "ParameterType", x.ParameterType ?? "" },
                    { "Symbol", x.Symbol ?? "" },
                    { "InputType", x.InputType ?? "Decimal" },
                    { "DecimalPrecision", x.DecimalPrecision },
                    { "ElementType", x.ElementType ?? "" },
                    { "IsCalculated", x.IsCalculated },
                    { "Formula", x.Formula ?? "" },
                    { "FormulaDisplay", x.FormulaDisplay ?? "" },
                    { "DropdownOptions", x.DropdownOptions.ToList() }
                }
            }).ToListAsync();

            return data;
        }

        // ─── Dropdown: Mechanical + Observation ───────────
        public async Task<List<DropdwonSelector>> GetMechanicalParameterDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.ParameterMasters
                         join u in _context.ParameterUnitMasters on a.ParameterUnitID equals u.ID into uGroup
                         from u in uGroup.DefaultIfEmpty()
                         where a.IsActive && (a.ParameterType == "Mechanical" || a.ParameterType == "Observation")
                         select new
                         {
                             a.ID,
                             a.Name,
                             a.ParameterType,
                             a.Symbol,
                             a.InputType,
                             a.DecimalPrecision,
                             a.ElementType,
                             a.IsCalculated,
                             a.Formula,
                             a.FormulaDisplay,
                             unitID = a.ParameterUnitID,
                             unit = u != null ? u.Name : "",
                              DropdownOptions = a.DropdownOptions.Where(o => o.IsActive).OrderBy(o => o.DisplayOrder).Select(o => new { o.DisplayText, o.Value, o.IsDefault })
                          };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                    _query = _query.Where(x => x.ID == exactId);
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.Name != null && x.Name.Contains(search));
                }
            }

            var skip = pageNo * pageSize;
            var data = await _query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.Name}",
                AdditionalValues = new Dictionary<string, object>
                {
                    { "UnitID", x.unitID! },
                    { "Unit", x.unit ?? "" },
                    { "ParameterType", x.ParameterType ?? "" },
                    { "Symbol", x.Symbol ?? "" },
                    { "InputType", x.InputType ?? "Decimal" },
                    { "DecimalPrecision", x.DecimalPrecision },
                    { "ElementType", x.ElementType ?? "" },
                    { "IsCalculated", x.IsCalculated },
                    { "Formula", x.Formula ?? "" },
                    { "FormulaDisplay", x.FormulaDisplay ?? "" },
                    { "DropdownOptions", x.DropdownOptions.ToList() }
                }
            }).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
            => await _context.ParameterMasters.AnyAsync(x => x.Name == name && x.IsActive);

        public async Task<bool> ExistsByNameAndNotId(string name, long id)
            => await _context.ParameterMasters.AnyAsync(x => x.Name == name && x.ID != id && x.IsActive);
    }
}