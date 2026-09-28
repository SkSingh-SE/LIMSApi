using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ExecutionLayoutRepository : IExecutionLayoutRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public ExecutionLayoutRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddLayout(ExecutionLayoutMaster model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.ExecutionLayoutMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLayout(ExecutionLayoutMaster model)
        {
            _context.ExecutionLayoutMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<ExecutionLayoutMaster?> GetById(long id)
        {
            return await _context.ExecutionLayoutMasters.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<ExecutionLayoutMaster?> GetByIdWithStructure(long id)
        {
            return await _context.ExecutionLayoutMasters.AsNoTracking()
                .Include(x => x.Sections.Where(s => s.CompanyCode == loggedInUser.CompanyCode))
                    .ThenInclude(s => s.Items.Where(i => i.CompanyCode == loggedInUser.CompanyCode))
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetPagedLayouts(ExecutionLayoutListRequest request)
        {
            var status = (request.Status ?? "active").Trim().ToLowerInvariant();
            var query = from u in _context.ExecutionLayoutMasters
                        join empMod in _context.EmployeeMasters on u.ModifiedBy equals empMod.ID into empModGroup
                        from empMod in empModGroup.DefaultIfEmpty()
                        join empCre in _context.EmployeeMasters on u.CreatedBy equals empCre.ID into empCreGroup
                        from empCre in empCreGroup.DefaultIfEmpty()
                        where u.CompanyCode == loggedInUser.CompanyCode
                        orderby u.DisplayOrder, u.Code
                        select new ExecutionLayoutListItemDto
                        {
                            ID = u.ID, Code = u.Code, Name = u.Name, RendererType = u.RendererType,
                            SectionsCount = u.Sections.Count(s => s.IsActive),
                            IsActive = u.IsActive,
                            ModifiedByName = empMod != null ? empMod.Name : (empCre != null ? empCre.Name : "-"),
                            ModifiedOn = u.ModifiedOn ?? u.CreatedOn,
                            IsValid = true, ValidationErrors = new List<string>()
                        };
            if (status == "active") query = query.Where(x => x.IsActive);
            else if (status == "inactive") query = query.Where(x => !x.IsActive);
            if (!string.IsNullOrWhiteSpace(request.searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(request.searchTerm, out long exactId))
                    query = query.Where(x => x.ID == exactId);
                else { var s = request.searchTerm.Trim(); query = query.Where(x => x.Name.Contains(s) || x.Code.Contains(s)); }
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
            return new PagedResponse<object>(items.Cast<object>().ToList(), totalRecords, request.PageNumber, request.PageSize);
        }

        public async Task<List<DropdwonSelector>> GetDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            var query = _context.ExecutionLayoutMasters
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                    query = query.Where(x => x.ID == exactId);
                else { var s = searchTerm.Trim(); query = query.Where(x => x.Name.Contains(s) || x.Code.Contains(s)); }
            }
            var skip = pageNo * pageSize;
            return await query.OrderBy(x => x.Code).Skip(skip).Take(pageSize)
                .Select(x => new DropdwonSelector { Id = x.ID, Name = x.Name }).ToListAsync();
        }

        public async Task<bool> ExistsByCode(string code)
        {
            return await _context.ExecutionLayoutMasters
                .AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long id)
        {
            return await _context.ExecutionLayoutMasters
                .AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<ExecutionLayoutMaster>> GetAllActive(string companyCode)
        {
            return await _context.ExecutionLayoutMasters.AsNoTracking()
                .Include(x => x.Sections).ThenInclude(s => s.Items)
                .Where(x => x.IsActive && x.CompanyCode == companyCode)
                .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Code).ToListAsync();
        }
    }
}
