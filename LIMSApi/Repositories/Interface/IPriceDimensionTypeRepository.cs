using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IPriceDimensionTypeRepository
    {
        Task<PriceDimensionType> AddPriceDimensionType(PriceDimensionType model);
        Task<PriceDimensionType> DeletePriceDimensionType(long id);
        Task<PriceDimensionType?> GetPriceDimensionTypeById(long id);
        Task<PriceDimensionType> UpdatePriceDimensionType(PriceDimensionType model);
        Task<PagedResponse<object>> GetAllPriceDimensionTypes(PageFilter filter);
        Task<List<DropdwonSelector>> GetPriceDimensionTypeDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
    }
}
