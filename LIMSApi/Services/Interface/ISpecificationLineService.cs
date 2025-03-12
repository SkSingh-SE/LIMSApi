using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ISpecificationLineService
    {
        Task CreateSpecificationLine(SpecificationLine model);
        Task ModifySpecificationLine(SpecificationLine model);
        Task RemoveSpecificationLine(long id);
        Task<SpecificationLine> GetSpecificationLineDetails(long id);
        Task<PagedResponse<object>> FetchSpecificationLineList(PageFilter filter);

        Task<List<DropdwonSelector>> GetSpecificationLineDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
