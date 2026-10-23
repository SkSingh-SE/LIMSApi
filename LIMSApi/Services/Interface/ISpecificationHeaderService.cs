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
        Task<SpecificationHeader> GetCloneTemplate(long id);
        Task<PagedResponse<object>> FetchSpecificationHeaderList(PageFilter filter);
        Task<PagedResponse<object>> FetchCustomSpecificationHeaderList(PageFilter filter);

        Task<List<DropdwonSelector>> GetSpecificationHeaderDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetGradeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetGradeDropdownMetalWise(string? searchTerm, int pageNo, int pageSize, long metalId);
        Task<List<DropdwonSelector>> GetDefaultStandardForSpecification(long gradeId);
        Task<List<DropdwonSelector>> GetTestMethodsForSpecifications(long gradeId1, long gradeId2 = 0);
        Task<List<ChemicalElementDto>> GetChemicalElementsBySpecificationsAsync(long gradeId1 = 0, long gradeId2 = 0);
    }
}
