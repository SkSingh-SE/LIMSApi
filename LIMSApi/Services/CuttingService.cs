using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace LIMSApi.Services
{
    public class CuttingService : ICuttingService
    {
        private readonly ICuttingRepository _cuttingRepository;
        private readonly ILogger<CuttingService> _logger;
        private LoggedInUserDTO loggedInUser;
        private readonly ISampleStatusService _sampleStatusService;
        public CuttingService(ICuttingRepository cuttingRepository,  ILogger<CuttingService> logger, ISampleStatusService sampleStatusService)
        {
            _cuttingRepository = cuttingRepository;
            _logger = logger;
            _sampleStatusService = sampleStatusService;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateAsync(CuttingChargeHeader payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            if (payload.InwardID <= 0)
                throw new Exception("Invalid InwardId.");

            if (payload.Samples == null || payload.Samples.Count == 0)
                throw new Exception("At least one sample is required.");


            var exists = await _cuttingRepository.ExistsByInwardIdAsync(payload.InwardID);

            if (exists)
                throw new Exception("Cutting charges already exist for this inward.");

            var statusJobs = new List<Func<Task>>();
            foreach (var sample in payload.Samples)
            {
                sample.SampleTotal = sample.CuttingChargeDetails.Sum(x => x.Total);
                statusJobs.Add(async () =>
                {
                    await _sampleStatusService.ForceAutoStatusAsync(
                        sample.SampleID,
                        SampleStatus.PREPARATION_COMPLETED,
                        loggedInUser.EmployeeID
                    );
                });
            }

            payload.GrandTotal = payload.Samples.Sum(x => x.SampleTotal);

            await _cuttingRepository.CreateAsync(payload);
            _logger.LogInformation("Cutting charges created successfully for InwardID: {InwardID}", payload.InwardID);

            // Update Status
            foreach (var job in statusJobs)
            {
                await job();
            }

            
            await _sampleStatusService.UpdateInwardStatus(payload.InwardID, InwardStatus.IN_PROGRESS, loggedInUser.EmployeeID);
        }

        public async Task<PagedResponse<object>> GetAllAsync(PageFilter filter)
        {
             return await _cuttingRepository.GetAllCuttingList(filter);
        }

        public async Task<CuttingChargeHeader?> GetByIdAsync(long id)
        {
            return await _cuttingRepository.GetByIdAsync(id);
        }

        public async Task<CuttingChargeHeader?> GetByInwardIdAsync(long inwardId)
        {
            return await _cuttingRepository.GetByInwardIdAsync(inwardId);
        }

        public async Task UpdateAsync(CuttingChargeHeader model)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));
            if (model.ID <= 0)
                throw new Exception("Invalid CuttingChargeHeader Id.");
            if(model.InwardID <= 0)
                throw new Exception("Invalid InwardId.");

            var existingRecord = await _cuttingRepository.GetByIdAsync(model.ID);
            if (existingRecord == null)
                throw new Exception("CuttingChargeHeader record not found.");


            foreach (var sample in model.Samples)
            {
                sample.SampleTotal = sample.CuttingChargeDetails.Sum(x => x.Total);
            }
            model.GrandTotal = model.Samples.Sum(x => x.SampleTotal);


            await _cuttingRepository.UpdateAsync(model);
        }

        public async Task UpdatePreparationStatusAsync(long sampleId, string status)
        {
            var validStatuses = new[] { "Pending", "InProgress", "Completed", "QCVerified" };
            if (!validStatuses.Contains(status))
                throw new Exception($"Invalid preparation status: '{status}'. Valid values: {string.Join(", ", validStatuses)}");

            var sample = await _cuttingRepository.GetSampleByIdAsync(sampleId);
            if (sample == null)
                throw new Exception("Cutting charge sample not found.");

            sample.PreparationStatus = status;
            await _cuttingRepository.UpdateSampleAsync(sample);

            _logger.LogInformation("Preparation status updated to '{Status}' for CuttingChargeSample ID: {SampleId}", status, sampleId);
        }
    }
}
