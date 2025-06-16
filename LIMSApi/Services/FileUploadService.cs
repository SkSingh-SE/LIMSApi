using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LIMSApi.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IFileUploadRepository _fileUploadRepository;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string _baseUploadDirectory;
        private readonly LoggedInUserDTO _loggedInUser;
        private readonly IWebHostEnvironment _env;
        public FileUploadService(IFileUploadRepository fileUploadRepo, ILogger<FileUploadService> logger, IConfiguration configuration,IWebHostEnvironment env)
        {
            _fileUploadRepository = fileUploadRepo;
            _logger = logger;
            _env = env;
            _baseUploadDirectory = configuration["FileUploadSettings:UploadDirectory"] ?? "/Uploads";
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }
        public async Task<UploadFile> GetFileAsync(long id)
        {
            var uploadedFile = await _fileUploadRepository.GetUploadFile(id);
            _logger.LogInformation("Fetching Uploaded file : '{FileName}'.", uploadedFile.OriginalFileName);
            if (uploadedFile == null)
                throw new InvalidOperationException("File not found!");
            return uploadedFile;
        }

        public async Task<UploadFile> UploadFileAsync(IFormFile file, FileType fileType, int? year, string? identifier = "")
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File cannot be empty");

                string originalFileName = Path.GetFileName(file.FileName);
                string originalFileExtension = Path.GetExtension(file.FileName);
                string newFileName = $"{Guid.NewGuid()}_{fileType}{(string.IsNullOrWhiteSpace(identifier) ? "" : $"_{identifier}")}";

                string relativeDirectory = getFileTypePath(fileType, year);
                string uploadDirectory = Path.Combine(_env.WebRootPath, relativeDirectory);

                if (!Directory.Exists(uploadDirectory))
                    Directory.CreateDirectory(uploadDirectory);

                string filePath = Path.Combine(uploadDirectory, newFileName);
                string relativeFilePath = Path.Combine(relativeDirectory, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var uploadedFile = new UploadFile
                {
                    OriginalFileName = originalFileName,
                    StoredFileName = newFileName,
                    FileType = fileType,
                    FileExtension = originalFileExtension,
                    FilePath = relativeFilePath,
                    FileSize = file.Length,
                    Year = year ?? DateTime.UtcNow.Year,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = _loggedInUser?.EmployeeID ?? 0,
                    CompanyCode = _loggedInUser?.CompanyCode ?? "NA"
                };

                var uploaded = await _fileUploadRepository.UploadFile(uploadedFile);
                return uploaded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while uploading file");
                throw;
            }
        }
        public string getFileTypePath(FileType fileType, int? year)
        {
            string uploadDirectory = string.Empty;
            if (fileType == FileType.Employee)
            {
                uploadDirectory = Path.Combine(_baseUploadDirectory, "Employee");
            }
            else if (fileType == FileType.Customer)
            {
                uploadDirectory = Path.Combine(_baseUploadDirectory, "Customer");
            }
            else if (fileType == FileType.Test)
            {
                uploadDirectory = Path.Combine(_baseUploadDirectory, "Test");
                if (year.HasValue)
                {
                    uploadDirectory = Path.Combine(uploadDirectory, year.ToString());
                }
            }
            else if (fileType == FileType.Material)
            {
                uploadDirectory = Path.Combine(_baseUploadDirectory, "Material");
                if (year.HasValue)
                {
                    uploadDirectory = Path.Combine(uploadDirectory, year.ToString());
                }
            }
            else if (fileType == FileType.Product)
            {
                uploadDirectory = Path.Combine(_baseUploadDirectory, "Product");
                if (year.HasValue)
                {
                    uploadDirectory = Path.Combine(uploadDirectory, year.ToString());
                }
            }
            else if (fileType == FileType.Sample)
            {
                uploadDirectory = Path.Combine(_baseUploadDirectory, "Sample");
                if (year.HasValue)
                {
                    uploadDirectory = Path.Combine(uploadDirectory, year.ToString());
                }
            }
            else
            {
                uploadDirectory = Path.Combine(_baseUploadDirectory, "Other");
            }
            return uploadDirectory;
        }

        public async Task RemoveFileAsync(long Id)
        {
            var existingFile = await _fileUploadRepository.GetUploadFile(Id);
            if(existingFile != null)
            {
                if (File.Exists(existingFile.FilePath))
                {
                    File.Delete(existingFile.FilePath);
                    await _fileUploadRepository.RemoveFile(existingFile);
                }
            }
        }
    }
}
