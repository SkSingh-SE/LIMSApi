using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IMaterialTestMappingRepository
    {

        Task<PagedResponse<object>> GetAllAsync(PageFilter filter);
        Task<MaterialTestMapping?> GetByIdAsync(long id);
        Task<IEnumerable<MaterialTestMapping>> GetByClassificationAsync(long metalClassificationId);
        Task AddAsync(MaterialTestMapping entity);
        Task UpdateAsync(MaterialTestMapping entity);
        Task<bool> ExistsAsync(long? metalClassificationId, long? productConditionId, long? gradeId);
        Task<List<DropdwonSelector>> GetSuggestedTestsAsync(TestSuggestionRequest request);
        Task<List<DropdwonSelector>> GetSuggestedGradeAsync(GradeSuggestionRequest request);

    }
}
