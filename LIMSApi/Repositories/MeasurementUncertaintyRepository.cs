using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class MeasurementUncertaintyRepository : IMeasurementUncertaintyRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public MeasurementUncertaintyRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddUncertainty(MeasurementUncertaintyMaster model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.MeasurementUncertaintyMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUncertainty(MeasurementUncertaintyMaster model)
        {
            _context.MeasurementUncertaintyMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<MeasurementUncertaintyMaster?> GetUncertaintyById(long id)
        {
            return await _context.MeasurementUncertaintyMasters
                .Include(x => x.LaboratoryTest)
                .Include(x => x.Parameter)
                .Include(x => x.TestMethodSpecification)
                .Include(x => x.TestMethodSpecificationVersion)
                .Include(x => x.ParameterUnit)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetPagedUncertainties(MeasurementUncertaintyListRequest request)
        {
            var status = (request.Status ?? "active").Trim().ToLowerInvariant();

            var query = from u in _context.MeasurementUncertaintyMasters
                        join t in _context.LaboratoryTests on u.LaboratoryTestID equals t.ID into tGroup
                        from t in tGroup.DefaultIfEmpty()
                        join p in _context.ParameterMasters on u.ParameterID equals p.ID into pGroup
                        from p in pGroup.DefaultIfEmpty()
                        join m in _context.TestMethodSpecifications on u.TestMethodSpecificationID equals m.ID into mGroup
                        from m in mGroup.DefaultIfEmpty()
                        join un in _context.ParameterUnitMasters on u.ParameterUnitID equals un.ID into unGroup
                        from un in unGroup.DefaultIfEmpty()
                        join empMod in _context.EmployeeMasters on u.ModifiedBy equals empMod.ID into empModGroup
                        from empMod in empModGroup.DefaultIfEmpty()
                        join empCre in _context.EmployeeMasters on u.CreatedBy equals empCre.ID into empCreGroup
                        from empCre in empCreGroup.DefaultIfEmpty()
                        where u.CompanyCode == loggedInUser.CompanyCode
                        select new MeasurementUncertaintyListItemDto
                        {
                            ID = u.ID,
                            Code = u.Code,
                            Name = u.Name,
                            LaboratoryTestID = u.LaboratoryTestID,
                            LaboratoryTestName = t != null ? t.Name : null,
                            ParameterID = u.ParameterID,
                            ParameterName = p != null ? p.Name : null,
                            TestMethodSpecificationID = u.TestMethodSpecificationID,
                            TestMethodName = m != null ? m.Name : null,
                            UncertaintyType = u.UncertaintyType,
                            Basis = u.Basis,
                            CombinedUncertainty = u.CombinedUncertainty,
                            ExpandedUncertainty = u.ExpandedUncertainty,
                            CoverageFactor = u.CoverageFactor,
                            ConfidenceLevel = u.ConfidenceLevel,
                            UnitSymbol = un != null ? un.Symbol : null,
                            SummaryText = "",
                            IsActive = u.IsActive,
                            CreatedByName = empCre != null ? empCre.Name : "-",
                            CreatedOn = u.CreatedOn,
                            ModifiedByName = empMod != null ? empMod.Name : (empCre != null ? empCre.Name : "-"),
                            ModifiedOn = u.ModifiedOn ?? u.CreatedOn
                        };

            if (status == "active")
                query = query.Where(x => x.IsActive);
            else if (status == "inactive")
                query = query.Where(x => !x.IsActive);

            if (request.LaboratoryTestID.HasValue && request.LaboratoryTestID.Value > 0)
                query = query.Where(x => x.LaboratoryTestID == request.LaboratoryTestID.Value);

            if (request.ParameterID.HasValue && request.ParameterID.Value > 0)
                query = query.Where(x => x.ParameterID == request.ParameterID.Value);

            if (request.TestMethodSpecificationID.HasValue && request.TestMethodSpecificationID.Value > 0)
                query = query.Where(x => x.TestMethodSpecificationID == request.TestMethodSpecificationID.Value);

            if (!string.IsNullOrWhiteSpace(request.UncertaintyType))
                query = query.Where(x => x.UncertaintyType == request.UncertaintyType!.Trim().ToUpperInvariant());

            if (!string.IsNullOrWhiteSpace(request.Basis))
                query = query.Where(x => x.Basis == request.Basis!.Trim());

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim();
                query = query.Where(x => (x.Code != null && x.Code.Contains(search))
                                      || (x.Name != null && x.Name.Contains(search))
                                      || (x.ParameterName != null && x.ParameterName.Contains(search))
                                      || (x.LaboratoryTestName != null && x.LaboratoryTestName.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.SortByColumn))
            {
                query = query.OrderBy($"{request.SortByColumn} {(request.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            else
            {
                query = query.OrderBy(x => x.Code);
            }

            var filter = new PageFilter
            {
                PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber,
                PageSize = request.PageSize <= 0 ? 10 : request.PageSize
            };

            var paged = await query.Cast<object>().ToPagedAsync(filter);

            foreach (var item in paged.Items.OfType<MeasurementUncertaintyListItemDto>())
                item.SummaryText = BuildSummaryText(item.UncertaintyType, item.ExpandedUncertainty, item.CombinedUncertainty, item.CoverageFactor, item.UnitSymbol);

            return paged;
        }

        public static string BuildSummaryText(string uncertaintyType, decimal? expanded, decimal? combined, decimal coverageFactor, string? unit)
        {
            var u = unit ?? "";
            if (expanded.HasValue)
                return $"± {expanded.Value} {u} (k={coverageFactor})".Trim();
            if (combined.HasValue)
                return $"uc {combined.Value} {u} (k={coverageFactor})".Trim();
            return $"{uncertaintyType} (k={coverageFactor})";
        }

        public async Task<List<DropdwonSelector>> GetUncertaintyDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            if (pageNo < 0) pageNo = 0;

            var query = from u in _context.MeasurementUncertaintyMasters
                        where u.IsActive && u.CompanyCode == loggedInUser.CompanyCode
                        select u;

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

            return await (query.OrderBy(x => x.Code).Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name
            })).ToListAsync();
        }

        public async Task<bool> ExistsByCode(string code)
        {
            return await _context.MeasurementUncertaintyMasters
                .AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long id)
        {
            return await _context.MeasurementUncertaintyMasters
                .AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsCombination(long? laboratoryTestId, long? parameterId, long? methodId, long? versionId)
        {
            return await _context.MeasurementUncertaintyMasters
                .AnyAsync(x => x.LaboratoryTestID == laboratoryTestId
                    && x.ParameterID == parameterId
                    && x.TestMethodSpecificationID == methodId
                    && x.TestMethodSpecificationVersionID == versionId
                    && x.IsActive
                    && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsCombinationAndNotId(long id, long? laboratoryTestId, long? parameterId, long? methodId, long? versionId)
        {
            return await _context.MeasurementUncertaintyMasters
                .AnyAsync(x => x.ID != id
                    && x.LaboratoryTestID == laboratoryTestId
                    && x.ParameterID == parameterId
                    && x.TestMethodSpecificationID == methodId
                    && x.TestMethodSpecificationVersionID == versionId
                    && x.IsActive
                    && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<MeasurementUncertaintyMaster>> GetActiveForScope(string companyCode, long? laboratoryTestId, long? methodId, long? versionId)
        {
            return await _context.MeasurementUncertaintyMasters
                .AsNoTracking()
                .Where(x => x.IsActive
                    && x.CompanyCode == companyCode
                    && (x.LaboratoryTestID == null || x.LaboratoryTestID == laboratoryTestId)
                    && (x.TestMethodSpecificationID == null || x.TestMethodSpecificationID == methodId)
                    && (x.TestMethodSpecificationVersionID == null || x.TestMethodSpecificationVersionID == versionId))
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Code)
                .ToListAsync();
        }
    }
}
