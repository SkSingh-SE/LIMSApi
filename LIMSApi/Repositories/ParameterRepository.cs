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
                .FirstOrDefaultAsync(x => x.ID == id);
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
                          join empMod in _context.EmployeeMasters on c.ModifiedBy equals empMod.ID into empModGroup
                          from empMod in empModGroup.DefaultIfEmpty()
                          join empCre in _context.EmployeeMasters on c.CreatedBy equals empCre.ID into empCreGroup
                          from empCre in empCreGroup.DefaultIfEmpty()
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
                              c.CreatedBy,
                              CreatedByName = empCre != null ? empCre.Name : "-",
                              c.CreatedOn,
                              ModifiedByName = empMod != null ? empMod.Name : (empCre != null ? empCre.Name : "-"),
                              ModifiedOn = c.ModifiedOn ?? c.CreatedOn
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
                          join empMod in _context.EmployeeMasters on c.ModifiedBy equals empMod.ID into empModGroup
                          from empMod in empModGroup.DefaultIfEmpty()
                          join empCre in _context.EmployeeMasters on c.CreatedBy equals empCre.ID into empCreGroup
                          from empCre in empCreGroup.DefaultIfEmpty()
                          where c.IsActive && (c.ParameterType == null || c.ParameterType != "Chemical")
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
                              c.CreatedBy,
                              CreatedByName = empCre != null ? empCre.Name : "-",
                              c.CreatedOn,
                              ModifiedByName = empMod != null ? empMod.Name : (empCre != null ? empCre.Name : "-"),
                              ModifiedOn = c.ModifiedOn ?? c.CreatedOn
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
                             a.Code,
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
                    var tokens = search.Split(new[] { ' ', ',', '-', ':', '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length <= 1)
                    {
                        var token = tokens.Length == 1 ? tokens[0] : search;
                        _query = _query.Where(x => (x.Name != null && x.Name.Contains(token)) || (x.Code != null && x.Code.Contains(token)));
                    }
                    else
                    {
                        var pattern = "%" + string.Join("%", tokens) + "%";
                        _query = _query.Where(x => (x.Name != null && EF.Functions.Like(x.Name, pattern))
                                                || (x.Code != null && EF.Functions.Like(x.Code, pattern)));
                    }
                }
            }

            var skip = pageNo * pageSize;
            var data = await _query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.Name} - ({x.ParameterType})",
                AdditionalValues = new Dictionary<string, object>
                {
                    { "PureName", x.Name },
                    { "Code", x.Code ?? "" },
                    { "UnitID", x.unitID! },
                    { "Unit", x.unit ?? "" },
                    { "ParameterType", x.ParameterType ?? "" },
                    { "Symbol", x.Symbol ?? "" },
                    { "InputType", x.InputType ?? "" },
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
                             a.Code,
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
                    var tokens = search.Split(new[] { ' ', ',', '-', ':', '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length <= 1)
                    {
                        var token = tokens.Length == 1 ? tokens[0] : search;
                        _query = _query.Where(x => (x.Name != null && x.Name.Contains(token)) || (x.Code != null && x.Code.Contains(token)));
                    }
                    else
                    {
                        var pattern = "%" + string.Join("%", tokens) + "%";
                        _query = _query.Where(x => (x.Name != null && EF.Functions.Like(x.Name, pattern))
                                                || (x.Code != null && EF.Functions.Like(x.Code, pattern)));
                    }
                }
            }

            var skip = pageNo * pageSize;
            var data = await _query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.Name}",
                AdditionalValues = new Dictionary<string, object>
                {
                    { "PureName", x.Name },
                    { "Code", x.Code ?? "" },
                    { "UnitID", x.unitID! },
                    { "Unit", x.unit ?? "" },
                    { "ParameterType", x.ParameterType ?? "" },
                    { "Symbol", x.Symbol ?? "" },
                    { "InputType", x.InputType ?? "" },
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
                         where a.IsActive && (a.ParameterType == null || a.ParameterType != "Chemical")
                         select new
                         {
                             a.ID,
                             a.Code,
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
                    var tokens = search.Split(new[] { ' ', ',', '-', ':', '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length <= 1)
                    {
                        var token = tokens.Length == 1 ? tokens[0] : search;
                        _query = _query.Where(x => (x.Name != null && x.Name.Contains(token)) || (x.Code != null && x.Code.Contains(token)));
                    }
                    else
                    {
                        var pattern = "%" + string.Join("%", tokens) + "%";
                        _query = _query.Where(x => (x.Name != null && EF.Functions.Like(x.Name, pattern))
                                                || (x.Code != null && EF.Functions.Like(x.Code, pattern)));
                    }
                }
            }

            var skip = pageNo * pageSize;
            var data = await _query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.Name}",
                AdditionalValues = new Dictionary<string, object>
                {
                    { "PureName", x.Name },
                    { "Code", x.Code ?? "" },
                    { "UnitID", x.unitID! },
                    { "Unit", x.unit ?? "" },
                    { "ParameterType", x.ParameterType ?? "" },
                    { "Symbol", x.Symbol ?? "" },
                    { "InputType", x.InputType ?? "" },
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

        public async Task<bool> ExistsByCode(string code)
            => await _context.ParameterMasters.AnyAsync(x => x.Code == code);

        public async Task<bool> ExistsByCodeAndNotId(string code, long id)
            => await _context.ParameterMasters.AnyAsync(x => x.Code == code && x.ID != id);

        public async Task<PagedResponse<ParameterListItemDto>> GetAllParametersUnified(PageFilter filter)
        {
            var query = from p in _context.ParameterMasters
                        join u in _context.ParameterUnitMasters on p.ParameterUnitID equals u.ID into unitGroup
                        from u in unitGroup.DefaultIfEmpty()
                        join empMod in _context.EmployeeMasters on p.ModifiedBy equals empMod.ID into empModGroup
                        from empMod in empModGroup.DefaultIfEmpty()
                        join empCre in _context.EmployeeMasters on p.CreatedBy equals empCre.ID into empCreGroup
                        from empCre in empCreGroup.DefaultIfEmpty()
                        select new ParameterListItemDto
                        {
                            ID = p.ID,
                            Code = p.Code,
                            Name = p.Name,
                            Symbol = p.Symbol,
                            ParameterType = p.ParameterType,
                            InputType = p.InputType,
                            ParameterUnitID = p.ParameterUnitID,
                            UnitName = u != null ? u.Name : null,
                            UnitSymbol = u != null ? u.Symbol : null,
                            DecimalPrecision = p.DecimalPrecision,
                            IsCalculated = p.IsCalculated,
                            CalculationRole = p.CalculationRole,
                            Formula = p.Formula,
                            FormulaDisplay = p.FormulaDisplay,
                            Sequence = p.Sequence,
                            Description = p.Note,
                            IsActive = p.IsActive,
                            CreatedOn = p.CreatedOn,
                            ModifiedOn = p.ModifiedOn,
                            CreatedByName = empCre != null ? empCre.Name : "-",
                            ModifiedByName = empMod != null ? empMod.Name : (empCre != null ? empCre.Name : "-")
                        };

            // Custom column-level dictionary filters
            if (filter.Filter != null && filter.Filter.Any())
            {
                foreach (var f in filter.Filter)
                {
                    if (string.IsNullOrWhiteSpace(f.Value)) continue;
                    var col = f.Column?.ToLower();
                    var val = f.Value.Trim();

                    switch (col)
                    {
                        case "code":
                            query = query.Where(x => x.Code != null && x.Code.Contains(val));
                            break;
                        case "name":
                            query = query.Where(x => x.Name != null && x.Name.Contains(val));
                            break;
                        case "parametertype":
                            query = query.Where(x => x.ParameterType == val);
                            break;
                        case "inputtype":
                            query = query.Where(x => x.InputType == val);
                            break;
                        case "calculationrole":
                            query = query.Where(x => x.CalculationRole == val);
                            break;
                        case "unitid":
                        case "parameterunitid":
                            if (long.TryParse(val, out long uid))
                                query = query.Where(x => x.ParameterUnitID == uid);
                            break;
                        case "isactive":
                            if (bool.TryParse(val, out bool act))
                                query = query.Where(x => x.IsActive == act);
                            break;
                    }
                }
            }

            // Global search
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                query = query.Where(x =>
                    (x.Code != null && x.Code.Contains(search)) ||
                    (x.Name != null && x.Name.Contains(search)) ||
                    (x.Symbol != null && x.Symbol.Contains(search)) ||
                    (x.UnitName != null && x.UnitName.Contains(search)) ||
                    (x.UnitSymbol != null && x.UnitSymbol.Contains(search)) ||
                    (x.ParameterType != null && x.ParameterType.Contains(search)) ||
                    (x.InputType != null && x.InputType.Contains(search))
                );
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                var sortCol = filter.SortByColumn.Trim();
                var sortOrder = string.Equals(filter.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "descending" : "ascending";
                query = query.OrderBy($"{sortCol} {sortOrder}");
            }
            else
            {
                query = query.OrderBy("Name ascending");
            }

            return await query.ToPagedAsync(filter);
        }

        public async Task<ParameterDetailDto?> GetParameterDetailById(long id)
        {
            var entity = await _context.ParameterMasters
                .Include(p => p.ParameterUnit)
                .Include(p => p.DropdownOptions.Where(o => o.IsActive))
                .FirstOrDefaultAsync(x => x.ID == id);

            if (entity == null) return null;

            return new ParameterDetailDto
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.Name,
                Symbol = entity.Symbol,
                ParameterType = entity.ParameterType,
                InputType = entity.InputType,
                ParameterUnitID = entity.ParameterUnitID,
                UnitName = entity.ParameterUnit?.Name,
                UnitSymbol = entity.ParameterUnit?.Symbol,
                ParameterUnitEquivalentID = entity.ParameterUnitEquivalentID,
                UnitConversionFactor = entity.UnitConversionFactor,
                DecimalPrecision = entity.DecimalPrecision,
                IsCalculated = entity.IsCalculated,
                CalculationRole = entity.CalculationRole,
                Formula = entity.Formula,
                FormulaDisplay = entity.FormulaDisplay,
                Sequence = entity.Sequence,
                Description = entity.Note,
                ElementType = entity.ElementType,
                IsActive = entity.IsActive,
                CreatedOn = entity.CreatedOn,
                ModifiedOn = entity.ModifiedOn,
                DropdownOptions = entity.DropdownOptions
                    .Where(o => o.IsActive)
                    .OrderBy(o => o.DisplayOrder)
                    .Select(o => new ParameterDropdownOptionDto
                    {
                        ID = o.ID,
                        ParameterID = o.ParameterID,
                        DisplayText = o.DisplayText,
                        Value = o.Value,
                        DisplayOrder = o.DisplayOrder,
                        IsDefault = o.IsDefault,
                        IsActive = o.IsActive
                    }).ToList()
            };
        }
    }
}