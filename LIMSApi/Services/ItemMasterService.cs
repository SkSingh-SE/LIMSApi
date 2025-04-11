using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ItemMasterService : IItemMasterService
    {
        private readonly IItemMasterRepository _itemRepository;
        private readonly ILogger<ItemMasterService> _logger;
        private LoggedInUserDTO loggedInUser;

        public ItemMasterService(IItemMasterRepository itemMasterRepository, ILogger<ItemMasterService> logger)
        {
            _itemRepository = itemMasterRepository;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateItem(ItemMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("ItemMaster name should not be empty!");

            bool exists = await _itemRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("ItemMaster already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _itemRepository.AddItem(model);
            _logger.LogInformation("ItemMaster '{ItemMasterName}' created successfully.", model.Name);
        }

        public async Task ModifyItem(ItemMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("ItemMaster ID should not be empty!");

            bool exists = await _itemRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same ItemMaster already exists!");

            var existingItemMaster = await _itemRepository.GetItemById(model.ID);
            if (existingItemMaster == null)
                throw new InvalidOperationException("ItemMaster not found!");


            existingItemMaster.Name = model.Name;
            existingItemMaster.Description = model.Description;
            existingItemMaster.ModifiedOn = DateTime.UtcNow;
            existingItemMaster.ModifiedBy = loggedInUser.EmployeeID;

            await _itemRepository.UpdateItem(existingItemMaster);
            _logger.LogInformation("ItemMaster '{ItemMasterName}' updated successfully.", model.Name);
        }

        public async Task RemoveItem(long id)
        {
            var existingItemMaster = await _itemRepository.GetItemById(id);
            if (existingItemMaster == null)
                throw new InvalidOperationException("ItemMaster not found!");

            existingItemMaster.IsActive = false;
            existingItemMaster.ModifiedOn = DateTime.UtcNow;
            existingItemMaster.ModifiedBy = loggedInUser.EmployeeID;

            await _itemRepository.DeleteItem(existingItemMaster);
            _logger.LogInformation("ItemMaster with ID '{ItemMasterId}' deleted successfully.", id);
        }

        public async Task<ItemMaster> GetItemDetails(long id)
        {
            var classification = await _itemRepository.GetItemById(id);
            if (classification == null)
                throw new InvalidOperationException("ItemMaster not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchItemList(PageFilter filter)
        {
            return await _itemRepository.GetAllItems(filter);
        }

        public async Task<List<DropdwonSelector>> GetItemDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _itemRepository.GetItemDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
