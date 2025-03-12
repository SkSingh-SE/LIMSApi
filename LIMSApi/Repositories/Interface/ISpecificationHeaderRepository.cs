using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISpecificationHeaderRepository
    {
        Task AddSpecificationHeader(SpecificationHeader model);
        Task UpdateSpecificationHeader(SpecificationHeader model);
        Task DeleteSpecificationHeader(long id);
        Task<SpecificationHeader> GetSpecificationHeaderById(long id);
        Task<PagedResponse<object>> GetAllSpecificationHeaders(PageFilter filter);

        Task<List<DropdwonSelector>> GetSpecificationHeaderDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
    }
}
