using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISpecificationMasterRepository
    {
        Task<PagedResponse<object>> GetAllSpecificationMasters(PageFilter filter, string companyCode);
        Task<SpecificationHeader?> GetSpecificationMasterById(long id, string companyCode);
        Task<bool> ExistsByCode(string code, string companyCode);
        Task<bool> ExistsByCodeAndNotId(string code, long id, string companyCode);
        Task AddSpecificationMaster(SpecificationHeader entity);
        Task UpdateSpecificationMaster(SpecificationHeader entity);
        Task<List<DropdwonSelector>> GetSpecificationMasterDropdown(string? searchTerm, int pageNo, int pageSize, string companyCode);
        Task<List<DropdwonSelector>> GetStandardOrganizationsDropdown();
        Task<int> GetVersionCount(long id);
        Task<bool> HasDownstreamReferences(long id);

        // Grade Management
        Task<List<SpecificationGrade>> GetGradesBySpecificationIdAsync(long specificationHeaderId, bool includeInactive = false);
        Task<SpecificationGrade?> GetGradeByIdAsync(long gradeId);
        Task<bool> GradeExistsAsync(long specificationHeaderId, string grade, long excludeGradeId = 0);
        Task AddGradeAsync(SpecificationGrade grade);
        Task UpdateGradeAsync(SpecificationGrade grade);
        Task<int> GetGradeRequirementCountAsync(long gradeId);
        Task<bool> GradeHasDownstreamReferencesAsync(long gradeId);
    }
}
