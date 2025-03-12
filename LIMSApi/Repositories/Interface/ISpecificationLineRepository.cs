using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISpecificationLineRepository
    {
        Task AddSpecificationLine(SpecificationLine model);
        Task UpdateSpecificationLine(SpecificationLine model);
        Task DeleteSpecificationLine(long id);
        Task<SpecificationLine> GetSpecificationLineById(long id);
        Task<PagedResponse<object>> GetAllSpecificationLines(PageFilter filter);

        Task<List<DropdwonSelector>> GetSpecificationLineDropdown(string? searchTerm, int pageNo, int pageSize);
       
    }
}
