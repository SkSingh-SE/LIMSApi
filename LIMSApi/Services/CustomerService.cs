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

            // G14 — duplicate GSTIN check on create
            if (!model.GSTNA && !string.IsNullOrWhiteSpace(model.GSTNo))
            {
                bool duplicateGst = await _customerRepository.ValidateDuplicateCustomer(model.GSTNo.Trim().ToUpper(), 0);
                if (duplicateGst)
                    throw new InvalidOperationException("A customer with this GSTIN already exists.");
            }

            // G11 — at least one company category required
            if (model.CustomerCompanyCategories == null || !model.CustomerCompanyCategories.Any())
                throw new ArgumentException("At least one company category is required.");

            // G10 — at least one dispatch mode required
            if (model.CustomerDispatchModes == null || !model.CustomerDispatchModes.Any())
                throw new ArgumentException("At least one dispatch mode is required.");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            if (model.ContactPersons != null)
            {
                var contact1 = model.ContactPersons.FirstOrDefault(c => c.Type == "contact1");
                if (contact1 == null || string.IsNullOrWhiteSpace(contact1.Name))
                    throw new ArgumentException("Contact Person 1 name is required.");

                // Remove empty optional contacts before saving
                model.ContactPersons = model.ContactPersons
                    .Where(c => c.Type == "contact1" || !string.IsNullOrWhiteSpace(c.Name))
                    .ToList();

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

            // G14 — duplicate GSTIN check: exclude the customer itself (x.ID != model.ID)
            if (!model.GSTNA && !string.IsNullOrWhiteSpace(model.GSTNo))
            {
                var normalizedGst = model.GSTNo.Trim().ToUpper();
                bool duplicateCustomer = await _customerRepository.ValidateDuplicateCustomer(normalizedGst, model.ID);
                if (duplicateCustomer)
                    throw new InvalidOperationException("Another customer with this GSTIN already exists.");
            }

            // G11 — at least one company category required
            if (model.CustomerCompanyCategories == null || !model.CustomerCompanyCategories.Any())
                throw new ArgumentException("At least one company category is required.");

            // G10 — at least one dispatch mode required
            if (model.CustomerDispatchModes == null || !model.CustomerDispatchModes.Any())
                throw new ArgumentException("At least one dispatch mode is required.");

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
            existingCustomer.CustomerStateCode = model.CustomerStateCode;
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

            if (model.ContactPersons != null)
                SyncContactPersons(existingCustomer, model.ContactPersons.ToList());
            // --- CompanyCategory  ---
            if (existingCustomer.CustomerCompanyCategories != null && model.CustomerCompanyCategories != null)
            {
                var categoriesToRemove = existingCustomer.CustomerCompanyCategories
                    .Where(existing => !model.CustomerCompanyCategories.Any(m => m.CompanyCategoryID == existing.CompanyCategoryID))
                    .ToList();
                if (categoriesToRemove.Any())
                {
                    _context.CustomerCompanyCategories.RemoveRange(categoriesToRemove);
                    foreach (var category in categoriesToRemove)
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

            if (existingCustomer.CustomerDispatchModes != null && model.CustomerDispatchModes != null)
            {
                var dispatchModesToRemove = existingCustomer.CustomerDispatchModes
                    .Where(existing => !model.CustomerDispatchModes.Any(m => m.DispatchModeID == existing.DispatchModeID))
                    .ToList();
                if (dispatchModesToRemove.Any())
                {
                    _context.CustomerDispatchModes.RemoveRange(dispatchModesToRemove);
                    foreach (var dispatchMode in dispatchModesToRemove)
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

        // ── Contact Person Sync ──────────────────────────────────────────────────
        // Rules:
        //   • contact1  — fixed first, always required, name mandatory
        //   • dynamic   — zero or more, inserted between contact1 and accountant
        //   • accountant — fixed last, optional (skipped if name is blank)
        //
        // Algorithm:
        //   1. Validate contact1 has a name
        //   2. Strip optional contacts whose name is blank (accountant / dynamic)
        //   3. Delete DB rows that are no longer in the incoming list
        //   4. Update rows that exist in DB (ID > 0)
        //   5. Insert new rows (ID == 0) — each treated independently, no in-memory collision
        private void SyncContactPersons(Customer existingCustomer, List<ContactPerson> incoming)
        {
            // 1. contact1 must be present and named
            var contact1 = incoming.FirstOrDefault(c => c.Type == "contact1");
            if (contact1 == null || string.IsNullOrWhiteSpace(contact1.Name))
                throw new ArgumentException("Contact Person 1 name is required.");

            // 2. Drop optional contacts with blank name
            incoming = incoming
                .Where(c => c.Type == "contact1" || !string.IsNullOrWhiteSpace(c.Name))
                .ToList();

            // 3. Delete contacts removed by the user
            var incomingIds = incoming.Where(c => c.ID > 0).Select(c => c.ID).ToHashSet();
            var toDelete = existingCustomer.ContactPersons
                .Where(c => !incomingIds.Contains(c.ID))
                .ToList();
            if (toDelete.Any())
            {
                _context.ContactPersons.RemoveRange(toDelete);
                foreach (var c in toDelete)
                    existingCustomer.ContactPersons.Remove(c);
            }

            // 4 & 5. Update existing rows or insert new ones
            foreach (var contact in incoming)
            {
                contact.CustomerID = existingCustomer.ID;

                if (contact.ID > 0)
                {
                    // Update — match strictly by DB ID (never matches ID=0 rows)
                    var dbRow = existingCustomer.ContactPersons.FirstOrDefault(c => c.ID == contact.ID);
                    if (dbRow == null) continue;
                    dbRow.Salutation   = contact.Salutation;
                    dbRow.Name         = contact.Name;
                    dbRow.Department   = contact.Department;
                    dbRow.EmailId      = contact.EmailId;
                    dbRow.MobileNo     = contact.MobileNo;
                    dbRow.IsWhatsappNo = contact.IsWhatsappNo;
                    dbRow.TelephoneNo  = contact.TelephoneNo;
                    dbRow.SendBill     = contact.SendBill;
                    dbRow.SendReport   = contact.SendReport;
                    dbRow.Address      = contact.Address;
                    dbRow.AreaID       = contact.AreaID;
                    dbRow.City         = contact.City;
                    dbRow.State        = contact.State;
                    dbRow.Country      = contact.Country;
                    dbRow.PinCode      = contact.PinCode;
                    dbRow.Type         = contact.Type;
                }
                else
                {
                    // Insert — ID=0 always means new, never looked up in existing list
                    existingCustomer.ContactPersons.Add(contact);
                }
            }
        }
    }
}
