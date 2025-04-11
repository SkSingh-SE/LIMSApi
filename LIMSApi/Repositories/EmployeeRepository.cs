using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public EmployeeRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<EmployeeMaster> AddEmployee(EmployeeMaster model)
        {
            await _context.EmployeeMasters.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task DeleteEmployee(long id)
        {
            var existingEmployee = await _context.EmployeeMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (existingEmployee != null)
            {
                existingEmployee.IsActive = false;
                existingEmployee.ModifiedOn = DateTime.UtcNow;
                _context.EmployeeMasters.Update(existingEmployee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<EmployeeMaster?> GetEmployeeById(long id)
        {
            return await _context.EmployeeMasters
           .Include(e => e.Department)
           .Include(e => e.Designation)
           .Include(e => e.ReportingManager)
           .Include(x => x.Qualifications)
           .Include(x => x.Documents)
           .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateEmployee(EmployeeMaster model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            model.ModifiedBy = loggedInUser.EmployeeID;
            model.ModifiedOn = DateTime.UtcNow;
            _context.EmployeeMasters.Update(model);
            await _context.SaveChangesAsync();
            
        }

        public async Task<PagedResponse<object>> GetAllEmployees(PageFilter filter)
        {
            var _query = (from e in _context.EmployeeMasters
                         where e.IsActive && e.CompanyCode == loggedInUser.CompanyCode
                         join d in _context.DepartmentMasters on e.DepartmentID equals d.ID into dpGroup
                         from dp in dpGroup.DefaultIfEmpty()

                         join ds in _context.DesignationMasters on e.DesignationID equals ds.ID into dsGroup
                         from ds in dsGroup.DefaultIfEmpty()
                         select new
                         {
                             e.ID,
                             e.Name,
                             e.EmailId,
                             e.DateOfJoin,
                             e.DateOfBirth,
                             e.Gender,
                             e.DepartmentID,
                             DepartmentName = dp.Name,
                             e.DesignationID,
                             DesignationName = ds.Name
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

        public async Task<List<DropdwonSelector>> GetEmployeeDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.EmployeeMasters where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x => x.ID.ToString().Contains(search) 
                ||x.Name != null && x.Name.ToLower().Contains(search));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByEmail(string email)
        {
            return await _context.EmployeeMasters.AnyAsync(x => x.EmailId == email && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByEmailAndNotId(string email, long Id)
        {
            return await _context.EmployeeMasters.AnyAsync(x => x.EmailId == email && x.ID != Id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        // Employee Qualification Management
        public async Task AddEmployeeQualification(EmployeeQualification qualification)
        {
            qualification.CompanyCode = loggedInUser.CompanyCode;
            qualification.CreatedBy = loggedInUser.EmployeeID;
            qualification.CreatedOn = DateTime.UtcNow;
            await _context.EmployeeQualifications.AddAsync(qualification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeQualification(EmployeeQualification qualification)
        {
            qualification.CompanyCode = loggedInUser.CompanyCode;
            qualification.ModifiedBy = loggedInUser.EmployeeID;
            qualification.ModifiedOn = DateTime.UtcNow;
            _context.EmployeeQualifications.Update(qualification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeQualification(long id)
        {
            var qualification = await _context.EmployeeQualifications.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (qualification != null)
            {
                _context.EmployeeQualifications.Remove(qualification);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<EmployeeQualification?> GetEmployeeQualificationById(long id)
        {
            return await _context.EmployeeQualifications.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<EmployeeQualification>> GetEmployeeQualifications(long employeeId)
        {
            return await _context.EmployeeQualifications
                .Where(q => q.EmployeeID == employeeId)
                .ToListAsync();
        }

        // Employee Document Management
        public async Task AddEmployeeDocument(EmployeeDocument document)
        {
            document.CompanyCode = loggedInUser.CompanyCode;
            await _context.EmployeeDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateEmployeeDocument(EmployeeDocument document)
        {
            document.CompanyCode = loggedInUser.CompanyCode;
            document.ModifiedBy = loggedInUser.EmployeeID;
            document.ModifiedOn = DateTime.UtcNow;
            _context.EmployeeDocuments.Update(document);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteEmployeeDocument(long id)
        {
            var document = await _context.EmployeeDocuments.FindAsync(id);
            if (document != null)
            {
                _context.EmployeeDocuments.Remove(document);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<EmployeeDocument?> GetEmployeeDocumentById(long id)
        {
            return await _context.EmployeeDocuments.Include(x => x.UploadFile).FirstOrDefaultAsync(d => d.ID == id);
        }

        public async Task<List<EmployeeDocument>> GetEmployeeDocuments(long employeeId)
        {
            return await _context.EmployeeDocuments
                .Where(d => d.EmployeeID == employeeId)
                .ToListAsync();
        }
    }
}
