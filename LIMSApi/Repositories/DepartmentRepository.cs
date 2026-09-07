using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    [Authorize]
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public DepartmentRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddDepartment(DepartmentMaster model)
        {
            await _context.DepartmentMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepartment(long id)
        {
            var existingDepartment = await _context.DepartmentMasters.FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);
            if (existingDepartment != null)
            {
                existingDepartment.IsActive = false;
                existingDepartment.ModifiedOn = DateTime.UtcNow;
                existingDepartment.ModifiedBy = loggedInUser?.EmployeeID ?? 0;
                _context.DepartmentMasters.Update(existingDepartment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DepartmentMaster?> GetDepartmentById(long id)
        {
            return await _context.DepartmentMasters
                .Include(d => d.Branch)
                .Include(d => d.Discipline)
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateDepartment(DepartmentMaster model)
        {
            _context.DepartmentMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllDepartments(PageFilter filter)
        {
            var query = from c in _context.DepartmentMasters
                        join b in _context.Branches on c.BranchID equals b.ID
                        join d in _context.DisciplineMasters on c.DisciplineID equals d.ID into disc
                        from dg in disc.DefaultIfEmpty()
                        join e in _context.EmployeeMasters on c.CreatedBy equals e.ID into emp
                        from eg in emp.DefaultIfEmpty()
                        where c.CompanyCode == loggedInUser.CompanyCode
                        select new DepartmentListItemDto
                        {
                            ID = c.ID,
                            BranchID = c.BranchID,
                            BranchName = b.Name,
                            BranchCode = b.Code,
                            DisciplineID = c.DisciplineID,
                            DisciplineName = dg != null ? dg.Name : "-",
                            DisciplineCode = dg != null ? dg.Code : "-",
                            Code = c.Code,
                            Name = c.Name,
                            Description = c.Description,
                            IsChemical = c.IsChemical,
                            IsActive = c.IsActive,
                            CreatedOn = c.CreatedOn,
                            ModifiedOn = c.ModifiedOn,
                            CreatedBy = eg != null ? eg.Name : null
                        };

            query = query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                query = query.Where(x =>
                    (x.Name != null && x.Name.ToLower().Contains(search))
                    || (x.Code != null && x.Code.ToLower().Contains(search))
                    || (x.BranchName != null && x.BranchName.ToLower().Contains(search))
                    || (x.DisciplineName != null && x.DisciplineName.ToLower().Contains(search))
                    || (x.Description != null && x.Description.ToLower().Contains(search))
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
            {
                query = query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            else
            {
                query = query.OrderBy(x => x.BranchName).ThenBy(x => x.Name);
            }

            return await query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetDepartmentDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var query = from a in _context.DepartmentMasters
                        where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                        select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    query = query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    query = query.Where(x => (x.Name != null && x.Name.Contains(search))
                                          || (x.Code != null && x.Code.Contains(search)));
                }
            }

            var skip = pageNo * pageSize;

            var data = await query.OrderBy(x => x.Name).Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name,
                AdditionalValues = new Dictionary<string, object>
                {
                    { "isChemical", x.IsChemical },
                    { "branchId", x.BranchID },
                    { "code", x.Code ?? "" },
                    { "disciplineId", x.DisciplineID ?? 0 }
                }
            }).ToListAsync();

            return data;
        }

        public async Task<bool> BranchExistsAndActive(long branchId)
        {
            return await _context.Branches.AnyAsync(b => b.ID == branchId && b.IsActive);
        }

        public async Task<bool> DisciplineExistsAndActive(long disciplineId)
        {
            return await _context.DisciplineMasters.AnyAsync(d => d.ID == disciplineId && d.IsActive);
        }

        public async Task<bool> BranchSupportsDiscipline(long branchId, long disciplineId)
        {
            return await _context.BranchDisciplines.AnyAsync(bd => bd.BranchID == branchId && bd.DisciplineID == disciplineId && bd.IsActive);
        }

        public async Task<bool> ExistsByCodeInBranch(string code, long branchId)
        {
            var trimmed = code.Trim().ToLower();
            return await _context.DepartmentMasters.AnyAsync(d => d.BranchID == branchId && d.Code != null && d.Code.ToLower() == trimmed && d.IsActive);
        }

        public async Task<bool> ExistsByCodeInBranchAndNotId(string code, long branchId, long id)
        {
            var trimmed = code.Trim().ToLower();
            return await _context.DepartmentMasters.AnyAsync(d => d.BranchID == branchId && d.ID != id && d.Code != null && d.Code.ToLower() == trimmed && d.IsActive);
        }

        public async Task<bool> ExistsByNameInBranch(string name, long branchId)
        {
            var trimmed = name.Trim().ToLower();
            return await _context.DepartmentMasters.AnyAsync(d => d.BranchID == branchId && d.Name.ToLower() == trimmed && d.IsActive);
        }

        public async Task<bool> ExistsByNameInBranchAndNotId(string name, long branchId, long id)
        {
            var trimmed = name.Trim().ToLower();
            return await _context.DepartmentMasters.AnyAsync(d => d.BranchID == branchId && d.ID != id && d.Name.ToLower() == trimmed && d.IsActive);
        }

        public async Task<List<object>> GetBranchDisciplines(long branchId)
        {
            var query = from bd in _context.BranchDisciplines
                        join d in _context.DisciplineMasters on bd.DisciplineID equals d.ID
                        where bd.BranchID == branchId && bd.IsActive && d.IsActive
                        orderby d.Name
                        select new
                        {
                            Id = d.ID,
                            Name = d.Name,
                            Code = d.Code,
                            IsAccredited = bd.IsAccredited
                        };

            var list = await query.ToListAsync();
            return list.Cast<object>().ToList();
        }

        public async Task<List<object>> GetAuthorizedBranches()
        {
            var user = LoggedInUserProvider.CurrentUser;
            var query = _context.Branches.Where(b => b.IsActive);

            if (user != null && !user.CanViewAllBranches && user.BranchID.HasValue)
            {
                query = query.Where(b => b.ID == user.BranchID.Value);
            }

            var branches = await query.OrderBy(b => b.Name)
                .Select(b => new
                {
                    Id = b.ID,
                    Name = b.Name,
                    Code = b.Code,
                    IsHeadOffice = b.IsHeadOffice
                })
                .ToListAsync();

            return branches.Cast<object>().ToList();
        }
    }
}
