using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IProductSpecificationGradeService
    {
        Task CreateProductSpecificationGrade(ProductSpecificationGrade model);
        Task ModifyProductSpecificationGrade(ProductSpecificationGrade model);
        Task RemoveProductSpecificationGrade(long id);
        Task<ProductSpecificationGrade> GetProductSpecificationGradeDetails(long id);
        Task<PagedResponse<object>> FetchProductSpecificationGradeList(PageFilter filter);
        Task<List<object>> GetByProductSpecificationId(long productSpecId);
    }
}
