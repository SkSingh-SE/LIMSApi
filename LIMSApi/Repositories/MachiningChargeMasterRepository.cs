using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class MachiningChargeMasterRepository : IMachiningChargeMasterRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public MachiningChargeMasterRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddMachiningChargeMaster(MachiningChargeMaster model)
        {
            await _context.MachiningChargeMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        // Persists changes already applied to a tracked MachiningChargeMaster graph
        // (scalar fields + Versions add/update/remove). No .Update() — the entity is tracked.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<long?> GetFinancialYearIdForDate(DateTime date)
        {
            return await _context.FinancialYears
                .Where(f => f.StartDate <= date && f.EndDate >= date)
                .Select(f => (long?)f.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<MachiningChargeMaster?> GetMachiningChargeMasterById(long id)
        {
            return await _context.MachiningChargeMasters
                .Include(x => x.Versions.Where(v => v.IsActive))
                    .ThenInclude(v => v.FinancialYear)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetAllMachiningChargeMasters(PageFilter filter)
        {
            var today = DateTime.UtcNow.Date;

            var _query = from m in _context.MachiningChargeMasters
                         join lt in _context.LaboratoryTests on m.LaboratoryTestID equals lt.ID into ltJoin
                         from lt in ltJoin.DefaultIfEmpty()
                         join sg in _context.LaboratoryTestSubGroups on m.LaboratoryTestID equals sg.ID into sgJoin
                         from sg in sgJoin.DefaultIfEmpty()
                         join at in _context.LaboratoryTestAnalysisTypes on m.LaboratoryTestID equals at.ID into atJoin
                         from at in atJoin.DefaultIfEmpty()
                         join tms in _context.TestMethodSpecifications on m.TestMethodStandardID equals tms.ID into tmsJoin
                         from tms in tmsJoin.DefaultIfEmpty()
                         join tmv in _context.TestMethodSpecificationVersions on m.TestMethodStandardID equals tmv.ID into tmvJoin
                         from tmv in tmvJoin.DefaultIfEmpty()
                         join tmvs in _context.TestMethodSpecifications on tmv.TestMethodSpecificationID equals tmvs.ID into tmvsJoin
                         from tmvs in tmvsJoin.DefaultIfEmpty()
                         where m.IsActive && m.CompanyCode == loggedInUser.CompanyCode
                         select new
                         {
                             m.ID,
                             m.LaboratoryTestID,
                             LaboratoryTestName = sg != null ? sg.Name : (at != null ? at.Name : (lt != null ? lt.Name : "")),
                             m.TestMethodStandardID,
                             TestMethodSpecificationName = tms != null ? (tms.TestMethodStandard + " - " + tms.Name) : (tmvs != null ? (tmvs.TestMethodStandard + " - " + tmvs.Name) : ""),
                             m.SpecimenRawMaterialSize,
                             m.SpecimenSize,
                             m.DrawingFilePath,
                             m.FileName,
                             m.UploadReferenceID,
                             CurrentPriceGeneralMetal = m.Versions
                                 .Where(v => v.IsActive && v.EffectiveFrom <= today)
                                 .OrderByDescending(v => v.EffectiveFrom)
                                 .Select(v => (decimal?)v.PriceGeneralMetal).FirstOrDefault(),
                             CurrentPriceHardMetal = m.Versions
                                 .Where(v => v.IsActive && v.EffectiveFrom <= today)
                                 .OrderByDescending(v => v.EffectiveFrom)
                                 .Select(v => (decimal?)v.PriceHardMetal).FirstOrDefault(),
                             VersionCount = m.Versions.Count(v => v.IsActive),
                             m.Remark,
                             m.ModifiedOn
                         };

            _query = _query.AsQueryable().ApplyFilters(filter.Filter);

            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x =>
                    (x.SpecimenSize != null && x.SpecimenSize.Contains(search))
                    || (x.SpecimenRawMaterialSize != null && x.SpecimenRawMaterialSize.Contains(search))
                    || (x.LaboratoryTestName != null && x.LaboratoryTestName.Contains(search))
                    || (x.TestMethodSpecificationName != null && x.TestMethodSpecificationName.Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetMachiningChargeMasterDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = _context.MachiningChargeMasters
                .Where(x => x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => x.SpecimenSize.Contains(search)
                        || (x.SpecimenRawMaterialSize != null && x.SpecimenRawMaterialSize.Contains(search)));
                }
            }

            var skip = pageNo * pageSize;

            return await _query.Skip(skip).Take(pageSize)
                .Select(x => new DropdwonSelector
                {
                    Id = x.ID,
                    Name = x.SpecimenSize + (x.SpecimenRawMaterialSize != null ? " (" + x.SpecimenRawMaterialSize + ")" : "")
                })
                .ToListAsync();
        }

        public async Task<bool> ExistsBySpecimenSizeAndTest(string specimenSize, long laboratoryTestID, long testMethodStandardID)
        {
            return await _context.MachiningChargeMasters.AnyAsync(x =>
                x.SpecimenSize == specimenSize
                && x.LaboratoryTestID == laboratoryTestID
                && x.TestMethodStandardID == testMethodStandardID
                && x.IsActive
                && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsBySpecimenSizeAndTestAndNotId(string specimenSize, long laboratoryTestID, long testMethodStandardID, long id)
        {
            return await _context.MachiningChargeMasters.AnyAsync(x =>
                x.SpecimenSize == specimenSize
                && x.LaboratoryTestID == laboratoryTestID
                && x.TestMethodStandardID == testMethodStandardID
                && x.ID != id
                && x.IsActive
                && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<MachiningChargeMaster>> GetByLabTestAndStandard(long labTestId, long standardId)
        {
            return await _context.MachiningChargeMasters
                .Include(x => x.Versions.Where(v => v.IsActive))
                    .ThenInclude(v => v.FinancialYear)
                .Where(x => (x.LaboratoryTestID == labTestId
                             || _context.LaboratoryTestSubGroups.Any(sg => sg.ID == labTestId && sg.LaboratoryTestID == x.LaboratoryTestID)
                             || _context.LaboratoryTestAnalysisTypes.Any(at => at.ID == labTestId && at.SubGroup != null && at.SubGroup.LaboratoryTestID == x.LaboratoryTestID))
                    && (x.TestMethodStandardID == standardId
                        || _context.TestMethodSpecificationVersions.Any(v => v.ID == standardId && v.TestMethodSpecificationID == x.TestMethodStandardID))
                    && x.IsActive
                    && x.CompanyCode == loggedInUser.CompanyCode)
                .OrderBy(x => x.SpecimenSize)
                .ToListAsync();
        }

    }
}
