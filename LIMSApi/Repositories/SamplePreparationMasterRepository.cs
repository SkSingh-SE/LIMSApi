using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class SamplePreparationMasterRepository : ISamplePreparationMasterRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public SamplePreparationMasterRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddSamplePreparationMaster(SamplePreparationMaster model)
        {
            await _context.SamplePreparationMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSamplePreparationMaster(SamplePreparationMaster model)
        {
            _context.SamplePreparationMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSamplePreparationMaster(SamplePreparationMaster model)
        {
            _context.SamplePreparationMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<SamplePreparationMaster?> GetSamplePreparationMasterById(long id)
        {
            return await _context.SamplePreparationMasters
                .Include(x => x.TestMethodStandard)
                .Include(x => x.LaboratoryTest)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetAllSamplePreparationMasters(PageFilter filter)
        {
            var _query = from sp in _context.SamplePreparationMasters
                         join tms in _context.TestMethodStandards on sp.TestMethodStandardID equals tms.ID into tmsJoin
                         from tms in tmsJoin.DefaultIfEmpty()
                         join lt in _context.LaboratoryTests on sp.LaboratoryTestID equals lt.ID into ltJoin
                         from lt in ltJoin.DefaultIfEmpty()
                         where sp.IsActive && sp.CompanyCode == loggedInUser.CompanyCode
                         select new
                         {
                             sp.ID,
                             sp.TestMethodStandardID,
                             TestMethodStandardName = tms != null ? tms.Name : "",
                             sp.LaboratoryTestID,
                             LaboratoryTestName = lt != null ? lt.Name : "",
                             sp.SpecimenType,
                             sp.Dimensions,
                             sp.MaterialType,
                             sp.Charges,
                             sp.Remark,
                             sp.ModifiedOn
                         };

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim().ToLower();
                _query = _query.Where(x =>
                    (x.SpecimenType != null && x.SpecimenType.ToLower().Contains(search))
                    || (x.Dimensions != null && x.Dimensions.ToLower().Contains(search))
                    || (x.MaterialType != null && x.MaterialType.ToLower().Contains(search))
                    || (x.TestMethodStandardName != null && x.TestMethodStandardName.ToLower().Contains(search))
                    || (x.LaboratoryTestName != null && x.LaboratoryTestName.ToLower().Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            int totalRecords = await _query.CountAsync();

            var items = await _query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<object>(items.Cast<object>().ToList(), totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<List<DropdwonSelector>> GetSamplePreparationMasterDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.SamplePreparationMasters
                         where a.IsActive && a.CompanyCode == loggedInUser.CompanyCode
                         select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                _query = _query.Where(x =>
                    (x.SpecimenType != null && x.SpecimenType.ToLower().Contains(search))
                    || (x.Dimensions != null && x.Dimensions.ToLower().Contains(search)));
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.SpecimenType + (x.Dimensions != null ? " " + x.Dimensions : ""),
            })).ToListAsync();

            return data;
        }

        public async Task<bool> ExistsBySpecimenAndDimensions(string specimenType, string? dimensions, long? testMethodStandardID, long? laboratoryTestID)
        {
            return await _context.SamplePreparationMasters.AnyAsync(x =>
                x.SpecimenType == specimenType
                && x.Dimensions == dimensions
                && x.TestMethodStandardID == testMethodStandardID
                && x.LaboratoryTestID == laboratoryTestID
                && x.IsActive
                && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsBySpecimenAndDimensionsAndNotId(string specimenType, string? dimensions, long? testMethodStandardID, long? laboratoryTestID, long id)
        {
            return await _context.SamplePreparationMasters.AnyAsync(x =>
                x.SpecimenType == specimenType
                && x.Dimensions == dimensions
                && x.TestMethodStandardID == testMethodStandardID
                && x.LaboratoryTestID == laboratoryTestID
                && x.ID != id
                && x.IsActive
                && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<SamplePreparationMaster>> GetByLaboratoryTestId(long laboratoryTestId)
        {
            return await _context.SamplePreparationMasters
                .Include(x => x.TestMethodStandard)
                .Include(x => x.LaboratoryTest)
                .Where(x => x.LaboratoryTestID == laboratoryTestId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .ToListAsync();
        }
    }
}
