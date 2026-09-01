using LIMSApi.Dtos;
using LIMSApi.Helpers;

namespace LIMSApi.Services.Interface
{
    public interface IProductMasterService
    {
        Task<object> CreateProductMaster(ProductMasterCreateDto dto);
        Task<object> UpdateProductMaster(ProductMasterUpdateDto dto);
        Task DeleteProductMaster(long id);
        Task<ProductMasterDetailsDto?> GetProductMasterById(long id);
        Task<PagedResponse<object>> GetAllProductMasters(PageFilter filter);
        Task<List<DropdwonSelector>> GetProductMasterDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20, long metalId = 0);
        Task<GradeParametersDto?> GetGradeParametersByGradeId(long gradeId);
        Task<List<string>> GetPrefixOptions();
        Task<bool> AddPrefixOption(string prefix);
    }
}
