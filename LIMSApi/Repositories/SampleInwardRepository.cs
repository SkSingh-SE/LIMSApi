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
            var sampleInward = await _context.SampleInwards
                                .Include(x => x.DispatchModes)
                                .Include(x => x.Contacts)
                                .Include(x => x.Addresses)
                                .Include(x => x.SampleDetails)
                                    .ThenInclude(sd => sd.AdditionalDetails)
                                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            return sampleInward;
        }

        public async Task<SampleInward?> GetSampleInwardWithPlans(long id)
        {
            var sampleInward = await _context.SampleInwards
                .Include(x => x.DispatchModes)
                .Include(x => x.Contacts)
                .Include(x => x.Addresses)
                .Include(x => x.SampleDetails)
                    .ThenInclude(sd => sd.AdditionalDetails)
                .Include(x => x.SampleDetails)
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.GeneralTests)
                            .ThenInclude(gt => gt.Methods)
                .Include(x => x.SampleDetails)
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.ChemicalTests)
                            .ThenInclude(ct => ct.Elements)
                .Include(x => x.SampleDetails)
                    .ThenInclude(sd => sd.TestPlans)
                        .ThenInclude(tp => tp.ChemicalTests)
                            .ThenInclude(ct => ct.TestTypes)
                .FirstOrDefaultAsync(x =>
                    x.ID == id &&
                    x.IsActive &&
                    x.CompanyCode == loggedInUser.CompanyCode);

            return sampleInward;
        }



        public async Task UpdateSampleInward(SampleInward model)
        {
            _context.SampleInwards.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllSampleInwards(PageFilter filter)
        {
            var _query = _context.SampleInwards
                         .Where(c => c.IsActive && c.CompanyCode == loggedInUser.CompanyCode)
                         .Select(c => new
                         {
                             c.ID,
                             c.CaseNo,
                             c.CustomerID,
                             CustomerName = c.Customer != null ? c.Customer.Name : string.Empty,
                             ContactPersonName = c.Contacts.OrderBy(x => x.ID).Select(x => x.Name).FirstOrDefault(),
                             ContactEmail = c.Contacts.OrderBy(x => x.ID).Select(x => x.EmailId).FirstOrDefault(),
                             ContactPhone = c.Contacts.OrderBy(x => x.ID).Select(x => x.MobileNo).FirstOrDefault(),
                             c.CollectionTime,
                             c.ModifiedOn,
                             ModifiedBy = _context.EmployeeMasters
                                          .Where(e => e.ID == c.ModifiedBy)
                                          .Select(e => e.Name)
                                          .FirstOrDefault()
                         });



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

            var lastSampleNo = await _context.SampleDetails
                .OrderByDescending(s => s.ID)
                .Select(s => s.SampleNo)
                .FirstOrDefaultAsync();

            long lastCaseNumber = 0;
            long lastSampleNumber = 0;

            if (!string.IsNullOrEmpty(lastCase))
            {
                if (long.TryParse(lastCase.Split('-')[1], out long parsed))
                {
                    lastCaseNumber = parsed;
                }
            }

            if (!string.IsNullOrEmpty(lastSampleNo))
            {
                if (long.TryParse(lastSampleNo.Split('-')[1], out long parsed))
                {
                    lastSampleNumber = parsed;
                }
            }

            long nextCaseNumber = lastCaseNumber + 1;
            long nextSampleNumber = lastSampleNumber + 1;
            var year = DateTime.UtcNow.Year.ToString().Substring(2, 2);

            var res = new
            {
                caseNo = $"DMSPL-{nextCaseNumber:D6}",  
                sampleNo = $"{year}-{nextSampleNumber:D6}", 
                nextSampleCounter = nextSampleNumber     
            };

            return res;
        }


    }
}
