using LIMSApi.Dtos;
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
        public CuttingService(ICuttingRepository cuttingRepository,  ILogger<CuttingService> logger)
        {
            _cuttingRepository = cuttingRepository;
            _logger = logger;
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

            
            foreach (var sample in payload.Samples)
            {
                sample.SampleTotal = sample.CuttingChargeDetails.Sum(x => x.Total);
            }

            payload.GrandTotal = payload.Samples.Sum(x => x.SampleTotal);

            await _cuttingRepository.CreateAsync(payload);
            _logger.LogInformation("Cutting charges created successfully for InwardID: {InwardID}", payload.InwardID);
        }

        public async Task<PagedResponse<object>> GetAllAsync(PageFilter filter)
        {
             return await _cuttingRepository.GetAllAsync(filter);
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
    }
}
