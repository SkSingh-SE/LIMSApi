using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class CuttingPriceMasterService : ICuttingPriceMasterService
    {
        private readonly ICuttingPriceMasterRepository _itemRepository;
        private readonly ILogger<CuttingPriceMasterService> _logger;
        private LoggedInUserDTO loggedInUser;

        public CuttingPriceMasterService(ICuttingPriceMasterRepository itemMasterRepository, ILogger<CuttingPriceMasterService> logger)
        {
            _itemRepository = itemMasterRepository;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateCuttingPrice(CuttingPriceMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.CuttingType))
                throw new ArgumentException("CuttingPriceMaster name should not be empty!");

            bool exists = await _itemRepository.ExistsByName(model.CuttingType);
            if (exists)
                throw new InvalidOperationException("CuttingPriceMaster already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _itemRepository.AddCuttingPrice(model);
            _logger.LogInformation("CuttingPriceMaster '{CuttingPriceMasterName}' created successfully.", model.CuttingType);
        }

        public async Task ModifyCuttingPrice(CuttingPriceMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("CuttingPriceMaster ID should not be empty!");

            bool exists = await _itemRepository.ExistsByNameAndNotId(model.CuttingType, model.ID);
            if (exists)
                throw new InvalidOperationException("Same CuttingPriceMaster already exists!");

            var existingCuttingPriceMaster = await _itemRepository.GetCuttingPriceById(model.ID);
            if (existingCuttingPriceMaster == null)
                throw new InvalidOperationException("CuttingPriceMaster not found!");


            existingCuttingPriceMaster.CuttingType = model.CuttingType;
            existingCuttingPriceMaster.UnitType = model.UnitType;
            existingCuttingPriceMaster.RatePerUnit = model.RatePerUnit;
            existingCuttingPriceMaster.Remark = model.Remark;
            existingCuttingPriceMaster.SpecimenTypeId = model.SpecimenTypeId;
            existingCuttingPriceMaster.ModifiedOn = DateTime.UtcNow;
            existingCuttingPriceMaster.ModifiedBy = loggedInUser.EmployeeID;

            await _itemRepository.UpdateCuttingPrice(existingCuttingPriceMaster);
            _logger.LogInformation("CuttingPriceMaster '{CuttingPriceMasterName}' updated successfully.", model.CuttingType);
        }

        public async Task RemoveCuttingPrice(long id)
        {
            var existingCuttingPriceMaster = await _itemRepository.GetCuttingPriceById(id);
            if (existingCuttingPriceMaster == null)
                throw new InvalidOperationException("CuttingPriceMaster not found!");

            existingCuttingPriceMaster.IsActive = false;
            existingCuttingPriceMaster.ModifiedOn = DateTime.UtcNow;
            existingCuttingPriceMaster.ModifiedBy = loggedInUser.EmployeeID;

            await _itemRepository.DeleteCuttingPrice(existingCuttingPriceMaster);
            _logger.LogInformation("CuttingPriceMaster with ID '{CuttingPriceMasterId}' deleted successfully.", id);
        }

        public async Task<CuttingPriceMaster> GetCuttingPriceDetails(long id)
        {
            var classification = await _itemRepository.GetCuttingPriceById(id);
            if (classification == null)
                throw new InvalidOperationException("CuttingPriceMaster not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchCuttingPriceList(PageFilter filter)
        {
            return await _itemRepository.GetAllCuttingPrices(filter);
        }

        public async Task<List<DropdwonSelector>> GetCuttingPriceDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _itemRepository.GetCuttingPriceDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<List<CuttingPriceMaster>> CuttingPriceList()
        {
            return await _itemRepository.GetAllCuttingPricesList();
        }

        public async Task<CuttingPriceMaster?> GetPriceBySpecimenAndCuttingType(long? specimenTypeId, string cuttingType)
        {
            return await _itemRepository.GetBySpecimenAndCuttingType(specimenTypeId, cuttingType);
        }
    }
}
