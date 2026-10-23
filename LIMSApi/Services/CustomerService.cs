using System.Text.Json;
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
        private readonly IWorkflowService _workflowService;
        private LoggedInUserDTO loggedInUser;

        public CustomerService(ICustomerRepository customerRepo, ILogger<CustomerService> logger, LIMSContext context, IWorkflowService workflowService)
        {
            _customerRepository = customerRepo;
            _logger = logger;
            _context = context;
            _workflowService = workflowService;
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
            // ── Level 2 approval (non-admin) ───────────────────────────────────────
            CustomerChangeValuesDto? proposed = null;
            if (!IsAdmin())
            {
                proposed = SnapshotLevel2(model);
                var defaults = DefaultLevel2Snapshot();
                // Reset Group B fields to safe defaults before saving
                model.CreditLimitAmount = null;
                model.CreditLimitTime = null;
                model.ConstantDiscount = false;
                model.ConstantDiscountPercentage = null;
                model.WeeklyBillingCustomer = false;
                model.MonthlyBillingCustomer = false;
                model.BillingEvery = false;
                model.BillingEveryDays = null;
                // CustomerType (Group A) saves immediately — no change
            }

            await _customerRepository.AddCustomer(model);

            // Create change request after customer is saved (needs CustomerID)
            if (!IsAdmin() && proposed != null && HasLevel2Changes(DefaultLevel2Snapshot(model), proposed))
                await CreateAndStartChangeRequest(model.ID, DefaultLevel2Snapshot(model), proposed);

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

            // ── Level 2 approval snapshot (non-admin) ──────────────────────────────
            var oldLevel2 = SnapshotLevel2(existingCustomer);
            var proposedLevel2 = SnapshotLevel2(model);

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
            // Group A: CustomerType saves immediately for all users
            existingCustomer.CustomerType = model.CustomerType;
            existingCustomer.IsBlock = model.IsBlock;
            existingCustomer.BlockReason = model.BlockReason;
            existingCustomer.GSTNo = model.GSTNo;
            existingCustomer.CustomerStateCode = model.CustomerStateCode;
            existingCustomer.PANNo = model.PANNo;
            existingCustomer.GSTNA = model.GSTNA;
            existingCustomer.SampleReturn = model.SampleReturn;
            existingCustomer.SpecialAccountingCase = model.SpecialAccountingCase;
            existingCustomer.DirectTaxInvoiceNoPerforma = model.DirectTaxInvoiceNoPerforma;
            existingCustomer.PerformaInvoiceRequiredBeforeTesting = model.PerformaInvoiceRequiredBeforeTesting;
            existingCustomer.IsVerified = model.IsVerified;
            existingCustomer.Remark = model.Remark;

            // Group B: apply directly for admin; defer via change request for non-admin
            if (IsAdmin())
            {
                existingCustomer.BillingEvery = model.BillingEvery;
                existingCustomer.BillingEveryDays = model.BillingEveryDays;
                existingCustomer.WeeklyBillingCustomer = model.WeeklyBillingCustomer;
                existingCustomer.MonthlyBillingCustomer = model.MonthlyBillingCustomer;
                existingCustomer.ConstantDiscount = model.ConstantDiscount;
                existingCustomer.ConstantDiscountPercentage = model.ConstantDiscountPercentage;
                existingCustomer.CreditLimitAmount = model.CreditLimitAmount;
                existingCustomer.CreditLimitTime = model.CreditLimitTime;
            }

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

            // Trigger change request for non-admin Level 2 changes
            if (!IsAdmin() && HasLevel2Changes(oldLevel2, proposedLevel2))
                await CreateAndStartChangeRequest(existingCustomer.ID, oldLevel2, proposedLevel2);

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

        // ── Level 2 Change Request ────────────────────────────────────────────────

        public async Task<List<CustomerChangeRequestResponseDto>> GetChangeRequests(long customerId)
        {
            var requests = await _context.CustomerChangeRequests
                .Where(r => r.CustomerID == customerId && r.IsActive)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();

            var employeeIds = requests
                .SelectMany(r => new[] { r.CreatedBy, r.ReviewedBy ?? 0 })
                .Where(id => id > 0).Distinct().ToList();

            var employees = await _context.EmployeeMasters
                .Where(e => employeeIds.Contains(e.ID))
                .Select(e => new { e.ID, e.Name })
                .ToDictionaryAsync(e => e.ID, e => e.Name);

            return requests.Select(r => new CustomerChangeRequestResponseDto
            {
                ID = r.ID,
                CustomerID = r.CustomerID,
                OldValues = JsonSerializer.Deserialize<CustomerChangeValuesDto>(r.OldValuesJson) ?? new(),
                NewValues = JsonSerializer.Deserialize<CustomerChangeValuesDto>(r.NewValuesJson) ?? new(),
                Status = r.Status,
                RejectionReason = r.RejectionReason,
                ReviewedByName = r.ReviewedBy.HasValue && employees.TryGetValue(r.ReviewedBy.Value, out var rv) ? rv : null,
                ReviewedOn = r.ReviewedOn,
                CreatedOn = r.CreatedOn,
                RequestedByName = employees.TryGetValue(r.CreatedBy, out var cb) ? cb : null,
                WorkflowInstanceID = r.WorkflowInstanceID,
            }).ToList();
        }

        public async Task<CustomerChangeRequestResponseDto?> GetPendingChangeRequest(long customerId)
        {
            var r = await _context.CustomerChangeRequests
                .FirstOrDefaultAsync(r => r.CustomerID == customerId && r.Status == "Pending" && r.IsActive);
            if (r == null) return null;

            return new CustomerChangeRequestResponseDto
            {
                ID = r.ID,
                CustomerID = r.CustomerID,
                OldValues = JsonSerializer.Deserialize<CustomerChangeValuesDto>(r.OldValuesJson) ?? new(),
                NewValues = JsonSerializer.Deserialize<CustomerChangeValuesDto>(r.NewValuesJson) ?? new(),
                Status = r.Status,
                WorkflowInstanceID = r.WorkflowInstanceID,
            };
        }

        public async Task ApplyChangeRequest(long changeRequestId)
        {
            var req = await _context.CustomerChangeRequests
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.ID == changeRequestId)
                ?? throw new KeyNotFoundException("Change request not found.");

            var proposed = JsonSerializer.Deserialize<CustomerChangeValuesDto>(req.NewValuesJson)
                ?? throw new InvalidOperationException("Invalid change request data.");
            var customer = req.Customer!;

            customer.CreditLimitAmount = proposed.CreditLimitAmount;
            customer.CreditLimitTime = proposed.CreditLimitTime;
            customer.ConstantDiscount = proposed.ConstantDiscount ?? false;
            customer.ConstantDiscountPercentage = proposed.ConstantDiscountPercentage;
            customer.WeeklyBillingCustomer = proposed.WeeklyBillingCustomer ?? false;
            customer.MonthlyBillingCustomer = proposed.MonthlyBillingCustomer ?? false;
            customer.BillingEvery = proposed.BillingEvery ?? false;
            customer.BillingEveryDays = proposed.BillingEveryDays;
            // CustomerType (Group A) already saved immediately — nothing to apply here

            customer.ModifiedOn = DateTime.UtcNow;
            customer.ModifiedBy = loggedInUser.EmployeeID;

            req.Status = "Approved";
            req.ReviewedBy = loggedInUser.EmployeeID;
            req.ReviewedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("CustomerChangeRequest {ID} approved — Group B fields applied to Customer {CustomerID}.", req.ID, req.CustomerID);
        }

        public async Task RejectChangeRequest(long changeRequestId, string? reason)
        {
            var req = await _context.CustomerChangeRequests
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.ID == changeRequestId)
                ?? throw new KeyNotFoundException("Change request not found.");

            var proposed = JsonSerializer.Deserialize<CustomerChangeValuesDto>(req.NewValuesJson) ?? new();
            var old = JsonSerializer.Deserialize<CustomerChangeValuesDto>(req.OldValuesJson) ?? new();
            var customer = req.Customer!;

            // Revert CustomerType (Group A) since it was saved immediately on submit
            if (proposed.CustomerType != old.CustomerType && old.CustomerType != null)
                customer.CustomerType = old.CustomerType;

            req.Status = "Rejected";
            req.RejectionReason = reason;
            req.ReviewedBy = loggedInUser.EmployeeID;
            req.ReviewedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("CustomerChangeRequest {ID} rejected.", req.ID);
        }

        public async Task DirectReviewChangeRequest(ReviewChangeRequestDto dto)
        {
            if (dto.Action.Equals("Approve", StringComparison.OrdinalIgnoreCase))
                await ApplyChangeRequest(dto.ChangeRequestId);
            else if (dto.Action.Equals("Reject", StringComparison.OrdinalIgnoreCase))
                await RejectChangeRequest(dto.ChangeRequestId, dto.Remarks);
            else
                throw new ArgumentException("Action must be 'Approve' or 'Reject'.");
        }

        // ── Level 2 Helpers ──────────────────────────────────────────────────────

        private bool IsAdmin() =>
            string.Equals(loggedInUser?.Role, "Admin", StringComparison.OrdinalIgnoreCase);

        private static CustomerChangeValuesDto SnapshotLevel2(Customer c) => new()
        {
            CustomerType = c.CustomerType,
            CreditLimitAmount = c.CreditLimitAmount,
            CreditLimitTime = c.CreditLimitTime,
            ConstantDiscount = c.ConstantDiscount,
            ConstantDiscountPercentage = c.ConstantDiscountPercentage,
            WeeklyBillingCustomer = c.WeeklyBillingCustomer,
            MonthlyBillingCustomer = c.MonthlyBillingCustomer,
            BillingEvery = c.BillingEvery,
            BillingEveryDays = c.BillingEveryDays,
        };

        // Snapshot of Group B defaults (what gets saved to Customer on create before approval)
        private static CustomerChangeValuesDto DefaultLevel2Snapshot(Customer? c = null) => new()
        {
            CustomerType = c?.CustomerType,  // CustomerType is whatever was saved (Group A)
            CreditLimitAmount = null,
            CreditLimitTime = null,
            ConstantDiscount = false,
            ConstantDiscountPercentage = null,
            WeeklyBillingCustomer = false,
            MonthlyBillingCustomer = false,
            BillingEvery = false,
            BillingEveryDays = null,
        };

        private static bool HasLevel2Changes(CustomerChangeValuesDto old, CustomerChangeValuesDto proposed)
        {
            return old.CustomerType != proposed.CustomerType
                || old.CreditLimitAmount != proposed.CreditLimitAmount
                || old.CreditLimitTime != proposed.CreditLimitTime
                || old.ConstantDiscount != proposed.ConstantDiscount
                || old.ConstantDiscountPercentage != proposed.ConstantDiscountPercentage
                || old.WeeklyBillingCustomer != proposed.WeeklyBillingCustomer
                || old.MonthlyBillingCustomer != proposed.MonthlyBillingCustomer
                || old.BillingEvery != proposed.BillingEvery
                || old.BillingEveryDays != proposed.BillingEveryDays;
        }

        private async Task CreateAndStartChangeRequest(long customerId,
            CustomerChangeValuesDto oldValues, CustomerChangeValuesDto proposedValues)
        {
            // Supersede any existing pending request for this customer
            var existing = await _context.CustomerChangeRequests
                .Where(r => r.CustomerID == customerId && r.Status == "Pending" && r.IsActive)
                .ToListAsync();
            foreach (var r in existing)
                r.Status = "Superseded";

            var changeRequest = new CustomerChangeRequest
            {
                CustomerID = customerId,
                OldValuesJson = JsonSerializer.Serialize(oldValues),
                NewValuesJson = JsonSerializer.Serialize(proposedValues),
                Status = "Pending",
                CreatedBy = loggedInUser.EmployeeID,
                CreatedOn = DateTime.UtcNow,
                CompanyCode = loggedInUser.CompanyCode ?? string.Empty,
                IsActive = true,
            };
            _context.CustomerChangeRequests.Add(changeRequest);
            await _context.SaveChangesAsync();

            // Start workflow if configured; otherwise leave as Pending for direct review
            bool workflowExists = await _workflowService.WorkflowExistsForEntityType("Customer Field Change");
            if (workflowExists)
            {
                await _workflowService.StartWorkflow(changeRequest.ID, "Customer Field Change");
                var instance = await _workflowService.GetActiveInstanceForEntityAsync(changeRequest.ID, "Customer Field Change");
                if (instance != null)
                {
                    changeRequest.WorkflowInstanceID = instance.ID;
                    await _context.SaveChangesAsync();
                }
            }
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
