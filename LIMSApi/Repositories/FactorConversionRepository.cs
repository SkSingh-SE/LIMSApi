using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class FactorConversionRepository : IFactorConversionRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public FactorConversionRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddFactor(FactorConversionMaster model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.FactorConversionMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFactor(FactorConversionMaster model)
        {
            _context.FactorConversionMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<FactorConversionMaster?> GetFactorById(long id)
        {
            return await _context.FactorConversionMasters
                .Include(x => x.InputParameter)
                .Include(x => x.OutputParameter)
                .Include(x => x.LaboratoryTest)
                .Include(x => x.TestMethodSpecification)
                .Include(x => x.TestMethodSpecificationVersion)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetPagedFactors(FactorConversionListRequest request)
        {
            var status = (request.Status ?? "active").Trim().ToLowerInvariant();

            var query = from f in _context.FactorConversionMasters
                        join ip in _context.ParameterMasters on f.InputParameterID equals ip.ID into ipGroup
                        from ip in ipGroup.DefaultIfEmpty()
                        join op in _context.ParameterMasters on f.OutputParameterID equals op.ID into opGroup
                        from op in opGroup.DefaultIfEmpty()
                        join empMod in _context.EmployeeMasters on f.ModifiedBy equals empMod.ID into empModGroup
                        from empMod in empModGroup.DefaultIfEmpty()
                        join empCre in _context.EmployeeMasters on f.CreatedBy equals empCre.ID into empCreGroup
                        from empCre in empCreGroup.DefaultIfEmpty()
                        where f.CompanyCode == loggedInUser.CompanyCode
                        select new FactorConversionListItemDto
                        {
                            ID = f.ID,
                            Code = f.Code,
                            Name = f.Name,
                            FactorType = f.FactorType,
                            FactorValue = f.FactorValue,
                            InputParameterID = f.InputParameterID,
                            InputParameterName = ip != null ? ip.Name : "-",
                            OutputParameterID = f.OutputParameterID,
                            OutputParameterName = op != null ? op.Name : null,
                            TransformationText = "",
                            IsMandatory = f.IsMandatory,
                            IsActive = f.IsActive,
                            CreatedByName = empCre != null ? empCre.Name : "-",
                            CreatedOn = f.CreatedOn,
                            ModifiedByName = empMod != null ? empMod.Name : (empCre != null ? empCre.Name : "-"),
                            ModifiedOn = f.ModifiedOn ?? f.CreatedOn
                        };

            if (status == "active")
                query = query.Where(x => x.IsActive);
            else if (status == "inactive")
                query = query.Where(x => !x.IsActive);

            if (!string.IsNullOrWhiteSpace(request.FactorType))
                query = query.Where(x => x.FactorType == request.FactorType!.Trim().ToUpperInvariant());

            if (request.InputParameterID.HasValue && request.InputParameterID.Value > 0)
                query = query.Where(x => x.InputParameterID == request.InputParameterID.Value);

            if (request.OutputParameterID.HasValue && request.OutputParameterID.Value > 0)
                query = query.Where(x => x.OutputParameterID == request.OutputParameterID.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim();
                query = query.Where(x => (x.Code != null && x.Code.Contains(search))
                                      || (x.Name != null && x.Name.Contains(search))
                                      || (x.InputParameterName != null && x.InputParameterName.Contains(search)));
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

            foreach (var item in paged.Items.OfType<FactorConversionListItemDto>())
                item.TransformationText = BuildTransformationText(item.FactorType, item.FactorValue, item.InputParameterName, item.OutputParameterName);

            return paged;
        }

        public static string BuildTransformationText(string factorType, decimal value, string input, string? output)
        {
            var target = output ?? input;
            return factorType switch
            {
                "DIVISION" => $"{input} ÷ {value} → {target}",
                "ADDITIVE_OFFSET" => $"{input} + ({value}) → {target}",
                "SUBTRACTIVE_OFFSET" => $"{input} - ({value}) → {target}",
                _ => $"{input} × {value} → {target}"
            };
        }

        public async Task<List<DropdwonSelector>> GetFactorDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            if (pageNo < 0) pageNo = 0;

            var query = from f in _context.FactorConversionMasters
                        where f.IsActive && f.CompanyCode == loggedInUser.CompanyCode
                        select f;

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
            return await _context.FactorConversionMasters
                .AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long id)
        {
            return await _context.FactorConversionMasters
                .AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        // Review rule: identical (parameter, type, test, method, version) scope may exist
        // only once among active records — silent conflicting duplicates are rejected.
        // Different scopes (global vs test vs method vs version) coexist; the resolver
        // applies most-specific-wins precedence per input parameter.
        public async Task<bool> ExistsCombination(long inputParameterId, string factorType, long? laboratoryTestId, long? methodId, long? versionId)
        {
            return await _context.FactorConversionMasters
                .AnyAsync(x => x.InputParameterID == inputParameterId
                    && x.FactorType == factorType
                    && x.LaboratoryTestID == laboratoryTestId
                    && x.TestMethodSpecificationID == methodId
                    && x.TestMethodSpecificationVersionID == versionId
                    && x.IsActive
                    && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsCombinationAndNotId(long id, long inputParameterId, string factorType, long? laboratoryTestId, long? methodId, long? versionId)
        {
            return await _context.FactorConversionMasters
                .AnyAsync(x => x.ID != id
                    && x.InputParameterID == inputParameterId
                    && x.FactorType == factorType
                    && x.LaboratoryTestID == laboratoryTestId
                    && x.TestMethodSpecificationID == methodId
                    && x.TestMethodSpecificationVersionID == versionId
                    && x.IsActive
                    && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<FactorConversionMaster>> GetActiveForTest(long laboratoryTestId, long? methodId, List<long> parameterIds)
        {
            var query = _context.FactorConversionMasters
                .AsNoTracking()
                .Where(x => x.IsActive
                    && x.CompanyCode == loggedInUser.CompanyCode
                    && parameterIds.Contains(x.InputParameterID)
                    && (x.LaboratoryTestID == null || x.LaboratoryTestID == laboratoryTestId)
                    && (x.TestMethodSpecificationID == null || x.TestMethodSpecificationID == methodId));

            return await query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Code).ToListAsync();
        }
    }
}
