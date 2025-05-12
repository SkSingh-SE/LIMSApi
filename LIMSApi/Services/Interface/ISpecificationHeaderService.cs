using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ISpecificationHeaderService
    {
        Task CreateSpecificationHeader(SpecificationHeader model);
        Task ModifySpecificationHeader(SpecificationHeader model);
        Task RemoveSpecificationHeader(long id);
        Task<SpecificationHeader> GetSpecificationHeaderDetails(long id);
        Task<PagedResponse<object>> FetchSpecificationHeaderList(PageFilter filter);
        Task<PagedResponse<object>> FetchCustomSpecificationHeaderList(PageFilter filter);

        Task<List<DropdwonSelector>> GetSpecificationHeaderDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
