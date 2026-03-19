using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.ServiceWORepo
{
    public class TpiService : ITpiService
    {
        private readonly LIMSContext _db;
        private readonly ILogger<TpiService> _logger;

        public TpiService(LIMSContext db, ILogger<TpiService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<TpiInspectionDto>> GetInspectionsBySampleInward(long sampleInwardId)
        {
            return await _db.TpiInspections
                .Where(t => t.SampleInwardID == sampleInwardId)
                .Include(t => t.SampleInward)
                .Include(t => t.SampleDetail)
                .Include(t => t.TPIMaster)
                .OrderByDescending(t => t.CreatedOn)
                .Select(t => MapToDto(t))
                .ToListAsync();
        }

        public async Task<TpiInspectionDto?> GetInspectionDetails(long inspectionId)
        {
            var entity = await _db.TpiInspections
                .Include(t => t.SampleInward)
                .Include(t => t.SampleDetail)
                .Include(t => t.TPIMaster)
                .FirstOrDefaultAsync(t => t.ID == inspectionId);

            return entity == null ? null : MapToDto(entity);
        }

        public async Task<TpiInspectionDto> CreateInspection(CreateTpiInspectionDto dto)
        {
            // Validate that the sample inward exists
            var sampleInward = await _db.SampleInwards
                .Include(s => s.SampleDetails)
                .FirstOrDefaultAsync(s => s.ID == dto.SampleInwardID)
                ?? throw new InvalidOperationException("Sample Inward not found.");

            // If a specific sample detail is provided, validate TPI is required for it
            if (dto.SampleDetailID.HasValue)
            {
                var sampleDetail = sampleInward.SampleDetails
                    .FirstOrDefault(sd => sd.ID == dto.SampleDetailID.Value);

                if (sampleDetail == null)
                    throw new InvalidOperationException("Sample Detail not found for this inward.");

                if (!sampleDetail.TpiRequired)
                    throw new InvalidOperationException("TPI is not required for this sample.");
            }
            else
            {
                // If no specific sample, check that at least one sample requires TPI
                var hasTpiSample = sampleInward.SampleDetails.Any(sd => sd.TpiRequired);
                if (!hasTpiSample)
                    throw new InvalidOperationException("No samples in this inward require TPI inspection.");
            }

            // Validate the stage value
            if (!Enum.TryParse<TpiStage>(dto.Stage, true, out var stage))
                throw new InvalidOperationException($"Invalid TPI stage: '{dto.Stage}'. Valid values: BeforePreparation, BeforeTesting.");

            // Validate TPIMaster exists
            var tpiMaster = await _db.Set<TPIMaster>().FindAsync(dto.TPIMasterID)
                ?? throw new InvalidOperationException("TPI Agency not found.");

            var inspection = new TpiInspection
            {
                SampleInwardID = dto.SampleInwardID,
                SampleDetailID = dto.SampleDetailID,
                TPIMasterID = dto.TPIMasterID,
                Stage = stage.ToString(),
                Status = TpiStatus.Pending.ToString(),
                ScheduledDate = dto.ScheduledDate,
                Remarks = dto.Remarks
            };

            _db.TpiInspections.Add(inspection);
            await _db.SaveChangesAsync();

            // Reload with navigation properties
            await _db.Entry(inspection).Reference(t => t.SampleInward).LoadAsync();
            await _db.Entry(inspection).Reference(t => t.SampleDetail).LoadAsync();
            await _db.Entry(inspection).Reference(t => t.TPIMaster).LoadAsync();

            _logger.LogInformation("TPI Inspection created: ID={InspectionId}, Inward={InwardId}, Stage={Stage}",
                inspection.ID, inspection.SampleInwardID, inspection.Stage);

            return MapToDto(inspection);
        }

        public async Task<TpiInspectionDto> UpdateInspectionStatus(long inspectionId, UpdateTpiStatusDto dto)
        {
            var inspection = await _db.TpiInspections
                .Include(t => t.SampleInward)
                .Include(t => t.SampleDetail)
                .Include(t => t.TPIMaster)
                .FirstOrDefaultAsync(t => t.ID == inspectionId)
                ?? throw new InvalidOperationException("TPI Inspection not found.");

            // Validate the status value
            if (!Enum.TryParse<TpiStatus>(dto.Status, true, out var status))
                throw new InvalidOperationException($"Invalid TPI status: '{dto.Status}'. Valid values: Pending, Scheduled, InProgress, Approved, Rejected, Waived.");

            inspection.Status = status.ToString();
            inspection.InspectorComments = dto.InspectorComments ?? inspection.InspectorComments;
            inspection.DocumentPath = dto.DocumentPath ?? inspection.DocumentPath;

            // Set inspection date when status moves to a terminal state
            if (status == TpiStatus.Approved || status == TpiStatus.Rejected || status == TpiStatus.Waived)
            {
                inspection.InspectionDate = DateTime.UtcNow;
            }

            inspection.ModifiedOn = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _logger.LogInformation("TPI Inspection status updated: ID={InspectionId}, Status={Status}",
                inspection.ID, inspection.Status);

            return MapToDto(inspection);
        }

        public async Task<bool> IsTpiCleared(long sampleInwardId, TpiStage stage)
        {
            var stageStr = stage.ToString();

            // Check if there are any TPI inspections for this stage
            var inspections = await _db.TpiInspections
                .Where(t => t.SampleInwardID == sampleInwardId && t.Stage == stageStr)
                .ToListAsync();

            // If no inspections exist for this stage, TPI is not cleared
            if (!inspections.Any())
                return false;

            // All inspections for this stage must be Approved or Waived
            return inspections.All(t =>
                t.Status == TpiStatus.Approved.ToString() ||
                t.Status == TpiStatus.Waived.ToString());
        }

        public async Task<List<TpiInspectionDto>> GetPendingInspections()
        {
            var pendingStatuses = new[]
            {
                TpiStatus.Pending.ToString(),
                TpiStatus.Scheduled.ToString(),
                TpiStatus.InProgress.ToString()
            };

            return await _db.TpiInspections
                .Where(t => pendingStatuses.Contains(t.Status))
                .Include(t => t.SampleInward)
                .Include(t => t.SampleDetail)
                .Include(t => t.TPIMaster)
                .OrderBy(t => t.ScheduledDate ?? DateTime.MaxValue)
                .ThenByDescending(t => t.CreatedOn)
                .Select(t => MapToDto(t))
                .ToListAsync();
        }

        private static TpiInspectionDto MapToDto(TpiInspection entity)
        {
            return new TpiInspectionDto
            {
                Id = entity.ID,
                SampleInwardId = entity.SampleInwardID,
                SampleInwardNumber = entity.SampleInward?.CaseNo,
                SampleDetailId = entity.SampleDetailID,
                SampleNo = entity.SampleDetail?.SampleNo,
                TpiMasterId = entity.TPIMasterID,
                TpiName = entity.TPIMaster?.AgencyName,
                Stage = entity.Stage,
                Status = entity.Status,
                ScheduledDate = entity.ScheduledDate,
                InspectionDate = entity.InspectionDate,
                Remarks = entity.Remarks,
                InspectorComments = entity.InspectorComments,
                DocumentPath = entity.DocumentPath,
                CreatedOn = entity.CreatedOn
            };
        }
    }
}
