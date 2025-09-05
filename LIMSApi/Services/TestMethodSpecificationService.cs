using System;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class TestMethodSpecificationService : ITestMethodSpecificationService
    {
        private readonly ITestMethodSpecificationRepository _TestMethodSpecificationRepository;
        private readonly ILogger<TestMethodSpecificationService> _logger;
        private LoggedInUserDTO loggedInUser;
        private readonly IFileUploadService _uploadService;

        public TestMethodSpecificationService(ITestMethodSpecificationRepository TestMethodSpecificationRepo, ILogger<TestMethodSpecificationService> logger, IFileUploadService uploadService)
        {
            _TestMethodSpecificationRepository = TestMethodSpecificationRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
            _uploadService = uploadService;
        }

        public async Task CreateTestMethodSpecification(TestMethodSpecification model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("TestMethodSpecification name should not be empty!");

            bool exists = await _TestMethodSpecificationRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("TestMethodSpecification already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            if (model.Versions.Any())
            {
                foreach(var version in model.Versions)
                {
                    version.TestMethodSpecificationID = model.ID;
                    if (version.file != null)
                    {
                        var fileUploadResponse = await _uploadService.UploadFileAsync(version.file, FileType.Other, null, version.StandardFile);
                        if (fileUploadResponse == null)
                            throw new InvalidOperationException("File upload failed!");
                        version.StandardFilePath = fileUploadResponse.FilePath;
                        version.StandardFile = fileUploadResponse.OriginalFileName;
                        version.UploadReferenceID = fileUploadResponse.ID;
                    }
                }
            }

            await _TestMethodSpecificationRepository.AddTestMethodSpecification(model);
            _logger.LogInformation("TestMethodSpecification '{TestMethodSpecificationName}' created successfully.", model.Name);
        }

        public async Task ModifyTestMethodSpecification(TestMethodSpecification model)
        {
            if (model.ID == 0)
                throw new ArgumentException("TestMethodSpecification ID should not be empty!");

            bool exists = await _TestMethodSpecificationRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same TestMethodSpecification already exists!");

            var existingTestMethodSpecification = await _TestMethodSpecificationRepository.GetTestMethodSpecificationById(model.ID);
            if (existingTestMethodSpecification == null)
                throw new InvalidOperationException("TestMethodSpecification not found!");


            existingTestMethodSpecification.Name = model.Name;
            existingTestMethodSpecification.Part = model.Part;
            existingTestMethodSpecification.StandardOrganizationID = model.StandardOrganizationID;
            existingTestMethodSpecification.TestMethodStandard = model.TestMethodStandard;
            existingTestMethodSpecification.IsDisabled = model.IsDisabled;
            existingTestMethodSpecification.ModifiedOn = DateTime.UtcNow;
            existingTestMethodSpecification.ModifiedBy = loggedInUser.EmployeeID;

            
            var updatedVersionIds = model.Versions.Select(v => v.ID).ToHashSet();

            
            var toRemove = existingTestMethodSpecification.Versions.Where(v => !updatedVersionIds.Contains(v.ID)).ToList();
            foreach (var item in toRemove)
            {
                existingTestMethodSpecification.Versions.Remove(item);
            }

            foreach (var versionModel in model.Versions)
            {
                if (versionModel.file != null)
                {
                    var fileUploadResponse = await _uploadService.UploadFileAsync(versionModel.file, FileType.Other, null, versionModel.StandardFile);
                    if (fileUploadResponse == null)
                        throw new InvalidOperationException("File upload failed!");
                    versionModel.StandardFilePath = fileUploadResponse.FilePath;
                    versionModel.StandardFile = fileUploadResponse.OriginalFileName;
                    versionModel.UploadReferenceID = fileUploadResponse.ID;
                }
                var existingVersion = existingTestMethodSpecification.Versions.FirstOrDefault(v => v.ID == versionModel.ID);

                if (existingVersion != null)
                {
                    existingVersion.Version = versionModel.Version;
                    existingVersion.StandardFile = versionModel.StandardFile;
                    existingVersion.StandardFilePath = versionModel.StandardFilePath;
                    existingVersion.Default = versionModel.Default;
                    existingVersion.UploadReferenceID = versionModel.UploadReferenceID;
                }
                else
                {
                    existingTestMethodSpecification.Versions.Add(new TestMethodSpecificationVersion
                    {
                        Version = versionModel.Version,
                        StandardFile = versionModel.StandardFile,
                        StandardFilePath = versionModel.StandardFilePath,
                        Default = versionModel.Default,
                        UploadReferenceID = versionModel.UploadReferenceID
                    });
                }
            }

            await _TestMethodSpecificationRepository.UpdateTestMethodSpecification(existingTestMethodSpecification);
            _logger.LogInformation("TestMethodSpecification '{TestMethodSpecificationName}' updated successfully.", model.Name);
        }

        public async Task RemoveTestMethodSpecification(long id)
        {
            var existingTestMethodSpecification = await _TestMethodSpecificationRepository.GetTestMethodSpecificationById(id);
            if (existingTestMethodSpecification == null)
                throw new InvalidOperationException("TestMethodSpecification not found!");

            existingTestMethodSpecification.IsActive = false;
            existingTestMethodSpecification.ModifiedOn = DateTime.UtcNow;
            existingTestMethodSpecification.ModifiedBy = loggedInUser.EmployeeID;

            await _TestMethodSpecificationRepository.UpdateTestMethodSpecification(existingTestMethodSpecification);
            _logger.LogInformation("TestMethodSpecification with ID '{TestMethodSpecificationId}' deleted successfully.", id);
        }
        public async Task EnableDisableTestMethodSpecification(long id)
        {
            var existingTestMethodSpecification = await _TestMethodSpecificationRepository.GetTestMethodSpecificationById(id);
            if (existingTestMethodSpecification == null)
                throw new InvalidOperationException("TestMethodSpecification not found!");

            existingTestMethodSpecification.IsDisabled = !existingTestMethodSpecification.IsDisabled;
            existingTestMethodSpecification.ModifiedOn = DateTime.UtcNow;
            existingTestMethodSpecification.ModifiedBy = loggedInUser.EmployeeID;

            await _TestMethodSpecificationRepository.UpdateTestMethodSpecification(existingTestMethodSpecification);
            _logger.LogInformation("TestMethodSpecification with ID '{TestMethodSpecificationId}' deleted successfully.", id);
        }

        public async Task<TestMethodSpecification> GetTestMethodSpecificationDetails(long id)
        {
            var classification = await _TestMethodSpecificationRepository.GetTestMethodSpecificationById(id);
            if (classification == null)
                throw new InvalidOperationException("TestMethodSpecification not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchTestMethodSpecificationList(PageFilter filter)
        {
            return await _TestMethodSpecificationRepository.GetAllTestMethodSpecifications(filter);
        }

        public async Task<List<DropdwonSelector>> GetTestMethodSpecificationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _TestMethodSpecificationRepository.GetTestMethodSpecificationDropdown(searchTerm, pageNo, pageSize);
        }

    }
}
