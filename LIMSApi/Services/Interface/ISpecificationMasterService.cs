using LIMSApi.Dtos;
using LIMSApi.Helpers;

namespace LIMSApi.Services.Interface
{
    public interface ISpecificationMasterService
    {
        Task<PagedResponse<object>> FetchSpecificationMasterList(PageFilter filter);
        Task<SpecificationMasterDetailDto> GetSpecificationMasterDetails(long id);
        Task CreateSpecificationMaster(SpecificationMasterCreateDto dto);
        Task ModifySpecificationMaster(SpecificationMasterUpdateDto dto);
        Task<bool> ToggleSpecificationMasterStatus(long id);
        Task<List<DropdwonSelector>> GetSpecificationMasterDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetStandardOrganizationsDropdown();

        // Grade Management
        Task<List<SpecificationGradeDto>> GetGradesBySpecification(long specId, bool includeInactive = false);
        Task<SpecificationGradeDto> GetGradeById(long gradeId);
        Task<long> CreateGrade(long specId, SpecificationGradeCreateDto dto);
        Task ModifyGrade(long gradeId, SpecificationGradeUpdateDto dto);
        Task<bool> ToggleGradeStatus(long gradeId);
    }
}
