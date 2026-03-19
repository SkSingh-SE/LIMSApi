using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IProductSpecificationGradeRepository
    {
        Task AddProductSpecificationGrade(ProductSpecificationGrade model);
        Task UpdateProductSpecificationGrade(ProductSpecificationGrade model);
        Task DeleteProductSpecificationGrade(long id);
        Task<ProductSpecificationGrade?> GetProductSpecificationGradeById(long id);
        Task<PagedResponse<object>> GetAllProductSpecificationGrades(PageFilter filter);
        Task<List<object>> GetByProductSpecificationId(long productSpecId);
    }
}
