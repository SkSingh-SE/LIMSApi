using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class CourierService : ICourierService
    {
        private readonly ICourierRepository _courierRepository;
        private readonly ILogger<CourierService> _logger;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public CourierService(ICourierRepository CourierRepo, ILogger<CourierService> logger, LIMSContext context)
        {
            _courierRepository = CourierRepo;
            _logger = logger;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateCourier(CourierMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Courier name should not be empty!");
            model.Name = model.Name.Trim();
            if (!string.IsNullOrWhiteSpace(model.ContactNo))
            {
                var phoneRegex = new System.Text.RegularExpressions.Regex(@"^[+]?\d{10,13}$");
                if (!phoneRegex.IsMatch(model.ContactNo.Trim()))
                    throw new ArgumentException("Invalid contact number. Must be 10-13 digits.");
            }

            bool exists = await _courierRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Courier already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _courierRepository.AddCourier(model);
            _logger.LogInformation("Courier '{CourierName}' created successfully.", model.Name);
        }

        public async Task ModifyCourier(CourierMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Courier ID should not be empty!");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Courier name should not be empty!");
            model.Name = model.Name.Trim();
            if (!string.IsNullOrWhiteSpace(model.ContactNo))
            {
                var phoneRegex = new System.Text.RegularExpressions.Regex(@"^[+]?\d{10,13}$");
                if (!phoneRegex.IsMatch(model.ContactNo.Trim()))
                    throw new ArgumentException("Invalid contact number. Must be 10-13 digits.");
            }

            bool exists = await _courierRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Courier already exists!");

            var existingCourier = await _courierRepository.GetCourierById(model.ID);
            if (existingCourier == null)
                throw new InvalidOperationException("Courier not found!");


            existingCourier.Name = model.Name;
            existingCourier.ContactNo = model.ContactNo;
            existingCourier.ModifiedOn = DateTime.UtcNow;
            existingCourier.ModifiedBy = loggedInUser.EmployeeID;

            await _courierRepository.UpdateCourier(existingCourier);
            _logger.LogInformation("Courier '{CourierName}' updated successfully.", model.Name);
        }

        public async Task RemoveCourier(long id)
        {
            var existingCourier = await _courierRepository.GetCourierById(id);
            if (existingCourier == null)
                throw new InvalidOperationException("Courier not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<CourierMaster>(_context, id, "Courier");

            existingCourier.IsActive = false;
            existingCourier.ModifiedOn = DateTime.UtcNow;
            existingCourier.ModifiedBy = loggedInUser.EmployeeID;

            await _courierRepository.DeleteCourier(existingCourier);
            _logger.LogInformation("Courier with ID '{CourierId}' deleted successfully.", id);
        }

        public async Task<CourierMaster> GetCourierDetails(long id)
        {
            var classification = await _courierRepository.GetCourierById(id);
            if (classification == null)
                throw new InvalidOperationException("Courier not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchCourierList(PageFilter filter)
        {
            return await _courierRepository.GetAllCouriers(filter);
        }

        public async Task<List<DropdwonSelector>> GetCourierDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _courierRepository.GetCourierDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
