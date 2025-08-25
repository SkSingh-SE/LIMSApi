using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Services
{
    public class SampleInwardService : ISampleInwardService
    {
        private readonly ISampleInwardRepository _SampleInwardRepository;
        private readonly ILogger<SampleInwardService> _logger;
        private readonly IFileUploadService _uploadService;

        public SampleInwardService(ISampleInwardRepository SampleInwardRepo, ILogger<SampleInwardService> logger, IFileUploadService uploadService)
        {
            _SampleInwardRepository = SampleInwardRepo;
            _logger = logger;
            _uploadService = uploadService;
        }

        public async Task CreateSampleInward([FromForm] SampleInward model)
        {
            if (model.File != null)
            {
                var fileUploadResponse = await _uploadService.UploadFileAsync(model.File, FileType.Other, null, model.RequestFileName);
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");
                model.RequestFilePath = fileUploadResponse.FilePath;
                model.RequestFileName = fileUploadResponse.OriginalFileName;
                model.UploadReferenceID = fileUploadResponse.ID;
            }
           
            if(model.SampleDetails.Any())
            {
                foreach(var sampleDetail in model.SampleDetails)
                {
                    if (sampleDetail.File != null)
                    {
                        var fileUploadResponse = await _uploadService.UploadFileAsync(sampleDetail.File, FileType.Other, null, sampleDetail.FileName);
                        if (fileUploadResponse == null)
                            throw new InvalidOperationException("File upload failed for sample detail!");
                        sampleDetail.SampleFilePath = fileUploadResponse.FilePath;
                        sampleDetail.FileName = fileUploadResponse.OriginalFileName;
                        sampleDetail.UploadReferenceID = fileUploadResponse.ID;
                    }
                }
            }

            await _SampleInwardRepository.AddSampleInward(model);
            _logger.LogInformation("SampleInward '{Case}' created successfully.", model.CaseNo);
        }

        public async Task ModifySampleInward(SampleInward model)
        {
            if (model.ID == 0)
                throw new ArgumentException("SampleInward ID should not be empty!");

            var existingSampleInward = await _SampleInwardRepository.GetSampleInwardById(model.ID);
            if (existingSampleInward == null)
                throw new InvalidOperationException("SampleInward not found!");

           

            await _SampleInwardRepository.UpdateSampleInward(existingSampleInward);
            _logger.LogInformation("SampleInward '{Case}' created successfully.", model.CaseNo);
        }


        public async Task RemoveSampleInward(long id)
        {
            var existingSampleInward = await _SampleInwardRepository.GetSampleInwardById(id);
            if (existingSampleInward == null)
                throw new InvalidOperationException("SampleInward not found!");

            existingSampleInward.IsActive = false;
            existingSampleInward.ModifiedOn = DateTime.UtcNow;

            await _SampleInwardRepository.UpdateSampleInward(existingSampleInward);
            _logger.LogInformation("SampleInward with ID '{SampleInwardId}' deleted successfully.", id);
        }

        public async Task<SampleInward> GetSampleInwardDetails(long id)
        {
            var classification = await _SampleInwardRepository.GetSampleInwardById(id);
            if (classification == null)
                throw new InvalidOperationException("SampleInward not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchSampleInwardList(PageFilter filter)
        {
            return await _SampleInwardRepository.GetAllSampleInwards(filter);
        }

        public async Task<object> GetCaseNoAndSampleNo()
        {
            var caseNumber = await _SampleInwardRepository.GetCaseNoAndSampleNo();
            if (caseNumber == null)
                throw new InvalidOperationException("No case number found!");
            return caseNumber;
        }
    }
}
