using System.Text.RegularExpressions;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class ClassificationService : IClassificationService
    {
        private readonly IClassificationRepository _repository;
        private readonly LIMSContext _context;
        private readonly ILogger<ClassificationService> _logger;
        private LoggedInUserDTO loggedInUser;

        public ClassificationService(IClassificationRepository repository, LIMSContext context, ILogger<ClassificationService> logger)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        private static string NormalizeCode(string code)
        {
            var normalized = code.Trim().ToUpperInvariant();
            normalized = Regex.Replace(normalized, @"\s+", "_");
            return normalized;
        }

        public async Task CreateClassification(ClassificationMasterCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Classification code should not be empty!");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Classification name should not be empty!");

            var code = NormalizeCode(dto.Code);
            if (!Regex.IsMatch(code, @"^[A-Z0-9_]+$"))
                throw new ArgumentException("Classification code may contain only letters, numbers and underscore (_).");

            if (await _repository.ExistsByCode(code))
                throw new InvalidOperationException($"Classification code '{code}' already exists!");

            if (await _repository.ExistsByName(dto.Name.Trim()))
                throw new InvalidOperationException("Classification name already exists!");

            var model = new ClassificationMaster
            {
                Code = code,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                DisplayOrder = dto.DisplayOrder,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = loggedInUser.EmployeeID,
                CompanyCode = loggedInUser.CompanyCode,
                IsActive = true
            };

            await _repository.AddClassification(model);
            _logger.LogInformation("Classification '{Name}' created successfully.", model.Name);
        }

        public async Task ModifyClassification(ClassificationMasterUpdateDto dto)
        {
            if (dto.ID == 0)
                throw new ArgumentException("Classification ID should not be empty!");

            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Classification code should not be empty!");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Classification name should not be empty!");

            var code = NormalizeCode(dto.Code);
            if (!Regex.IsMatch(code, @"^[A-Z0-9_]+$"))
                throw new ArgumentException("Classification code may contain only letters, numbers and underscore (_).");

            if (await _repository.ExistsByCodeAndNotId(code, dto.ID))
                throw new InvalidOperationException($"Classification code '{code}' already exists!");

            if (await _repository.ExistsByNameAndNotId(dto.Name.Trim(), dto.ID))
                throw new InvalidOperationException("Classification name already exists!");

            var existing = await _context.ClassificationMasters
                .FirstOrDefaultAsync(x => x.ID == dto.ID && x.CompanyCode == loggedInUser.CompanyCode);

            if (existing == null)
                throw new InvalidOperationException("Classification not found!");

            existing.Code = code;
            existing.Name = dto.Name.Trim();
            existing.Description = dto.Description?.Trim();
            existing.DisplayOrder = dto.DisplayOrder;
            existing.IsActive = dto.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateClassification(existing);
            _logger.LogInformation("Classification '{Name}' updated successfully.", existing.Name);
        }

        public async Task RemoveClassification(long id)
        {
            var existing = await _context.ClassificationMasters
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);

            if (existing == null)
                throw new InvalidOperationException("Classification not found!");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateClassification(existing);
            _logger.LogInformation("Classification with ID '{Id}' deactivated successfully.", id);
        }

        public async Task<bool> ToggleClassificationStatus(long id)
        {
            var existing = await _context.ClassificationMasters
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);

            if (existing == null)
                throw new InvalidOperationException("Classification not found!");

            if (!existing.IsActive && !string.IsNullOrWhiteSpace(existing.Code))
            {
                if (await _repository.ExistsByCodeAndNotId(existing.Code.Trim(), existing.ID))
                    throw new InvalidOperationException($"Cannot activate: Classification code '{existing.Code.Trim()}' is already in use by another active classification.");
            }

            existing.IsActive = !existing.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateClassification(existing);
            _logger.LogInformation("Classification '{Name}' status toggled to {Status}.", existing.Name, existing.IsActive ? "Active" : "Inactive");
            return existing.IsActive;
        }

        public async Task<ClassificationMasterDto> GetClassificationDetails(long id)
        {
            var entity = await _repository.GetClassificationById(id);
            if (entity == null)
                throw new InvalidOperationException("Classification not found!");

            return new ClassificationMasterDto
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                DisplayOrder = entity.DisplayOrder,
                IsActive = entity.IsActive,
                CompanyCode = entity.CompanyCode
            };
        }

        public async Task<PagedResponse<object>> FetchClassificationList(PageFilter filter)
        {
            return await _repository.GetAllClassifications(filter);
        }

        public async Task<List<DropdwonSelector>> GetClassificationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetClassificationDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
