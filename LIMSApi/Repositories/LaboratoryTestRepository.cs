using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class LaboratoryTestRepository : ILaboratoryTestRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public LaboratoryTestRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddTestMethod(LaboratoryTest model)
        {
            await _context.LaboratoryTests.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTestMethod(long id)
        {
            var existingTestMethod = await _context.LaboratoryTests.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (existingTestMethod != null)
            {
                existingTestMethod.IsActive = false;
                existingTestMethod.ModifiedOn = DateTime.UtcNow;
                _context.LaboratoryTests.Update(existingTestMethod);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<LaboratoryTest?> GetTestMethodById(long id)
        {
            //return await _context.LaboratoryTests.Include(t => t.SubGroups).FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            return await _context.LaboratoryTests
                .Include(y => y.InvoiceCases)
                .ThenInclude(z => z.InvoiceCaseConfiguration)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateTestMethod(LaboratoryTest model)
        {
            _context.LaboratoryTests.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllTestMethods(PageFilter filter)
        {

            var _query = from c in _context.LaboratoryTests
                         where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                         join d in _context.DepartmentMasters on c.LabDepartmentID equals d.ID into dsGroup
                         from ds in dsGroup.DefaultIfEmpty()
                         select new
                         {
                             c.ID,
                             c.Name,
                             c.LabDepartmentID,
                             DepartmentName = ds.Name,
                             c.SubGroup,
                             c.ModifiedOn,
                             c.CreatedOn
                         };


            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x =>
                                    (!string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(search)) ||
                                    (!string.IsNullOrEmpty(x.DepartmentName) && x.DepartmentName.ToLower().Contains(search)) ||
                                    (!string.IsNullOrEmpty(x.SubGroup) && x.SubGroup.ToLower().Contains(search))
                                    );
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

        public async Task<List<DropdwonSelector>> GetTestMethodDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.LaboratoryTests
                         join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                         where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                         select new
                         {
                             a.ID,
                             a.SubGroup,
                             Department = d.Name
                         };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x =>(x.SubGroup != null && x.SubGroup.ToLower().Contains(search)) || x.ID.ToString().Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.SubGroup} ({x.Department}) ",
            })).ToListAsync();

            return data;
        }
        public async Task<List<DropdwonSelector>> GetChemicalTestMethodDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.LaboratoryTests
                         join d in _context.DepartmentMasters on a.LabDepartmentID equals d.ID
                         where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                         select new
                         {
                             a.ID,
                             a.SubGroup,
                             Department = d.Name
                         };

            _query = _query.Where(x => x.Department.ToString().ToLower().Contains("chemical"));
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => (x.SubGroup != null && x.SubGroup.ToLower().Contains(search)) || x.ID.ToString().Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = $"{x.SubGroup} ({x.Department}) ",
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.LaboratoryTests.AnyAsync(x => x.Name == name && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.LaboratoryTests.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<string>> GetDistinctTestNames(string? searchTerm, int pageSize = 20)
        {
            var query = _context.LaboratoryTests
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .Select(x => x.Name)
                .Distinct();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(n => n.ToLower().Contains(search));
            }

            return await query.OrderBy(n => n).Take(pageSize).ToListAsync();
        }

        public async Task<List<object>> GetTestCases(long testMethodId)
        {
            var query = (
                from l in _context.LaboratoryTests
                join li in _context.LaboratoryTestInvoiceCase on l.ID equals li.LabTestID
                join c in _context.InvoiceCaseConfigurations on li.InvoiceCaseConfigID equals c.ID
                where l.ID == testMethodId && c.IsActive
                select new
                {
                    c.ID,
                    c.SelectionType,   // Element / Hours / Load / Day …
                    c.Name,            // "1 Element", "Up to 24 hrs"
                    c.Value,           // 1, 24, 5
                    c.Unit             // Element, hr, ton, day
                }
            ).Distinct();

            var data = await query.ToListAsync();

            var result = data.Cast<object>().ToList();
            return result;
        }
    }
}
