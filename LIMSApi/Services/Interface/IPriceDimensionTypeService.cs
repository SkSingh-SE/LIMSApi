using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IPriceDimensionTypeService
    {
        Task CreatePriceDimensionType(PriceDimensionType model);
        Task ModifyPriceDimensionType(PriceDimensionType model);
        Task<PriceDimensionType> RemovePriceDimensionType(long id);
        Task<PriceDimensionType> GetPriceDimensionTypeDetails(long id);
        Task<PagedResponse<object>> FetchPriceDimensionTypeList(PageFilter filter);
        Task<List<DropdwonSelector>> GetPriceDimensionTypeDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
