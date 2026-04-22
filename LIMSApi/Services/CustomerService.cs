using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerService> _logger;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public CustomerService(ICustomerRepository customerRepo, ILogger<CustomerService> logger, LIMSContext context)
        {
            _customerRepository = customerRepo;
            _logger = logger;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateCustomer(Customer model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Customer name should not be empty!");
            if (string.IsNullOrWhiteSpace(model.Address))
                throw new ArgumentException("Customer address should not be empty!");
            if (model.CityID == 0)
                throw new ArgumentException("City is required.");
            if (model.StateID == 0)
                throw new ArgumentException("State is required.");
            if (model.CountryID == 0)
                throw new ArgumentException("Country is required.");
            if (string.IsNullOrWhiteSpace(model.CustomerType))
                throw new ArgumentException("Customer type is required.");
            if (string.IsNullOrWhiteSpace(model.TallyLedgerName))
                throw new ArgumentException("Tally ledger name is required.");
            if (!model.GSTNA && string.IsNullOrWhiteSpace(model.GSTNo))
                throw new ArgumentException("GST number is required (or mark as Not Applicable).");
            if (!model.GSTNA && !string.IsNullOrWhiteSpace(model.GSTNo))
            {
                var gstRegex = new System.Text.RegularExpressions.Regex(@"^\d{2}[A-Z]{5}\d{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$");
                if (!gstRegex.IsMatch(model.GSTNo.Trim().ToUpper()))
                    throw new ArgumentException("Invalid GSTIN format (e.g. 22AAAAA0000A1Z5).");
            }
            if (!string.IsNullOrWhiteSpace(model.PinCode))
            {
                var pinRegex = new System.Text.RegularExpressions.Regex(@"^\d{6}$");
                if (!pinRegex.IsMatch(model.PinCode.Trim()))
                    throw new ArgumentException("PIN code must be exactly 6 digits.");
            }
            model.Name = model.Name.Trim();

            bool exists = await _customerRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Customer already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            if (model.ContactPersons != null)
            {
                foreach (var contact in model.ContactPersons)
                {
                    contact.CustomerID = model.ID;
                }
            }
            if (model.CustomerCompanyCategories != null)
            {
                foreach (var category in model.CustomerCompanyCategories)
                {
                    category.CustomerID = model.ID;
                }
            }
            if(model.CustomerDispatchModes != null && model.CustomerDispatchModes.Any())
            {
                foreach (var dispatchMode in model.CustomerDispatchModes)
                {
                    dispatchMode.CustomerID = model.ID;
                }
            }
            await _customerRepository.AddCustomer(model);
            _logger.LogInformation("Customer '{CustomerName}' created successfully.", model.Name);
        }

        public async Task ModifyCustomer(Customer model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Customer ID should not be empty!");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Customer name should not be empty!");
            if (string.IsNullOrWhiteSpace(model.Address))
                throw new ArgumentException("Customer address should not be empty!");
            if (model.CityID == 0)
                throw new ArgumentException("City is required.");
            if (model.StateID == 0)
                throw new ArgumentException("State is required.");
            if (model.CountryID == 0)
                throw new ArgumentException("Country is required.");
            if (string.IsNullOrWhiteSpace(model.CustomerType))
                throw new ArgumentException("Customer type is required.");
            if (string.IsNullOrWhiteSpace(model.TallyLedgerName))
                throw new ArgumentException("Tally ledger name is required.");
            if (!model.GSTNA && string.IsNullOrWhiteSpace(model.GSTNo))
                throw new ArgumentException("GST number is required (or mark as Not Applicable).");
            if (!model.GSTNA && !string.IsNullOrWhiteSpace(model.GSTNo))
            {
                var gstRegex = new System.Text.RegularExpressions.Regex(@"^\d{2}[A-Z]{5}\d{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$");
                if (!gstRegex.IsMatch(model.GSTNo.Trim().ToUpper()))
                    throw new ArgumentException("Invalid GSTIN format (e.g. 22AAAAA0000A1Z5).");
            }
            if (!string.IsNullOrWhiteSpace(model.PinCode))
            {
                var pinRegex = new System.Text.RegularExpressions.Regex(@"^\d{6}$");
                if (!pinRegex.IsMatch(model.PinCode.Trim()))
                    throw new ArgumentException("PIN code must be exactly 6 digits.");
            }
            model.Name = model.Name.Trim();

            bool exists = await _customerRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Customer already exists!");

            if (!string.IsNullOrWhiteSpace(model.GSTNo))
            {
                bool duplicateCustomer = await _customerRepository.ValidateDuplicateCustomer(model.GSTNo, model.ID);
                if(duplicateCustomer)
                    throw new InvalidOperationException("Same Customer GST already exists!");
            }

            var existingCustomer = await _customerRepository.GetCustomerById(model.ID);
            if (existingCustomer == null)
                throw new InvalidOperationException("Customer not found!");


            existingCustomer.Name = model.Name;
            existingCustomer.LegalName = model.LegalName;
            existingCustomer.TallyLedgerName = model.TallyLedgerName;
            existingCustomer.Address = model.Address;
            existingCustomer.PinCode = model.PinCode;
            existingCustomer.AreaID = model.AreaID;
            existingCustomer.CityID = model.CityID;
            existingCustomer.StateID = model.StateID;
            existingCustomer.CountryID = model.CountryID;
            existingCustomer.CurrencyID = model.CurrencyID;
            existingCustomer.CustomerType = model.CustomerType;
            existingCustomer.IsBlock = model.IsBlock;
            existingCustomer.BlockReason = model.BlockReason;
            existingCustomer.GSTNo = model.GSTNo;
            existingCustomer.PANNo = model.PANNo;
            existingCustomer.GSTNA = model.GSTNA;
            existingCustomer.SampleReturn = model.SampleReturn;
            existingCustomer.BillingEvery = model.BillingEvery;
            existingCustomer.BillingEveryDays = model.BillingEveryDays;
            existingCustomer.SpecialAccountingCase = model.SpecialAccountingCase;
            existingCustomer.WeeklyBillingCustomer = model.WeeklyBillingCustomer;
            existingCustomer.MonthlyBillingCustomer = model.MonthlyBillingCustomer;
            existingCustomer.DirectTaxInvoiceNoPerforma = model.DirectTaxInvoiceNoPerforma;
            existingCustomer.PerformaInvoiceRequiredBeforeTesting = model.PerformaInvoiceRequiredBeforeTesting;
            existingCustomer.ConstantDiscount = model.ConstantDiscount;
            existingCustomer.ConstantDiscountPercentage = model.ConstantDiscountPercentage;
            existingCustomer.CreditLimitAmount = model.CreditLimitAmount;
            existingCustomer.CreditLimitTime = model.CreditLimitTime;
            existingCustomer.IsVerified = model.IsVerified;
            existingCustomer.Remark = model.Remark;
            existingCustomer.ModifiedOn = DateTime.UtcNow;
            existingCustomer.ModifiedBy = loggedInUser.EmployeeID;

            // remove unwanted mappings
            if (existingCustomer.ContactPersons.Any())
            {
                var contactsToRemove = existingCustomer.ContactPersons.Where(contact => !model.ContactPersons.Any(m => m.ID == contact.ID)).ToList();
                foreach (var contact in contactsToRemove)
                {
                    existingCustomer.ContactPersons.Remove(contact);
                }
            }

            if (model.ContactPersons != null && model.ContactPersons.Any())
            {
                // Add or update mappings
                foreach (var contactPerson in model.ContactPersons)
                {
                    contactPerson.CustomerID = model.ID;

                    var existingContactPerson = existingCustomer.ContactPersons
                        .FirstOrDefault(m => m.ID == contactPerson.ID);

                    if (existingContactPerson != null)
                    {
                        existingContactPerson.CustomerID = model.ID;
                        existingContactPerson.Salutation = contactPerson.Salutation;
                        existingContactPerson.Name = contactPerson.Name;
                        existingContactPerson.Department = contactPerson.Department;
                        existingContactPerson.EmailId = contactPerson.EmailId;
                        existingContactPerson.MobileNo = contactPerson.MobileNo;
                        existingContactPerson.IsWhatsappNo = contactPerson.IsWhatsappNo;
                        existingContactPerson.TelephoneNo = contactPerson.TelephoneNo;
                        existingContactPerson.SendBill = contactPerson.SendBill;
                        existingContactPerson.SendReport = contactPerson.SendReport;
                        existingContactPerson.Address = contactPerson.Address;
                        existingContactPerson.AreaID = contactPerson.AreaID;
                        existingContactPerson.City = contactPerson.City;
                        existingContactPerson.State = contactPerson.State;
                        existingContactPerson.Country = contactPerson.Country;
                        existingContactPerson.PinCode = contactPerson.PinCode;

                        existingContactPerson.Type = contactPerson.Type;
                    }
                    else
                    {
                        existingCustomer.ContactPersons.Add(contactPerson);
                    }
                }
            }
            // --- CompanyCategory  ---
            if (existingCustomer.CustomerCompanyCategories != null)
            {
                var categoriesToRemove = existingCustomer.CustomerCompanyCategories
                    .Where(existing => !model.CustomerCompanyCategories.Any(m => m.CompanyCategoryID == existing.CompanyCategoryID))
                    .ToList();

                foreach (var category in categoriesToRemove)
                {
                    existingCustomer.CustomerCompanyCategories.Remove(category);
                }
            }

            if (model.CustomerCompanyCategories != null && model.CustomerCompanyCategories.Any())
            {
                foreach (var category in model.CustomerCompanyCategories)
                {
                    category.CustomerID = model.ID;

                    var existingCategory = existingCustomer.CustomerCompanyCategories
                        .FirstOrDefault(c => c.CompanyCategoryID == category.CompanyCategoryID);

                    if (existingCategory == null)
                    {
                        existingCustomer.CustomerCompanyCategories.Add(category);
                    }
                    else
                    {
                        existingCategory.CompanyCategoryID = category.CompanyCategoryID;
                    }
                    
                }
            }

            if(existingCustomer.CustomerDispatchModes != null)
            {
                var dispatchModesToRemove = existingCustomer.CustomerDispatchModes
                    .Where(existing => !model.CustomerDispatchModes.Any(m => m.DispatchModeID == existing.DispatchModeID))
                    .ToList();
                foreach (var dispatchMode in dispatchModesToRemove)
                {
                    existingCustomer.CustomerDispatchModes.Remove(dispatchMode);
                }
            }
            if (model.CustomerDispatchModes != null && model.CustomerDispatchModes.Any())
            {
                foreach (var dispatchMode in model.CustomerDispatchModes)
                {
                    dispatchMode.CustomerID = model.ID;
                    var existingDispatchMode = existingCustomer.CustomerDispatchModes
                        .FirstOrDefault(c => c.DispatchModeID == dispatchMode.DispatchModeID);
                    if (existingDispatchMode == null)
                    {
                        existingCustomer.CustomerDispatchModes.Add(dispatchMode);
                    }
                    else
                    {
                        existingDispatchMode.DispatchModeID = dispatchMode.DispatchModeID;
                    }
                }
            }
            await _customerRepository.UpdateCustomer(existingCustomer);

            _logger.LogInformation("Customer '{CustomerName}' updated successfully.", model.Name);
        }

        public async Task RemoveCustomer(long id)
        {
            var existingCustomer = await _customerRepository.GetCustomerById(id);
            if (existingCustomer == null)
                throw new InvalidOperationException("Customer not found!");

            // Check FK dependencies — prevent delete if linked to inwards, invoices, etc.
            await DeleteValidationHelper.ValidateDeleteAsync<Customer>(_context, id, "Customer");

            existingCustomer.IsActive = false;
            existingCustomer.ModifiedOn = DateTime.UtcNow;
            existingCustomer.ModifiedBy = loggedInUser.EmployeeID;

            await _customerRepository.DeleteCustomer(existingCustomer);
            _logger.LogInformation("Customer with ID '{CustomerId}' deleted successfully.", id);
        }

        public async Task<Customer> GetCustomerDetails(long id)
        {
            var classification = await _customerRepository.GetCustomerById(id);
            if (classification == null)
                throw new InvalidOperationException("Customer not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchCustomerList(PageFilter filter)
        {
            return await _customerRepository.GetAllCustomers(filter);
        }

        public async Task<List<DropdwonSelector>> GetCustomerDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _customerRepository.GetCustomerDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task VerifyCustomer(long id, bool status)
        {
           var customer = await _customerRepository.GetCustomerById(id);
            if(customer == null)
                throw new InvalidOperationException("Customer not found!");
            customer.IsVerified = status;
            customer.VerifiedOn = DateTime.UtcNow;
            customer.VerifiedBy = loggedInUser.EmployeeID;
            await _customerRepository.UpdateCustomer(customer);

        }
    }
}
