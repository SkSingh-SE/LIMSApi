using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILogger<SupplierService> _logger;
        private readonly IFileUploadService _uploadService;
        private readonly LIMSContext _context;

        public SupplierService(ISupplierRepository supplierRepo, ILogger<SupplierService> logger, IFileUploadService fileUploadService, LIMSContext context)
        {
            _supplierRepository = supplierRepo;
            _logger = logger;
            _uploadService = fileUploadService;
            _context = context;
        }

        public async Task CreateSupplier(SupplierMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Supplier name should not be empty!");
            model.Name = model.Name.Trim();
            if (string.IsNullOrWhiteSpace(model.ContactPerson1))
                throw new ArgumentException("Primary contact person name is required.");
            if (string.IsNullOrWhiteSpace(model.ContactNo1))
                throw new ArgumentException("Primary contact number is required.");
            var _phoneRegex = new System.Text.RegularExpressions.Regex(@"^[+]?\d{10,13}$");
            if (!_phoneRegex.IsMatch(model.ContactNo1.Trim()))
                throw new ArgumentException("Invalid primary contact number format. Must be 10-13 digits.");
            if (model.IsBlacklisted && string.IsNullOrWhiteSpace(model.ReasonForBlacklisting))
                throw new ArgumentException("Reason for blacklisting is required when supplier is blacklisted.");
            if (model.IsBlacklisted && !model.BlacklistDate.HasValue)
                throw new ArgumentException("Blacklist date is required when supplier is blacklisted.");
            if (model.SupplierApproved && model.IsBlacklisted)
                throw new ArgumentException("A supplier cannot be both approved and blacklisted.");

            bool exists = await _supplierRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Supplier already exists!");

            if(model.file != null)
            {
                var fileUploadResponse = await _uploadService.UploadFileAsync(model.file, FileType.Other, null, model.FileName);
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");
                model.AgreementFilePath = fileUploadResponse.FilePath;
                model.FileName = fileUploadResponse.OriginalFileName;
                model.UploadReferenceID = fileUploadResponse.ID;
            }
            await _supplierRepository.AddSupplier(model);
            _logger.LogInformation("Supplier '{SupplierName}' created successfully.", model.Name);
        }

        public async Task ModifySupplier(SupplierMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Supplier ID should not be empty!");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Supplier name should not be empty!");
            model.Name = model.Name.Trim();
            if (string.IsNullOrWhiteSpace(model.ContactPerson1))
                throw new ArgumentException("Primary contact person name is required.");
            if (string.IsNullOrWhiteSpace(model.ContactNo1))
                throw new ArgumentException("Primary contact number is required.");
            var _phoneRegex = new System.Text.RegularExpressions.Regex(@"^[+]?\d{10,13}$");
            if (!_phoneRegex.IsMatch(model.ContactNo1.Trim()))
                throw new ArgumentException("Invalid primary contact number format. Must be 10-13 digits.");
            if (model.IsBlacklisted && string.IsNullOrWhiteSpace(model.ReasonForBlacklisting))
                throw new ArgumentException("Reason for blacklisting is required when supplier is blacklisted.");
            if (model.IsBlacklisted && !model.BlacklistDate.HasValue)
                throw new ArgumentException("Blacklist date is required when supplier is blacklisted.");
            if (model.SupplierApproved && model.IsBlacklisted)
                throw new ArgumentException("A supplier cannot be both approved and blacklisted.");

            bool exists = await _supplierRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Supplier already exists!");

            var existingSupplier = await _supplierRepository.GetSupplierById(model.ID);
            if (existingSupplier == null)
                throw new InvalidOperationException("Supplier not found!");

            existingSupplier.Name = model.Name;
            existingSupplier.PresentStatus = model.PresentStatus;
            existingSupplier.ContactPerson1 = model.ContactPerson1;
            existingSupplier.ContactPerson2 = model.ContactPerson2;
            existingSupplier.ContactPerson3 = model.ContactPerson3;
            existingSupplier.ContactNo1 = model.ContactNo1;
            existingSupplier.ContactNo2 = model.ContactNo2;
            existingSupplier.ContactNo3 = model.ContactNo3;
            existingSupplier.EmailId1 = model.EmailId1;
            existingSupplier.EmailId2 = model.EmailId2;
            existingSupplier.EmailId3 = model.EmailId3;
            existingSupplier.ProductType = model.ProductType;
            existingSupplier.Address = model.Address;
            existingSupplier.IsBlacklisted = model.IsBlacklisted;
            existingSupplier.ReasonForBlacklisting = model.ReasonForBlacklisting;
            existingSupplier.BlacklistDate = model.BlacklistDate;
            existingSupplier.BlacklistedBy = model.BlacklistedBy;

            existingSupplier.ModifiedOn = DateTime.UtcNow;

            if(model.file != null)
            {
                var fileUploadResponse = await _uploadService.UploadFileAsync(model.file, FileType.Other, null, model.FileName);
                if (fileUploadResponse == null)
                    throw new InvalidOperationException("File upload failed!");
                existingSupplier.AgreementFilePath = fileUploadResponse.FilePath;
                existingSupplier.FileName = fileUploadResponse.OriginalFileName;
                existingSupplier.UploadReferenceID = fileUploadResponse.ID;
            }
            else if(string.IsNullOrEmpty(model.FileName) && existingSupplier.UploadReferenceID != null)
            {
                await _uploadService.RemoveFileAsync((long)existingSupplier.UploadReferenceID);
            }
            await _supplierRepository.UpdateSupplier(existingSupplier);
            _logger.LogInformation("Supplier '{SupplierName}' updated successfully.", model.Name);
        }

        public async Task RemoveSupplier(long id)
        {
            var existingSupplier = await _supplierRepository.GetSupplierById(id);
            if (existingSupplier == null)
                throw new InvalidOperationException("Supplier not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<SupplierMaster>(_context, id, "Supplier");

            existingSupplier.IsActive = false;
            existingSupplier.ModifiedOn = DateTime.UtcNow;

            if (existingSupplier.UploadReferenceID != null)
            {
                await _uploadService.RemoveFileAsync((long)existingSupplier.UploadReferenceID);
                existingSupplier.AgreementFilePath = null;
                existingSupplier.FileName = null;
                existingSupplier.UploadReferenceID = null;
            }
            await _supplierRepository.UpdateSupplier(existingSupplier);
            _logger.LogInformation("Supplier with ID '{SupplierId}' deleted successfully.", id);
        }

        public async Task<SupplierMaster> GetSupplierDetails(long id)
        {
            var classification = await _supplierRepository.GetSupplierById(id);
            if (classification == null)
                throw new InvalidOperationException("Supplier not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchSupplierList(PageFilter filter)
        {
            return await _supplierRepository.GetAllSuppliers(filter);
        }

        public async Task<List<DropdwonSelector>> GetSupplierDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _supplierRepository.GetSupplierDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
