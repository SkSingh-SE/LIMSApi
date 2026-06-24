using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class CuttingPriceMasterService : ICuttingPriceMasterService
    {
        private readonly ICuttingPriceMasterRepository _itemRepository;
        private readonly ILogger<CuttingPriceMasterService> _logger;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public CuttingPriceMasterService(ICuttingPriceMasterRepository itemMasterRepository, ILogger<CuttingPriceMasterService> logger, LIMSContext context)
        {
            _itemRepository = itemMasterRepository;
            _logger = logger;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        // Each version's Effective From must fall within its selected Financial Year (StartDate..EndDate).
        private async Task ValidateEffectiveWithinFinancialYearAsync(IEnumerable<CuttingPriceVersion> versions)
        {
            var fyIds = versions.Where(v => v.FinancialYearId.HasValue).Select(v => v.FinancialYearId!.Value).Distinct().ToList();
            if (fyIds.Count == 0) return;
            var fys = await _context.FinancialYears.Where(f => fyIds.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => new { f.StartDate, f.EndDate });
            foreach (var v in versions.Where(v => v.FinancialYearId.HasValue))
            {
                if (fys.TryGetValue(v.FinancialYearId!.Value, out var fy)
                    && (v.EffectiveFrom.Date < fy.StartDate.Date || v.EffectiveFrom.Date > fy.EndDate.Date))
                {
                    throw new ArgumentException("Effective From must fall within the selected financial year.");
                }
            }
        }

        public async Task CreateCuttingPrice(CuttingPriceMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.CuttingType))
                throw new ArgumentException("CuttingPriceMaster name should not be empty!");

            ValidateVersions(model.Versions);
            await ValidateEffectiveWithinFinancialYearAsync(model.Versions);

            bool exists = await _itemRepository.ExistsByName(model.CuttingType);
            if (exists)
                throw new InvalidOperationException("CuttingPriceMaster already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            foreach (var version in model.Versions)
            {
                version.ID = 0;
                version.CreatedOn = DateTime.UtcNow;
                version.CreatedBy = loggedInUser.EmployeeID;
                version.ModifiedOn = null;
                version.ModifiedBy = null;
                version.CompanyCode = loggedInUser.CompanyCode;
                version.IsActive = true;
            }

            await _itemRepository.AddCuttingPrice(model);
            _logger.LogInformation("CuttingPriceMaster '{CuttingPriceMasterName}' created successfully.", model.CuttingType);
        }

        public async Task ModifyCuttingPrice(CuttingPriceMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("CuttingPriceMaster ID should not be empty!");

            ValidateVersions(model.Versions);
            await ValidateEffectiveWithinFinancialYearAsync(model.Versions);

            bool exists = await _itemRepository.ExistsByNameAndNotId(model.CuttingType, model.ID);
            if (exists)
                throw new InvalidOperationException("Same CuttingPriceMaster already exists!");

            var existing = await _itemRepository.GetCuttingPriceById(model.ID);
            if (existing == null)
                throw new InvalidOperationException("CuttingPriceMaster not found!");

            existing.CuttingType = model.CuttingType;
            existing.UnitType = model.UnitType;
            existing.SizeRangeMin = model.SizeRangeMin;
            existing.SizeRangeMax = model.SizeRangeMax;
            existing.Remark = model.Remark;
            existing.SpecimenTypeId = model.SpecimenTypeId;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            SyncVersions(existing, model.Versions);

            await _itemRepository.SaveChangesAsync();
            _logger.LogInformation("CuttingPriceMaster '{CuttingPriceMasterName}' updated successfully.", model.CuttingType);
        }

        public async Task RemoveCuttingPrice(long id)
        {
            var existing = await _itemRepository.GetCuttingPriceById(id);
            if (existing == null)
                throw new InvalidOperationException("CuttingPriceMaster not found!");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _itemRepository.SaveChangesAsync();
            _logger.LogInformation("CuttingPriceMaster with ID '{CuttingPriceMasterId}' deleted successfully.", id);
        }

        public async Task<CuttingPriceMaster> GetCuttingPriceDetails(long id)
        {
            var classification = await _itemRepository.GetCuttingPriceById(id);
            if (classification == null)
                throw new InvalidOperationException("CuttingPriceMaster not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchCuttingPriceList(PageFilter filter)
        {
            return await _itemRepository.GetAllCuttingPrices(filter);
        }

        public async Task<List<DropdwonSelector>> GetCuttingPriceDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _itemRepository.GetCuttingPriceDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<List<object>> CuttingPriceList()
        {
            var today = DateTime.UtcNow;
            var list = await _itemRepository.GetAllCuttingPricesList();

            return list.Select(c =>
            {
                var version = ResolveVersion(c.Versions, today);
                return (object)new
                {
                    c.ID,
                    c.CuttingType,
                    c.UnitType,
                    c.SizeRangeMin,
                    c.SizeRangeMax,
                    c.SpecimenTypeId,
                    RatePerUnit = version?.RatePerUnit ?? 0,
                    RatePerUnitHard = version?.RatePerUnitHard ?? 0,
                    EffectiveFrom = version?.EffectiveFrom,
                    FinancialYearId = version?.FinancialYearId
                };
            }).ToList();
        }

        public async Task<object?> GetPriceBySpecimenAndCuttingType(long? specimenTypeId, string cuttingType)
        {
            var master = await _itemRepository.GetBySpecimenAndCuttingType(specimenTypeId, cuttingType);
            if (master == null)
                return null;

            var version = ResolveVersion(master.Versions, DateTime.UtcNow);
            if (version == null)
                return null;

            return new
            {
                master.ID,
                master.CuttingType,
                master.UnitType,
                master.SpecimenTypeId,
                version.EffectiveFrom,
                version.FinancialYearId,
                version.RatePerUnit,
                version.RatePerUnitHard
            };
        }

        // At least one version required; each must have a Financial Year + valid EffectiveFrom; no duplicate dates.
        private static void ValidateVersions(ICollection<CuttingPriceVersion> versions)
        {
            if (versions == null || versions.Count == 0)
                throw new ArgumentException("At least one rate version is required!");
            if (versions.Any(v => !v.FinancialYearId.HasValue))
                throw new ArgumentException("Each rate version must have a Financial Year selected!");
            if (versions.Any(v => v.EffectiveFrom == default))
                throw new ArgumentException("Each rate version must have an Effective From date!");
            if (versions.Where(v => v.FinancialYearId.HasValue).GroupBy(v => v.FinancialYearId).Any(g => g.Count() > 1))
                throw new ArgumentException("Duplicate Financial Year in rate versions — each financial year can appear only once!");
            if (versions.GroupBy(v => v.EffectiveFrom.Date).Any(g => g.Count() > 1))
                throw new ArgumentException("Duplicate Effective From date in rate versions — each date can appear only once!");
        }

        // Returns the version with max EffectiveFrom ≤ referenceDate.
        // Fallback: earliest version if referenceDate precedes all versions.
        private static CuttingPriceVersion? ResolveVersion(ICollection<CuttingPriceVersion> versions, DateTime referenceDate)
        {
            if (versions == null || versions.Count == 0)
                return null;
            var active = versions.Where(v => v.IsActive).ToList();
            if (active.Count == 0)
                return null;

            var match = active
                .Where(v => v.EffectiveFrom.Date <= referenceDate.Date)
                .OrderByDescending(v => v.EffectiveFrom)
                .FirstOrDefault();

            return match ?? active.OrderBy(v => v.EffectiveFrom).First();
        }

        // Reconciles tracked Versions against incoming list:
        // update matched rows, add new ones, soft-delete removed ones.
        private void SyncVersions(CuttingPriceMaster existing, ICollection<CuttingPriceVersion> incoming)
        {
            var incomingIds = incoming.Where(v => v.ID > 0).Select(v => v.ID).ToHashSet();

            foreach (var stale in existing.Versions.Where(v => v.IsActive && !incomingIds.Contains(v.ID)))
            {
                stale.IsActive = false;
                stale.ModifiedOn = DateTime.UtcNow;
                stale.ModifiedBy = loggedInUser.EmployeeID;
            }

            foreach (var v in incoming)
            {
                if (v.ID > 0)
                {
                    var current = existing.Versions.FirstOrDefault(x => x.ID == v.ID);
                    if (current == null) continue;
                    current.EffectiveFrom = v.EffectiveFrom;
                    current.FinancialYearId = v.FinancialYearId;
                    current.RatePerUnit = v.RatePerUnit;
                    current.RatePerUnitHard = v.RatePerUnitHard;
                    current.ModifiedOn = DateTime.UtcNow;
                    current.ModifiedBy = loggedInUser.EmployeeID;
                }
                else
                {
                    existing.Versions.Add(new CuttingPriceVersion
                    {
                        EffectiveFrom = v.EffectiveFrom,
                        FinancialYearId = v.FinancialYearId,
                        RatePerUnit = v.RatePerUnit,
                        RatePerUnitHard = v.RatePerUnitHard,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = loggedInUser.EmployeeID,
                        ModifiedOn = null,
                        ModifiedBy = null,
                        CompanyCode = loggedInUser.CompanyCode,
                        IsActive = true
                    });
                }
            }
        }
    }
}
