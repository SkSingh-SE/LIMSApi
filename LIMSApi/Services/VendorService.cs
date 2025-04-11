using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ILogger<VendorService> _logger;

        public VendorService(IVendorRepository vendorRepo, ILogger<VendorService> logger)
        {
            _vendorRepository = vendorRepo;
            _logger = logger;
        }

        public async Task CreateVendor(VendorMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Vendor name should not be empty!");

            bool exists = await _vendorRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Vendor already exists!");
            await _vendorRepository.AddVendor(model);
            _logger.LogInformation("Vendor '{VendorName}' created successfully.", model.Name);
        }

        public async Task ModifyVendor(VendorMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Vendor ID should not be empty!");

            bool exists = await _vendorRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Vendor already exists!");

            var existingVendor = await _vendorRepository.GetVendorById(model.ID);
            if (existingVendor == null)
                throw new InvalidOperationException("Vendor not found!");

            existingVendor.Name = model.Name;
            existingVendor.Code = model.Code;
            existingVendor.TellyLedgerName = model.TellyLedgerName;
            existingVendor.GSTNo = model.GSTNo;
            existingVendor.PANNo = model.PANNo;
            existingVendor.ContactPersonName = model.ContactPersonName;
            existingVendor.MobileNo = model.MobileNo;
            existingVendor.EmailID = model.EmailID;
            existingVendor.Address = model.Address;
            existingVendor.ModifiedOn = DateTime.UtcNow;

            await _vendorRepository.UpdateVendor(existingVendor);
            _logger.LogInformation("Vendor '{VendorName}' updated successfully.", model.Name);
        }

        public async Task RemoveVendor(long id)
        {
            var existingVendor = await _vendorRepository.GetVendorById(id);
            if (existingVendor == null)
                throw new InvalidOperationException("Vendor not found!");

            existingVendor.IsActive = false;
            existingVendor.ModifiedOn = DateTime.UtcNow;

            await _vendorRepository.UpdateVendor(existingVendor);
            _logger.LogInformation("Vendor with ID '{VendorId}' deleted successfully.", id);
        }

        public async Task<VendorMaster> GetVendorDetails(long id)
        {
            var classification = await _vendorRepository.GetVendorById(id);
            if (classification == null)
                throw new InvalidOperationException("Vendor not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchVendorList(PageFilter filter)
        {
            return await _vendorRepository.GetAllVendors(filter);
        }

        public async Task<List<DropdwonSelector>> GetVendorDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _vendorRepository.GetVendorDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
