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
        private LoggedInUserDTO loggedInUser;

        public BankService(IBankRepository BankRepo, ILogger<BankService> logger)
        {
            _bankRepository = BankRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateBank(BankMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.BankName))
                throw new ArgumentException("Bank name should not be empty!");

            bool exists = await _bankRepository.ExistsByName(model.BankName);
            if (exists)
                throw new InvalidOperationException("Bank already exists!");

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

            bool exists = await _bankRepository.ExistsByNameAndNotId(model.BankName, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Bank already exists!");

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
