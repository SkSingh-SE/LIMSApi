using System.Linq.Dynamic.Core;
using System.Text.Json;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ConditionMasterRepository : IConditionMasterRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public ConditionMasterRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddConditionMaster(ConditionMaster model)
        {
            await _context.ConditionMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateConditionMaster(ConditionMaster model)
        {
            _context.ConditionMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteConditionMaster(ConditionMaster model)
        {
            _context.ConditionMasters.Remove(model);
            await _context.SaveChangesAsync();
        }

        public async Task<ConditionMaster?> GetConditionMasterById(long id)
        {
            return await _context.ConditionMasters
                .Include(x => x.ParameterUnit)
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<ConditionMaster?> GetConditionMasterByCode(string code)
        {
            return await _context.ConditionMasters
                .Include(x => x.ParameterUnit)
                .FirstOrDefaultAsync(x => x.Code == code && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetAllConditionMasters(PageFilter filter)
        {
            var companyCode = loggedInUser?.CompanyCode ?? "LIMS";

            var baseQuery = _context.ConditionMasters
                .Include(x => x.ParameterUnit)
                .Where(c => c.CompanyCode == companyCode)
                .AsQueryable();

            if (filter.Filter != null && filter.Filter.Count > 0)
            {
                baseQuery = baseQuery.ApplyFilters(filter.Filter);
            }

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                baseQuery = baseQuery.Where(x =>
                    (x.Name != null && x.Name.ToLower().Contains(search))
                    || (x.Code != null && x.Code.ToLower().Contains(search))
                    || (x.Category != null && x.Category.ToLower().Contains(search))
                    || (x.ValueType != null && x.ValueType.ToLower().Contains(search))
                    || (x.Description != null && x.Description.ToLower().Contains(search))
                );
            }

            var totalRecords = await baseQuery.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                baseQuery = baseQuery.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            else
            {
                baseQuery = baseQuery.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name);
            }

            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var pagedEntities = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var conditionIds = pagedEntities.Select(x => x.ID).ToList();

            // Count references in SpecificationLineConditions
            var refCounts = await _context.SpecificationLineConditions
                .Where(c => conditionIds.Contains(c.ConditionMasterID))
                .GroupBy(c => c.ConditionMasterID)
                .Select(g => new { ID = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.ID, g => g.Count);

            var items = pagedEntities.Select(x =>
            {
                List<string> ops = new();
                if (!string.IsNullOrWhiteSpace(x.AllowedOperators))
                {
                    try
                    {
                        ops = JsonSerializer.Deserialize<List<string>>(x.AllowedOperators) ?? new();
                    }
                    catch
                    {
                        ops = x.AllowedOperators.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
                    }
                }

                List<string>? vals = null;
                if (!string.IsNullOrWhiteSpace(x.AllowedValuesJson))
                {
                    try
                    {
                        vals = JsonSerializer.Deserialize<List<string>>(x.AllowedValuesJson);
                    }
                    catch { }
                }

                return (object)new ConditionMasterDto
                {
                    ID = x.ID,
                    Code = x.Code,
                    Name = x.Name,
                    Category = x.Category,
                    ValueType = x.ValueType,
                    ParameterUnitID = x.ParameterUnitID,
                    ParameterUnitName = x.ParameterUnit?.Name,
                    ParameterUnitSymbol = x.ParameterUnit?.Symbol,
                    AllowedOperators = ops,
                    AllowedValues = vals,
                    AllowedValuesJson = x.AllowedValuesJson,
                    DefaultValue = x.DefaultValue,
                    Description = x.Description,
                    DisplayOrder = x.DisplayOrder,
                    IsActive = x.IsActive,
                    CreatedBy = x.CreatedBy,
                    CreatedOn = x.CreatedOn,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedOn = x.ModifiedOn,
                    CompanyCode = x.CompanyCode,
                    ReferenceCount = refCounts.ContainsKey(x.ID) ? refCounts[x.ID] : 0
                };
            }).ToList();

            return new PagedResponse<object>(items, totalRecords, pageNumber, pageSize);
        }

        public async Task<List<ConditionMasterDropdownDto>> GetConditionMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var companyCode = loggedInUser?.CompanyCode ?? "LIMS";

            var query = _context.ConditionMasters
                .Include(x => x.ParameterUnit)
                .Where(x => x.CompanyCode == companyCode && x.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(term) ||
                    x.Code.ToLower().Contains(term) ||
                    x.Category.ToLower().Contains(term));
            }

            var results = await query
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return results.Select(x =>
            {
                List<string> ops = new();
                if (!string.IsNullOrWhiteSpace(x.AllowedOperators))
                {
                    try { ops = JsonSerializer.Deserialize<List<string>>(x.AllowedOperators) ?? new(); }
                    catch { ops = x.AllowedOperators.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(); }
                }

                List<string>? vals = null;
                if (!string.IsNullOrWhiteSpace(x.AllowedValuesJson))
                {
                    try { vals = JsonSerializer.Deserialize<List<string>>(x.AllowedValuesJson); }
                    catch { }
                }

                return new ConditionMasterDropdownDto
                {
                    ID = x.ID,
                    Code = x.Code,
                    Name = x.Name,
                    Category = x.Category,
                    ValueType = x.ValueType,
                    ParameterUnitID = x.ParameterUnitID,
                    UnitSymbol = x.ParameterUnit?.Symbol,
                    AllowedOperators = ops,
                    AllowedValues = vals,
                    DefaultValue = x.DefaultValue,
                    IsActive = x.IsActive
                };
            }).ToList();
        }

        public async Task<bool> ExistsByCode(string code)
        {
            var companyCode = loggedInUser?.CompanyCode ?? "LIMS";
            return await _context.ConditionMasters
                .AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.CompanyCode == companyCode);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long id)
        {
            var companyCode = loggedInUser?.CompanyCode ?? "LIMS";
            return await _context.ConditionMasters
                .AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.ID != id && x.CompanyCode == companyCode);
        }

        public async Task<int> GetReferenceCount(long conditionMasterId)
        {
            return await _context.SpecificationLineConditions
                .CountAsync(c => c.ConditionMasterID == conditionMasterId);
        }
    }
}
