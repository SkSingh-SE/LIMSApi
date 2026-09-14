using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace LIMSApi.Services
{
    public class SamplePreparationService : ISamplePreparationService
    {
        private readonly LIMSContext _db;
        private readonly ILogger<SamplePreparationService> _logger;
        private readonly LoggedInUserDTO _loggedInUser;

        public SamplePreparationService(LIMSContext db, ILogger<SamplePreparationService> logger)
        {
            _db = db;
            _logger = logger;
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<PagedResponse<SamplePreparationListDto>> GetAllAsync(PageFilter filter)
        {
            var query =
                from sd in _db.SampleDetails
                join si in _db.SampleInwards on sd.InwardID equals si.ID
                join c in _db.Customers on si.CustomerID equals c.ID
                where sd.IsActive && si.IsActive
                      && (sd.TestPlans.Any(tp => tp.GeneralTests.Any(gt => gt.Methods.Any(m => !m.Cancel && m.PreparationRequired)) || tp.ChemicalTests.Any(ct => ct.Methods.Any(m => !m.Cancel && m.PreparationRequired))) || _db.SamplePreparations.Any(sp => sp.SampleID == sd.ID))
                      && si.CompanyCode == _loggedInUser.CompanyCode

                // Left join to SamplePreparation
                join sp in _db.SamplePreparations on sd.ID equals sp.SampleID into spJoin
                from sp in spJoin.DefaultIfEmpty()

                // Left join for assigned employee
                join emp in _db.EmployeeMasters on sp.AssignedToEmployeeID equals emp.ID into empJoin
                from emp in empJoin.DefaultIfEmpty()

                // Left join for prepared by
                join prep in _db.EmployeeMasters on sp.PreparedByEmployeeID equals prep.ID into prepJoin
                from prep in prepJoin.DefaultIfEmpty()

                select new SamplePreparationListDto
                {
                    Id = sp != null ? sp.ID : 0,
                    SampleID = sd.ID,
                    SampleNo = sd.SampleNo,
                    InwardID = si.ID,
                    CaseNo = si.CaseNo,
                    CustomerID = c.ID,
                    CustomerName = c.Name,
                    SampleDescription = sd.Details,
                    MaterialClassification = sd.MetalClassification != null ? sd.MetalClassification.Name : null,
                    Status = sp != null ? sp.Status : "Pending",
                    AssignedToName = emp != null ? emp.Name : null,
                    PreparedByName = prep != null ? prep.Name : null,
                    StartedOn = sp != null ? sp.StartedOn : null,
                    CompletedOn = sp != null ? sp.CompletedOn : null,
                    CuttingChargesTotal = sp != null ? sp.CuttingChargesTotal : 0,
                    MachiningChargesTotal = sp != null ? sp.MachiningChargesTotal : 0,
                    OtherChargesTotal = sp != null ? sp.OtherChargesTotal : 0,
                    IsInvoiced = si.IsInvoiceGenerated,
                    PreparationRequired = sd.TestPlans.Any(tp => tp.GeneralTests.Any(gt => gt.Methods.Any(m => !m.Cancel && m.PreparationRequired)) || tp.ChemicalTests.Any(ct => ct.Methods.Any(m => !m.Cancel && m.PreparationRequired)))
                };

            // Search
            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                query = query.Where(x =>
                    x.SampleNo.Contains(search) ||
                    x.CaseNo.Contains(search) ||
                    (x.CustomerName != null && x.CustomerName.Contains(search)) ||
                    (x.SampleDescription != null && x.SampleDescription.Contains(search)) ||
                    x.Status.Contains(search));
            }

            // Sort
            if (!string.IsNullOrWhiteSpace(filter.SortByColumn))
                query = query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            else
                query = query.OrderByDescending(x => x.InwardID);

            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<SamplePreparationListDto>(items, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<SamplePreparationDetailDto> GetByIdAsync(long id)
        {
            var sp = await _db.SamplePreparations
                .Include(x => x.Sample).ThenInclude(s => s!.SampleInward).ThenInclude(i => i!.Customer)
                .Include(x => x.Sample).ThenInclude(s => s!.MetalClassification)
                .Include(x => x.AssignedTo)
                .Include(x => x.PreparedBy)
                .Include(x => x.VerifiedBy)
                .Include(x => x.Equipment)
                .Include(x => x.TestItems).ThenInclude(ti => ti.LaboratoryTest)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive)
                ?? throw new KeyNotFoundException("Preparation record not found.");

            return await MapToDetailDtoAsync(sp);
        }

        public async Task<SamplePreparationDetailDto> GetOrCreateBySampleAsync(long sampleId)
        {
            var existing = await _db.SamplePreparations
                .Include(x => x.Sample).ThenInclude(s => s!.SampleInward).ThenInclude(i => i!.Customer)
                .Include(x => x.Sample).ThenInclude(s => s!.MetalClassification)
                .Include(x => x.AssignedTo)
                .Include(x => x.PreparedBy)
                .Include(x => x.VerifiedBy)
                .Include(x => x.Equipment)
                .Include(x => x.TestItems).ThenInclude(ti => ti.LaboratoryTest)
                .FirstOrDefaultAsync(x => x.SampleID == sampleId && x.IsActive);

            var sample = await _db.SampleDetails
                .Include(s => s.SampleInward)
                .Include(s => s.ProductCondition)
                .FirstOrDefaultAsync(s => s.ID == sampleId)
                ?? throw new KeyNotFoundException("Sample not found.");

            SamplePreparation sp;
            if (existing != null)
            {
                sp = existing;
            }
            else
            {
                // Auto-fill environment conditions from today's monitoring
                decimal? autoTemp = null, autoHumidity = null;
                var today = DateTime.UtcNow.Date;
                var envRecord = await _db.EnvironmentDailyRecords
                    .Where(r => r.RecordDate.Date == today && r.IsActive)
                    .OrderByDescending(r => r.CreatedOn)
                    .FirstOrDefaultAsync();
                if (envRecord != null)
                {
                    autoTemp = envRecord.Temperature;
                    autoHumidity = envRecord.Humidity;
                }

                sp = new SamplePreparation
                {
                    SampleID = sampleId,
                    InwardID = sample.InwardID,
                    Status = "Pending",
                    PreparationInstructions = sample.TestInstructions,
                    Temperature = autoTemp,
                    Humidity = autoHumidity,
                    CreatedBy = _loggedInUser.EmployeeID,
                    CreatedOn = DateTime.UtcNow,
                    CompanyCode = _loggedInUser.CompanyCode,
                    IsActive = true
                };

                // Pull existing charges
                await SyncChargeTotals(sp);

                _db.SamplePreparations.Add(sp);
                await _db.SaveChangesAsync();
            }

            // Auto-seed SamplePreparationTestItems from SampleTestPlan if missing
            var testPlan = await _db.TestPlans
                .Include(tp => tp.GeneralTests).ThenInclude(gt => gt.Methods)
                .Include(tp => tp.ChemicalTests).ThenInclude(ct => ct.Methods)
                .FirstOrDefaultAsync(tp => tp.SampleID == sampleId);

            bool addedItems = false;
            if (testPlan != null)
            {
                foreach (var gt in testPlan.GeneralTests)
                {
                    foreach (var m in gt.Methods.Where(m => !m.Cancel))
                    {
                        if (!sp.TestItems.Any(ti => ti.PlannedTestMethodID == m.ID && ti.PlannedTestType == "General"))
                        {
                            var item = new SamplePreparationTestItem
                            {
                                SamplePreparationID = sp.ID,
                                SampleID = sampleId,
                                TestPlanID = testPlan.ID,
                                PlannedTestType = "General",
                                PlannedTestMethodID = m.ID,
                                LaboratoryTestID = m.LaboratoryTestID,
                                SpecimenSize = sample.Specimen ?? "Standard Specimen",
                                Quantity = m.Quantity > 0 ? m.Quantity : 1,
                                CuttingRequired = m.PreparationRequired,
                                MachiningRequired = m.PreparationRequired,
                                Status = m.PreparationRequired ? "Required" : "Completed",
                                CreatedBy = _loggedInUser.EmployeeID,
                                CreatedOn = DateTime.UtcNow,
                                CompanyCode = _loggedInUser.CompanyCode,
                                IsActive = true
                            };
                            if (!m.PreparationRequired)
                            {
                                item.CompletedOn = DateTime.UtcNow;
                            }
                            _db.SamplePreparationTestItems.Add(item);
                            sp.TestItems.Add(item);
                            addedItems = true;
                        }
                    }
                }

                foreach (var ct in testPlan.ChemicalTests)
                {
                    foreach (var m in ct.Methods.Where(m => !m.Cancel))
                    {
                        if (!sp.TestItems.Any(ti => ti.PlannedTestMethodID == m.ID && ti.PlannedTestType == "Chemical"))
                        {
                            long labTestId = 0;
                            long? analysisTypeId = ct.LaboratoryTestAnalysisTypeID;
                            if (!analysisTypeId.HasValue)
                            {
                                var firstElem = await _db.ChemicalTestElements.FirstOrDefaultAsync(e => e.ChemicalTestID == ct.ID && e.LaboratoryTestAnalysisTypeID.HasValue);
                                if (firstElem != null) analysisTypeId = firstElem.LaboratoryTestAnalysisTypeID;
                            }
                            if (analysisTypeId.HasValue)
                            {
                                var analysisType = await _db.LaboratoryTestAnalysisTypes.Include(at => at.SubGroup).FirstOrDefaultAsync(at => at.ID == analysisTypeId.Value);
                                if (analysisType?.SubGroup != null) labTestId = analysisType.SubGroup.LaboratoryTestID;
                            }

                            var item = new SamplePreparationTestItem
                            {
                                SamplePreparationID = sp.ID,
                                SampleID = sampleId,
                                TestPlanID = testPlan.ID,
                                PlannedTestType = "Chemical",
                                PlannedTestMethodID = m.ID,
                                LaboratoryTestID = labTestId,
                                TestMethodSpecificationID = m.TestMethodSpecificationID,
                                SpecimenSize = sample.Specimen ?? "Standard Specimen",
                                Quantity = m.Quantity > 0 ? m.Quantity : 1,
                                CuttingRequired = m.PreparationRequired,
                                MachiningRequired = m.PreparationRequired,
                                Status = m.PreparationRequired ? "Required" : "Completed",
                                CreatedBy = _loggedInUser.EmployeeID,
                                CreatedOn = DateTime.UtcNow,
                                CompanyCode = _loggedInUser.CompanyCode,
                                IsActive = true
                            };
                            if (!m.PreparationRequired)
                            {
                                item.CompletedOn = DateTime.UtcNow;
                            }
                            _db.SamplePreparationTestItems.Add(item);
                            sp.TestItems.Add(item);
                            addedItems = true;
                        }
                    }
                }
            }

            if (addedItems)
            {
                await _db.SaveChangesAsync();
            }

            return await GetByIdAsync(sp.ID);
        }

        public async Task<SamplePreparationDetailDto> CreateAsync(SamplePreparationCreateDto dto)
        {
            var sample = await _db.SampleDetails
                .Include(s => s.SampleInward)
                .FirstOrDefaultAsync(s => s.ID == dto.SampleID)
                ?? throw new KeyNotFoundException("Sample not found.");

            // Check not already created
            var exists = await _db.SamplePreparations
                .AnyAsync(x => x.SampleID == dto.SampleID && x.IsActive);
            if (exists)
                throw new InvalidOperationException("Preparation record already exists for this sample.");

            var sp = new SamplePreparation
            {
                SampleID = dto.SampleID,
                InwardID = dto.InwardID > 0 ? dto.InwardID : sample.InwardID,
                Status = "Pending",
                AssignedToEmployeeID = dto.AssignedToEmployeeID,
                AssignedOn = dto.AssignedToEmployeeID.HasValue ? DateTime.UtcNow : null,
                EquipmentID = dto.EquipmentID,
                PreparationMethod = dto.PreparationMethod,
                PreparationInstructions = dto.PreparationInstructions,
                PreConditionNotes = dto.PreConditionNotes,
                CreatedBy = _loggedInUser.EmployeeID,
                CreatedOn = DateTime.UtcNow,
                CompanyCode = _loggedInUser.CompanyCode,
                IsActive = true
            };

            await SyncChargeTotals(sp);

            _db.SamplePreparations.Add(sp);
            await _db.SaveChangesAsync();

            _logger.LogInformation("SamplePreparation created for SampleID {SampleID}", dto.SampleID);
            return await GetByIdAsync(sp.ID);
        }

        public async Task<SamplePreparationDetailDto> UpdateAsync(SamplePreparationUpdateDto dto)
        {
            var sp = await _db.SamplePreparations.FindAsync(dto.ID)
                ?? throw new KeyNotFoundException("Preparation record not found.");

            // Invoice lock
            var isInvoiced = await _db.SampleInwards
                .Where(x => x.ID == sp.InwardID)
                .Select(x => x.IsInvoiceGenerated)
                .FirstOrDefaultAsync();
            if (isInvoiced)
                throw new InvalidOperationException("Cannot modify preparation after invoice generation.");

            if (!string.IsNullOrWhiteSpace(dto.Status)) sp.Status = dto.Status;
            if (dto.AssignedToEmployeeID.HasValue)
            {
                sp.AssignedToEmployeeID = dto.AssignedToEmployeeID;
                sp.AssignedOn ??= DateTime.UtcNow;
            }
            if (dto.PreparedByEmployeeID.HasValue) sp.PreparedByEmployeeID = dto.PreparedByEmployeeID;
            if (dto.VerifiedByEmployeeID.HasValue) sp.VerifiedByEmployeeID = dto.VerifiedByEmployeeID;
            if (dto.EquipmentID.HasValue) sp.EquipmentID = dto.EquipmentID;
            if (dto.PreparationMethod != null) sp.PreparationMethod = dto.PreparationMethod;
            if (dto.Temperature.HasValue) sp.Temperature = dto.Temperature;
            if (dto.Humidity.HasValue) sp.Humidity = dto.Humidity;
            if (dto.PreparationInstructions != null) sp.PreparationInstructions = dto.PreparationInstructions;
            if (dto.PreConditionNotes != null) sp.PreConditionNotes = dto.PreConditionNotes;
            if (dto.PostConditionNotes != null) sp.PostConditionNotes = dto.PostConditionNotes;
            if (dto.VerificationRemarks != null) sp.VerificationRemarks = dto.VerificationRemarks;

            if (dto.NumberOfCuts.HasValue) sp.NumberOfCuts = dto.NumberOfCuts;
            if (dto.CutThickness.HasValue) sp.CutThickness = dto.CutThickness;
            if (dto.WaterJetCuttingMins.HasValue) sp.WaterJetCuttingMins = dto.WaterJetCuttingMins;
            if (dto.EdmCutting != null) sp.EdmCutting = dto.EdmCutting;
            if (dto.EdmCuttingCharge.HasValue) sp.EdmCuttingCharge = dto.EdmCuttingCharge.Value;
            if (dto.GasCutting != null) sp.GasCutting = dto.GasCutting;
            if (dto.GasCuttingCharge.HasValue) sp.GasCuttingCharge = dto.GasCuttingCharge.Value;
            if (dto.SpecialCutting != null) sp.SpecialCutting = dto.SpecialCutting;
            if (dto.SpecialCuttingCharge.HasValue) sp.SpecialCuttingCharge = dto.SpecialCuttingCharge.Value;

            if (dto.Items != null && dto.Items.Count > 0)
            {
                var existingItems = await _db.SamplePreparationTestItems
                    .Where(x => x.SamplePreparationID == sp.ID && x.IsActive)
                    .ToListAsync();

                foreach (var itemDto in dto.Items)
                {
                    var item = existingItems.FirstOrDefault(x => x.ID == itemDto.Id)
                        ?? existingItems.FirstOrDefault(x => x.PlannedTestMethodID == itemDto.PlannedTestMethodID && x.LaboratoryTestID == itemDto.LaboratoryTestID);

                    if (item != null)
                    {
                        if (itemDto.SpecimenPreparationMasterID.HasValue && itemDto.SpecimenPreparationMasterID > 0)
                            item.SpecimenPreparationMasterID = itemDto.SpecimenPreparationMasterID;
                        if (!string.IsNullOrEmpty(itemDto.SpecimenSize))
                            item.SpecimenSize = itemDto.SpecimenSize;
                        if (!string.IsNullOrEmpty(itemDto.SpecimenRawMaterialSize))
                            item.SpecimenRawMaterialSize = itemDto.SpecimenRawMaterialSize;
                        if (itemDto.Quantity > 0)
                            item.Quantity = itemDto.Quantity;
                        item.CuttingRequired = itemDto.CuttingRequired;
                        item.MachiningRequired = itemDto.MachiningRequired;
                        item.NoTesting = itemDto.NoTesting;
                        if (itemDto.ResolvedCuttingRate > 0)
                            item.ResolvedCuttingRate = itemDto.ResolvedCuttingRate;
                        if (itemDto.ResolvedMachiningRate > 0)
                            item.ResolvedMachiningRate = itemDto.ResolvedMachiningRate;
                        item.CuttingTotal = item.CuttingRequired ? item.ResolvedCuttingRate * item.Quantity : 0;
                        item.MachiningTotal = item.MachiningRequired ? item.ResolvedMachiningRate * item.Quantity : 0;
                        if (!string.IsNullOrEmpty(itemDto.Status))
                            item.Status = itemDto.Status;
                        item.ModifiedBy = _loggedInUser.EmployeeID;
                        item.ModifiedOn = DateTime.UtcNow;
                    }
                }
            }

            sp.ModifiedBy = _loggedInUser.EmployeeID;
            sp.ModifiedOn = DateTime.UtcNow;

            await SyncChargeTotals(sp);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(sp.ID);
        }

        public async Task UpdateStatusAsync(long id, SamplePreparationStatusDto dto)
        {
            var sp = await _db.SamplePreparations.FindAsync(id)
                ?? throw new KeyNotFoundException("Preparation record not found.");

            var validStatuses = new[] { "Pending", "Assigned", "InProgress", "Completed", "QCVerified", "Rejected" };
            if (!validStatuses.Contains(dto.Status))
                throw new ArgumentException($"Invalid status: {dto.Status}");

            sp.Status = dto.Status;
            sp.ModifiedBy = _loggedInUser.EmployeeID;
            sp.ModifiedOn = DateTime.UtcNow;

            switch (dto.Status)
            {
                case "InProgress":
                    sp.StartedOn ??= DateTime.UtcNow;
                    sp.PreparedByEmployeeID ??= _loggedInUser.EmployeeID;
                    break;
                case "Completed":
                    sp.CompletedOn = DateTime.UtcNow;
                    sp.PreparedByEmployeeID ??= _loggedInUser.EmployeeID;
                    break;
                case "QCVerified":
                    sp.VerifiedOn = DateTime.UtcNow;
                    sp.VerifiedByEmployeeID = _loggedInUser.EmployeeID;
                    sp.VerificationRemarks = dto.Remarks;
                    break;
                case "Rejected":
                    sp.VerificationRemarks = dto.Remarks;
                    break;
            }

            await SyncChargeTotals(sp);
            await _db.SaveChangesAsync();

            _logger.LogInformation("SamplePreparation {ID} status changed to {Status}", id, dto.Status);
        }

        public async Task RefreshChargeTotalsAsync(long sampleId)
        {
            var sp = await _db.SamplePreparations
                .FirstOrDefaultAsync(x => x.SampleID == sampleId && x.IsActive);
            if (sp == null) return;

            await SyncChargeTotals(sp);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateItemStatusAsync(long itemId, SamplePreparationItemUpdateDto dto)
        {
            var item = await _db.SamplePreparationTestItems.FindAsync(itemId)
                ?? throw new KeyNotFoundException("Preparation item not found.");

            item.Status = dto.Status;
            if (dto.Status == "Completed")
            {
                item.CompletedOn = DateTime.UtcNow;
                item.CompletedByEmployeeID = dto.CompletedByEmployeeID ?? _loggedInUser.EmployeeID;
            }
            if (!string.IsNullOrWhiteSpace(dto.Remarks))
            {
                item.Remarks = dto.Remarks;
            }
            item.ModifiedBy = _loggedInUser.EmployeeID;
            item.ModifiedOn = DateTime.UtcNow;

            // Check parent SamplePreparation: if all items are completed, mark parent SamplePreparation as "Completed"
            var parent = await _db.SamplePreparations
                .Include(sp => sp.TestItems)
                .FirstOrDefaultAsync(sp => sp.ID == item.SamplePreparationID);

            if (parent != null)
            {
                var activeItems = parent.TestItems.Where(x => x.IsActive && x.Status != "Not Required" && x.Status != "Cancelled").ToList();
                if (activeItems.Any() && activeItems.All(x => x.Status == "Completed"))
                {
                    parent.Status = "Completed";
                    parent.CompletedOn ??= DateTime.UtcNow;
                    parent.PreparedByEmployeeID ??= _loggedInUser.EmployeeID;
                }
                else if (activeItems.Any(x => x.Status == "InProgress" || x.Status == "CuttingCompleted" || x.Status == "MachiningCompleted"))
                {
                    parent.Status = "InProgress";
                    parent.StartedOn ??= DateTime.UtcNow;
                }
            }

            await _db.SaveChangesAsync();
        }

        // ── Private Helpers ──

        private async Task SyncChargeTotals(SamplePreparation sp)
        {
            // Cutting charges
            sp.CuttingChargesTotal = await _db.CuttingChargeHeaders
                .Where(c => c.InwardID == sp.InwardID && c.IsActive)
                .Select(c => c.GrandTotal)
                .FirstOrDefaultAsync();

            // Machining charges
            sp.MachiningChargesTotal = await _db.MachiningChargeItems
                .Where(m => m.SampleID == sp.SampleID && m.IsActive)
                .SumAsync(m => m.Amount);

            // Other preparation charges
            sp.OtherChargesTotal = 0;
        }

        private async Task<SamplePreparationDetailDto> MapToDetailDtoAsync(SamplePreparation sp)
        {
            var sample = sp.Sample;
            var inward = sample?.SampleInward;

            var missingTestIds = sp.TestItems?
                .Where(ti => ti.IsActive && ti.LaboratoryTestID > 0 && ti.LaboratoryTest == null)
                .Select(ti => ti.LaboratoryTestID)
                .Distinct()
                .ToList() ?? new List<long>();

            var subGroupMap = missingTestIds.Count > 0
                ? await _db.LaboratoryTestSubGroups
                    .Include(sg => sg.LaboratoryTest)
                    .Where(sg => missingTestIds.Contains(sg.ID))
                    .ToDictionaryAsync(sg => sg.ID, sg => sg)
                : new Dictionary<long, LaboratoryTestSubGroup>();

            var directTestMap = missingTestIds.Count > 0
                ? await _db.LaboratoryTests
                    .Where(lt => missingTestIds.Contains(lt.ID))
                    .ToDictionaryAsync(lt => lt.ID, lt => lt.Name)
                : new Dictionary<long, string>();

            return new SamplePreparationDetailDto
            {
                Id = sp.ID,
                SampleID = sp.SampleID,
                SampleNo = sample?.SampleNo ?? "",
                InwardID = sp.InwardID,
                CaseNo = inward?.CaseNo ?? "",
                CustomerName = inward?.Customer?.Name,
                SampleDescription = sample?.Details,
                Status = sp.Status,
                AssignedToEmployeeID = sp.AssignedToEmployeeID,
                AssignedToName = sp.AssignedTo?.Name,
                PreparedByEmployeeID = sp.PreparedByEmployeeID,
                PreparedByName = sp.PreparedBy?.Name,
                VerifiedByEmployeeID = sp.VerifiedByEmployeeID,
                VerifiedByName = sp.VerifiedBy?.Name,
                AssignedOn = sp.AssignedOn,
                StartedOn = sp.StartedOn,
                CompletedOn = sp.CompletedOn,
                VerifiedOn = sp.VerifiedOn,
                EquipmentID = sp.EquipmentID,
                EquipmentName = sp.Equipment?.Name,
                PreparationMethod = sp.PreparationMethod,
                Temperature = sp.Temperature,
                Humidity = sp.Humidity,
                PreparationInstructions = sample?.TestInstructions ?? sp.PreparationInstructions,
                PreConditionNotes = sp.PreConditionNotes,
                PostConditionNotes = sp.PostConditionNotes,
                VerificationRemarks = sp.VerificationRemarks,
                CuttingChargesTotal = sp.CuttingChargesTotal,
                MachiningChargesTotal = sp.MachiningChargesTotal,
                OtherChargesTotal = sp.OtherChargesTotal,
                NumberOfCuts = sp.NumberOfCuts,
                CutThickness = sp.CutThickness,
                WaterJetCuttingMins = sp.WaterJetCuttingMins,
                EdmCutting = sp.EdmCutting,
                EdmCuttingCharge = sp.EdmCuttingCharge,
                GasCutting = sp.GasCutting,
                GasCuttingCharge = sp.GasCuttingCharge,
                SpecialCutting = sp.SpecialCutting,
                SpecialCuttingCharge = sp.SpecialCuttingCharge,
                Thickness = sample?.Thickness,
                Diameter = sample?.Diameter,
                Width = sample?.Width,
                Length = sample?.Length,
                IsInvoiced = inward?.IsInvoiceGenerated ?? false,

                // Plan-level fields
                PreparationRequired = sample?.TestPlans.Any(tp => tp.GeneralTests.Any(gt => gt.Methods.Any(m => !m.Cancel && m.PreparationRequired)) || tp.ChemicalTests.Any(ct => ct.Methods.Any(m => !m.Cancel && m.PreparationRequired))) ?? false,
                MachiningRequired = false,
                MachiningAmountFromPlan = 0,
                OtherPreparation = false,
                OtherPreparationCharge = 0,
                Specimen = sample?.Specimen,
                TestInstructions = sample?.TestInstructions,

                // Test-wise preparation items
                Items = sp.TestItems?.Where(ti => ti.IsActive).Select(ti =>
                {
                    var resolvedName = ti.LaboratoryTest?.Name
                        ?? (subGroupMap.TryGetValue(ti.LaboratoryTestID, out var sg)
                            ? (sg.LaboratoryTest?.Name ?? (!string.IsNullOrWhiteSpace(sg.ReportTestName) ? sg.ReportTestName : sg.Name))
                            : (directTestMap.TryGetValue(ti.LaboratoryTestID, out var dName) ? dName : null));

                    return new SamplePreparationTestItemDto
                    {
                        Id = ti.ID,
                        SamplePreparationID = ti.SamplePreparationID,
                        SampleID = ti.SampleID,
                        TestPlanID = ti.TestPlanID,
                        PlannedTestType = ti.PlannedTestType,
                        PlannedTestMethodID = ti.PlannedTestMethodID,
                        LaboratoryTestID = ti.LaboratoryTestID,
                        LaboratoryTestName = resolvedName,
                        TestMethodSpecificationID = ti.TestMethodSpecificationID,
                        SpecimenSize = ti.SpecimenSize,
                        SpecimenRawMaterialSize = ti.SpecimenRawMaterialSize,
                        Quantity = ti.Quantity,
                        CuttingRequired = ti.CuttingRequired,
                        MachiningRequired = ti.MachiningRequired,
                        NoTesting = ti.NoTesting,
                        PreparationType = ti.CuttingRequired && ti.MachiningRequired ? "Cutting & Machining" : (ti.MachiningRequired ? "Machining" : (ti.CuttingRequired ? "Cutting" : "Preparation")),
                        Status = ti.Status,
                        CompletedOn = ti.CompletedOn,
                        CompletedByEmployeeID = ti.CompletedByEmployeeID,
                        Remarks = ti.Remarks,
                        ResolvedCuttingRate = ti.ResolvedCuttingRate,
                        ResolvedMachiningRate = ti.ResolvedMachiningRate,
                        CuttingTotal = ti.CuttingTotal,
                        MachiningTotal = ti.MachiningTotal
                    };
                }).ToList() ?? new List<SamplePreparationTestItemDto>()
            };
        }
    }
}
