using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class CustomerTypeService : ICustomerTypeService
    {
        private readonly ICustomerTypeRepository _customerTypeRepository;
        private readonly ILogger<CustomerTypeService> _logger;
        private LoggedInUserDTO loggedInUser;

        public CustomerTypeService(ICustomerTypeRepository CustomerTypeRepo, ILogger<CustomerTypeService> logger)
        {
            _customerTypeRepository = CustomerTypeRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateCustomerType(CustomerTypeMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("CustomerType name should not be empty!");

            bool exists = await _customerTypeRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("CustomerType already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _customerTypeRepository.AddCustomerType(model);
            _logger.LogInformation("CustomerType '{CustomerTypeName}' created successfully.", model.Name);
        }

        public async Task ModifyCustomerType(CustomerTypeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("CustomerType ID should not be empty!");

            bool exists = await _customerTypeRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same CustomerType already exists!");

            var existingCustomerType = await _customerTypeRepository.GetCustomerTypeById(model.ID);
            if (existingCustomerType == null)
                throw new InvalidOperationException("CustomerType not found!");


            existingCustomerType.Name = model.Name;
            existingCustomerType.Description = model.Description;
            existingCustomerType.ModifiedOn = DateTime.UtcNow;
            existingCustomerType.ModifiedBy = loggedInUser.EmployeeID;

            await _customerTypeRepository.UpdateCustomerType(existingCustomerType);
            _logger.LogInformation("CustomerType '{CustomerTypeName}' updated successfully.", model.Name);
        }

        public async Task RemoveCustomerType(long id)
        {
            var existingCustomerType = await _customerTypeRepository.GetCustomerTypeById(id);
            if (existingCustomerType == null)
                throw new InvalidOperationException("CustomerType not found!");

            existingCustomerType.IsActive = false;
            existingCustomerType.ModifiedOn = DateTime.UtcNow;
            existingCustomerType.ModifiedBy = loggedInUser.EmployeeID;

            await _customerTypeRepository.DeleteCustomerType(existingCustomerType);
            _logger.LogInformation("CustomerType with ID '{CustomerTypeId}' deleted successfully.", id);
        }

        public async Task<CustomerTypeMaster> GetCustomerTypeDetails(long id)
        {
            var classification = await _customerTypeRepository.GetCustomerTypeById(id);
            if (classification == null)
                throw new InvalidOperationException("CustomerType not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchCustomerTypeList(PageFilter filter)
        {
            return await _customerTypeRepository.GetAllCustomerTypes(filter);
        }

        public async Task<List<DropdwonSelector>> GetCustomerTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _customerTypeRepository.GetCustomerTypeDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
