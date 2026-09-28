using System.Collections.Generic;
using System.Threading.Tasks;
using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IConfigurationAdjustmentService
    {
        Task<ConfigurationAdjustmentDetailDto?> GetActiveAdjustmentAsync(long utgId);
        Task<ConfigurationAdjustmentDetailDto?> GetByIdAsync(long id);
        Task<List<ConfigurationAdjustmentDetailDto>> GetHistoryAsync(long utgId);
        Task<List<ConfigurationAdjustmentItemDto>> GetDifferenceAuditAsync(long utgId, string? section = null, string? changeType = null);
        Task<ConfigurationAdjustmentDetailDto> SaveDraftAsync(ConfigurationAdjustmentDraftDto draft);
        Task<AdjustmentValidationResultDto> ValidateAdjustmentAsync(long utgId, List<ConfigurationAdjustmentItemDto> items);
        Task<ConfigurationAdjustmentDetailDto> ApplyAdjustmentAsync(ApplyAdjustmentRequestDto request);
        Task<ConfigurationAdjustmentDetailDto> ApproveAdjustmentAsync(ApproveAdjustmentRequestDto request);
        Task<ConfigurationAdjustmentDetailDto> RejectAdjustmentAsync(RejectAdjustmentRequestDto request);
        Task<DeviationLookupResultDto> GetDeviationLookupOptionsAsync(long utgId, string deviationCategory);
        Task<ConfigurationAdjustmentDetailDto> RequestExecutionDeviationAsync(ExecutionDeviationRequestDto request);
        Task<ComprehensiveDifferenceAuditDto> GetComprehensiveDifferenceAuditAsync(long utgId);
        Task<AdjustedConfigurationDto> GetAdjustedConfigurationAsync(long utgId, bool includeUnapproved = false);
    }
}
