using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ICuttingPriceMasterRepository
    {
        Task AddCuttingPrice(CuttingPriceMaster model);
        Task UpdateCuttingPrice(CuttingPriceMaster model);
        Task DeleteCuttingPrice(CuttingPriceMaster model);
        Task<CuttingPriceMaster> GetCuttingPriceById(long id);
        Task<PagedResponse<object>> GetAllCuttingPrices(PageFilter filter);

        Task<List<DropdwonSelector>> GetCuttingPriceDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<CuttingPriceMaster>> GetAllCuttingPricesList();
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
