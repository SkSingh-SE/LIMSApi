using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class BankService : IBankService
    {
        private readonly IBankRepository _bankRepository;
        private readonly ILogger<BankService> _logger;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public BankService(IBankRepository BankRepo, ILogger<BankService> logger, LIMSContext context)
        {
            _bankRepository = BankRepo;
            _logger = logger;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateBank(BankMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.BankName))
                throw new ArgumentException("Bank name should not be empty!");
            if (string.IsNullOrWhiteSpace(model.AccountHolderName))
                throw new ArgumentException("Account holder name should not be empty!");
            if (string.IsNullOrWhiteSpace(model.AccountNumber))
                throw new ArgumentException("Account number should not be empty!");
            if (string.IsNullOrWhiteSpace(model.AccountType))
                throw new ArgumentException("Account type should not be empty!");
            if (string.IsNullOrWhiteSpace(model.IFSCCode))
                throw new ArgumentException("IFSC code should not be empty!");
            var ifscRegex = new System.Text.RegularExpressions.Regex(@"^[A-Z]{4}0[A-Z0-9]{6}$");
            if (!ifscRegex.IsMatch(model.IFSCCode.Trim().ToUpper()))
                throw new ArgumentException("Invalid IFSC code format (e.g. SBIN0001234).");
            if (string.IsNullOrWhiteSpace(model.BranchName))
                throw new ArgumentException("Branch name should not be empty!");
            model.BankName = model.BankName.Trim();
            model.AccountHolderName = model.AccountHolderName.Trim();
            model.IFSCCode = model.IFSCCode.Trim().ToUpper();
            model.BranchName = model.BranchName.Trim();

            bool exists = await _bankRepository.ExistsByName(model.BankName);
            if (exists)
                throw new InvalidOperationException("Bank already exists!");

            bool accountExists = await _bankRepository.ExistsByAccountNumber(model.AccountNumber.Trim());
            if (accountExists)
                throw new InvalidOperationException("A bank with this account number already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _bankRepository.AddBank(model);
            _logger.LogInformation("Bank '{BankName}' created successfully.", model.BankName);
        }

        public async Task ModifyBank(BankMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Bank ID should not be empty!");

            if (string.IsNullOrWhiteSpace(model.BankName))
                throw new ArgumentException("Bank name should not be empty!");
            if (string.IsNullOrWhiteSpace(model.AccountHolderName))
                throw new ArgumentException("Account holder name should not be empty!");
            if (string.IsNullOrWhiteSpace(model.AccountNumber))
                throw new ArgumentException("Account number should not be empty!");
            if (string.IsNullOrWhiteSpace(model.AccountType))
                throw new ArgumentException("Account type should not be empty!");
            if (string.IsNullOrWhiteSpace(model.IFSCCode))
                throw new ArgumentException("IFSC code should not be empty!");
            var ifscRegex = new System.Text.RegularExpressions.Regex(@"^[A-Z]{4}0[A-Z0-9]{6}$");
            if (!ifscRegex.IsMatch(model.IFSCCode.Trim().ToUpper()))
                throw new ArgumentException("Invalid IFSC code format (e.g. SBIN0001234).");
            if (string.IsNullOrWhiteSpace(model.BranchName))
                throw new ArgumentException("Branch name should not be empty!");
            model.BankName = model.BankName.Trim();
            model.AccountHolderName = model.AccountHolderName.Trim();
            model.IFSCCode = model.IFSCCode.Trim().ToUpper();
            model.BranchName = model.BranchName.Trim();

            bool exists = await _bankRepository.ExistsByNameAndNotId(model.BankName, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Bank already exists!");

            bool accountExists = await _bankRepository.ExistsByAccountNumberAndNotId(model.AccountNumber.Trim(), model.ID);
            if (accountExists)
                throw new InvalidOperationException("A bank with this account number already exists!");

            var existingBank = await _bankRepository.GetBankById(model.ID);
            if (existingBank == null)
                throw new InvalidOperationException("Bank not found!");


            existingBank.BankName = model.BankName;
            existingBank.AccountHolderName = model.AccountHolderName;
            existingBank.AccountNumber = model.AccountNumber;
            existingBank.AccountType = model.AccountType;
            existingBank.BranchName = model.BranchName;
            existingBank.IFSCCode = model.IFSCCode;
            existingBank.ModifiedOn = DateTime.UtcNow;
            existingBank.ModifiedBy = loggedInUser.EmployeeID;

            await _bankRepository.UpdateBank(existingBank);
            _logger.LogInformation("Bank '{BankName}' updated successfully.", model.BankName);
        }

        public async Task RemoveBank(long id)
        {
            var existingBank = await _bankRepository.GetBankById(id);
            if (existingBank == null)
                throw new InvalidOperationException("Bank not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<BankMaster>(_context, id, "Bank");

            existingBank.IsActive = false;
            existingBank.ModifiedOn = DateTime.UtcNow;
            existingBank.ModifiedBy = loggedInUser.EmployeeID;

            await _bankRepository.DeleteBank(existingBank);
            _logger.LogInformation("Bank with ID '{BankId}' deleted successfully.", id);
        }

        public async Task<BankMaster> GetBankDetails(long id)
        {
            var classification = await _bankRepository.GetBankById(id);
            if (classification == null)
                throw new InvalidOperationException("Bank not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchBankList(PageFilter filter)
        {
            return await _bankRepository.GetAllBanks(filter);
        }

        public async Task<List<DropdwonSelector>> GetBankDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _bankRepository.GetBankDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
