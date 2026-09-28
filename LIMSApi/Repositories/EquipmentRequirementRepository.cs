using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class EquipmentRequirementRepository : IEquipmentRequirementRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public EquipmentRequirementRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddRequirement(EquipmentRequirementMaster model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.EquipmentRequirementMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRequirement(EquipmentRequirementMaster model)
        {
            _context.EquipmentRequirementMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<EquipmentRequirementMaster?> GetRequirementById(long id)
        {
            return await _context.EquipmentRequirementMasters
                .Include(x => x.LaboratoryTest)
                .Include(x => x.TestMethodSpecification)
                .Include(x => x.TestMethodSpecificationVersion)
                .Include(x => x.Parameter)
                .Include(x => x.EquipmentType)
                .Include(x => x.Equipment)
                .Include(x => x.RangeUnit)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<PagedResponse<object>> GetPagedRequirements(EquipmentRequirementListRequest request)
        {
            var status = (request.Status ?? "active").Trim().ToLowerInvariant();

            var query = from r in _context.EquipmentRequirementMasters
                        join t in _context.LaboratoryTests on r.LaboratoryTestID equals t.ID into testGroup
                        from t in testGroup.DefaultIfEmpty()
                        join m in _context.TestMethodSpecifications on r.TestMethodSpecificationID equals m.ID into methodGroup
                        from m in methodGroup.DefaultIfEmpty()
                        join p in _context.ParameterMasters on r.ParameterID equals p.ID into paramGroup
                        from p in paramGroup.DefaultIfEmpty()
                        join et in _context.EquipmentTypeMasters on r.EquipmentTypeID equals et.ID into typeGroup
                        from et in typeGroup.DefaultIfEmpty()
                        join e in _context.EquipmentMasters on r.EquipmentID equals e.ID into equipGroup
                        from e in equipGroup.DefaultIfEmpty()
                        join empMod in _context.EmployeeMasters on r.ModifiedBy equals empMod.ID into empModGroup
                        from empMod in empModGroup.DefaultIfEmpty()
                        join empCre in _context.EmployeeMasters on r.CreatedBy equals empCre.ID into empCreGroup
                        from empCre in empCreGroup.DefaultIfEmpty()
                        where r.CompanyCode == loggedInUser.CompanyCode
                        select new EquipmentRequirementListItemDto
                        {
                            ID = r.ID,
                            Code = r.Code,
                            Name = r.Name,
                            LaboratoryTestID = r.LaboratoryTestID,
                            LaboratoryTestName = t != null ? t.Name : "-",
                            TestMethodSpecificationID = r.TestMethodSpecificationID,
                            TestMethodName = m != null ? m.Name : null,
                            ParameterID = r.ParameterID,
                            ParameterName = p != null ? p.Name : null,
                            EquipmentTypeID = r.EquipmentTypeID,
                            EquipmentTypeName = et != null ? et.Name : "-",
                            EquipmentID = r.EquipmentID,
                            EquipmentName = e != null ? e.Name : null,
                            RequiredCapability = r.RequiredCapability,
                            IsMandatory = r.IsMandatory,
                            IsActive = r.IsActive,
                            CreatedByName = empCre != null ? empCre.Name : "-",
                            CreatedOn = r.CreatedOn,
                            ModifiedByName = empMod != null ? empMod.Name : (empCre != null ? empCre.Name : "-"),
                            ModifiedOn = r.ModifiedOn ?? r.CreatedOn
                        };

            if (status == "active")
                query = query.Where(x => x.IsActive);
            else if (status == "inactive")
                query = query.Where(x => !x.IsActive);

            if (request.LaboratoryTestID.HasValue && request.LaboratoryTestID.Value > 0)
                query = query.Where(x => x.LaboratoryTestID == request.LaboratoryTestID.Value);

            if (request.TestMethodSpecificationID.HasValue && request.TestMethodSpecificationID.Value > 0)
                query = query.Where(x => x.TestMethodSpecificationID == request.TestMethodSpecificationID.Value);

            if (request.EquipmentTypeID.HasValue && request.EquipmentTypeID.Value > 0)
                query = query.Where(x => x.EquipmentTypeID == request.EquipmentTypeID.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim();
                query = query.Where(x => (x.Code != null && x.Code.Contains(search))
                                      || (x.Name != null && x.Name.Contains(search))
                                      || (x.RequiredCapability != null && x.RequiredCapability.Contains(search))
                                      || (x.LaboratoryTestName != null && x.LaboratoryTestName.Contains(search))
                                      || (x.EquipmentTypeName != null && x.EquipmentTypeName.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.SortByColumn))
            {
                query = query.OrderBy($"{request.SortByColumn} {(request.SortOrder == "asc" ? "ascending" : "descending")}");
            }
            else
            {
                query = query.OrderBy(x => x.LaboratoryTestName).ThenBy(x => x.Code);
            }

            var filter = new PageFilter
            {
                PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber,
                PageSize = request.PageSize <= 0 ? 10 : request.PageSize
            };

            return await query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetRequirementDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            if (pageNo < 0) pageNo = 0;

            var query = from r in _context.EquipmentRequirementMasters
                        where r.IsActive && r.CompanyCode == loggedInUser.CompanyCode
                        select r;

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

            return await (query.OrderBy(x => x.Name).Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name
            })).ToListAsync();
        }

        public async Task<bool> ExistsByCode(string code)
        {
            return await _context.EquipmentRequirementMasters
                .AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long id)
        {
            return await _context.EquipmentRequirementMasters
                .AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.ID != id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsCombination(long laboratoryTestId, long? methodId, long? parameterId, long equipmentTypeId)
        {
            return await _context.EquipmentRequirementMasters
                .AnyAsync(x => x.LaboratoryTestID == laboratoryTestId
                    && x.TestMethodSpecificationID == methodId
                    && x.ParameterID == parameterId
                    && x.EquipmentTypeID == equipmentTypeId
                    && x.IsActive
                    && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<bool> ExistsCombinationAndNotId(long id, long laboratoryTestId, long? methodId, long? parameterId, long equipmentTypeId)
        {
            return await _context.EquipmentRequirementMasters
                .AnyAsync(x => x.ID != id
                    && x.LaboratoryTestID == laboratoryTestId
                    && x.TestMethodSpecificationID == methodId
                    && x.ParameterID == parameterId
                    && x.EquipmentTypeID == equipmentTypeId
                    && x.IsActive
                    && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task<List<EquipmentRequirementMaster>> GetActiveForTest(long laboratoryTestId, long? methodId)
        {
            var query = _context.EquipmentRequirementMasters
                .Include(x => x.EquipmentType)
                .Include(x => x.Equipment)
                .AsNoTracking()
                .Where(x => x.IsActive
                    && x.CompanyCode == loggedInUser.CompanyCode
                    && x.LaboratoryTestID == laboratoryTestId
                    && (x.TestMethodSpecificationID == null || x.TestMethodSpecificationID == methodId));

            return await query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Code).ToListAsync();
        }
    }
}
