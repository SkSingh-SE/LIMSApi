using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;
        public EquipmentRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddEquipment(EquipmentMaster model)
        {
            await _context.EquipmentMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEquipment(long id)
        {
            var existingEquipment = await _context.EquipmentMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (existingEquipment != null)
            {
                existingEquipment.IsActive = false;
                existingEquipment.ModifiedOn = DateTime.UtcNow;
                _context.EquipmentMasters.Update(existingEquipment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<EquipmentMaster?> GetEquipmentById(long id)
        {
            return await _context.EquipmentMasters
                .Include(c => c.Calibrations)
                .Include(m => m.Maintenances)
                .Include(s => s.SOPs).FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task UpdateEquipment(EquipmentMaster model)
        {
            _context.EquipmentMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllEquipments(PageFilter filter)
        {
            var _query = from c in _context.EquipmentMasters
                         join d in _context.DepartmentMasters on c.DepartmentID equals d.ID into departmentGroup
                         from d in departmentGroup.DefaultIfEmpty()
                         join et in _context.EquipmentTypeMasters on c.EquipmentTypeID equals et.ID into typeGroup
                         from et in typeGroup.DefaultIfEmpty()

                         join o in _context.OEMMasters on c.OEMID equals o.ID into oemGroup
                         from o in oemGroup.DefaultIfEmpty()
                         where c.IsActive && c.CompanyCode == loggedInUser.CompanyCode
                         select new
                         {
                             c.ID,
                             c.Name,
                             c.EquipmentNo,
                             c.DepartmentID,
                             DepartmentName = d.Name,
                             c.EquipmentTypeID,
                             EquipmentType = et.Name,
                             c.OEMID,
                             OEMName = o.Name,
                             c.NextCalibrationDueDate,
                             c.NextMaintenanceDueDate,
                             c.ModifiedOn,
                             c.CreatedOn
                         };
            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

          

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.Name != null && x.Name.Contains(search))
                || x.EquipmentNo.Contains(search)
                || x.DepartmentName != null && x.DepartmentName.Contains(search)
                || x.EquipmentType != null && x.EquipmentType.Contains(search)
                || x.OEMName != null && x.OEMName.Contains(search)
                );
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetEquipmentDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.EquipmentMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => (x.Name != null && x.Name.Contains(search)));
                }
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name,
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.EquipmentMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.EquipmentMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }
    }
}
