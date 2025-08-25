using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SampleInwardRepository : ISampleInwardRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public SampleInwardRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddSampleInward(SampleInward model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.SampleInwards.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSampleInward(long id)
        {
            var existingSampleInward = await _context.SampleInwards.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingSampleInward != null)
            {
                existingSampleInward.IsActive = false;
                existingSampleInward.ModifiedOn = DateTime.UtcNow;
                _context.SampleInwards.Update(existingSampleInward);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SampleInward?> GetSampleInwardById(long id)
        {
            return await _context.SampleInwards.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateSampleInward(SampleInward model)
        {
            _context.SampleInwards.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllSampleInwards(PageFilter filter)
        {
            var _query = from c in _context.SampleInwards where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode select c;

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.CaseNo != null && x.CaseNo.ToLower().Contains(search)));
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

        public async Task<List<DropdwonSelector>> GetSampleInwardDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.SampleInwards where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.CaseNo != null && x.CaseNo.ToLower().Contains(search)));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.CaseNo,
            })).ToListAsync();

            return data;
        }

        public async Task<object> GetCaseNoAndSampleNo()
        {
            var lastCase = await _context.SampleInwards
                .OrderByDescending(s => s.ID)
                .Select(s => s.CaseNo)
                .FirstOrDefaultAsync();

            var lastSampleNo = await _context.SampleDetails.OrderByDescending(s => s.ID)
                .Select(s => s.SampleNo)
                .FirstOrDefaultAsync();

            long lastNumber = 0;
            long lastSampleNumber = 0;

            if (!string.IsNullOrEmpty(lastCase))
            {
                if (long.TryParse(lastCase.Split('-')[1], out long parsed))
                {
                    lastNumber = parsed;
                }
            }
            if (!string.IsNullOrEmpty(lastSampleNo))
            {
                if (long.TryParse(lastSampleNo.Split('-')[1], out long parsed))
                {
                    lastSampleNumber = parsed;
                }
            }

            long nextNumber = lastNumber + 1;
            long nextSampleNumber = lastSampleNumber + 1;
            var year = DateTime.UtcNow.Year.ToString().Substring(2,2);
            var res = new 
            {
                caseNo = $"DMSPL-{nextNumber.ToString("D6")}",
                sampleNo = $"{year}-{nextSampleNumber.ToString("D6")}"
            };

            return res;
        }

    }
}
