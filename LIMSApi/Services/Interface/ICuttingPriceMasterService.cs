using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ICuttingPriceMasterService
    {
        Task CreateCuttingPrice(CuttingPriceMaster model);
        Task ModifyCuttingPrice(CuttingPriceMaster model);
        Task RemoveCuttingPrice(long id);
        Task<CuttingPriceMaster> GetCuttingPriceDetails(long id);
        Task<List<object>> CuttingPriceList();
        Task<PagedResponse<object>> FetchCuttingPriceList(PageFilter filter);

        Task<List<DropdwonSelector>> GetCuttingPriceDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<object?> GetPriceBySpecimenAndCuttingType(long? specimenTypeId, string cuttingType);
    }
}
