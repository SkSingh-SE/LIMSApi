using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface ILabScopeConfigurationService
    {
        Task<PagedResponse<ScopeListItemDto>> QueryScopesAsync(ScopeListRequest request);
        Task<ScopeDetailDto> GetScopeDetailsAsync(long id);
        Task<long> CreateScopeAsync(ScopeCreateDto dto);
        Task ModifyScopeAsync(ScopeUpdateDto dto);
        Task<bool> ToggleScopeStatusAsync(long id, bool activate);
        Task RemoveScopeAsync(long id);
        Task<ScopeDecisionDto> ValidateAsync(ScopeValidateRequest request);
        Task<ScopePreviewDto> PreviewAsync(ScopePreviewRequest request);
        Task<AccreditationContextDto> GetAccreditationContextAsync(long? branchId);
    }
}
