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
                where sd.IsActive && si.IsActive && sd.PreparationRequired
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
                    PreparationRequired = sd.PreparationRequired
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
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive)
                ?? throw new KeyNotFoundException("Preparation record not found.");

            return MapToDetailDto(sp);
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
                .FirstOrDefaultAsync(x => x.SampleID == sampleId && x.IsActive);

            if (existing != null)
                return MapToDetailDto(existing);

            // Auto-create from sample with full context
            var sample = await _db.SampleDetails
                .Include(s => s.SampleInward)
                .Include(s => s.ProductCondition)
                .FirstOrDefaultAsync(s => s.ID == sampleId)
                ?? throw new KeyNotFoundException("Sample not found.");

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

            var sp = new SamplePreparation
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
            var sample = await _db.SampleDetails.FindAsync(sp.SampleID);
            sp.OtherChargesTotal = (sample?.OtherPreparation == true ? sample.OtherPreparationCharge : 0);
        }

        private SamplePreparationDetailDto MapToDetailDto(SamplePreparation sp)
        {
            var sample = sp.Sample;
            var inward = sample?.SampleInward;

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
                PreparationInstructions = sp.PreparationInstructions,
                PreConditionNotes = sp.PreConditionNotes,
                PostConditionNotes = sp.PostConditionNotes,
                VerificationRemarks = sp.VerificationRemarks,
                CuttingChargesTotal = sp.CuttingChargesTotal,
                MachiningChargesTotal = sp.MachiningChargesTotal,
                OtherChargesTotal = sp.OtherChargesTotal,
                Thickness = sample?.Thickness,
                Diameter = sample?.Diameter,
                Width = sample?.Width,
                Length = sample?.Length,
                IsInvoiced = inward?.IsInvoiceGenerated ?? false,

                // Plan-level fields
                PreparationRequired = sample?.PreparationRequired ?? false,
                MachiningRequired = sample?.MachiningRequired ?? false,
                MachiningAmountFromPlan = sample?.MachiningAmount ?? 0,
                OtherPreparation = sample?.OtherPreparation ?? false,
                OtherPreparationCharge = sample?.OtherPreparationCharge ?? 0,
                Specimen = sample?.Specimen,
                TestInstructions = sample?.TestInstructions
            };
        }
    }
}
