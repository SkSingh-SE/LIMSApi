using System.Threading.Tasks;
using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IUniversalTestExecutionService
    {
        Task<TestExecutionDto> StartExecutionAsync(long universalTestGroupId, long userId, long branchId, long organizationId, bool isRetest = false);
        Task<TestExecutionDto?> GetExecutionByIdAsync(long testExecutionId, long branchId, long organizationId, bool canViewAllBranches = false);
        Task<TestExecutionDto?> GetExecutionByGroupIdAsync(long universalTestGroupId, long branchId, long organizationId, bool canViewAllBranches = false);
        Task<TestExecutionDto> SaveObservationsAsync(long testExecutionId, TestExecutionSaveDto data, long userId, long branchId, long organizationId);
        Task<TestExecutionDto> CompleteExecutionAsync(long testExecutionId, long userId, long branchId, long organizationId);
        Task<TestExecutionDto> VerifyExecutionAsync(long testExecutionId, ExecutionActionDto dto, long userId, long branchId, long organizationId);
        Task<TestExecutionDto> ApproveExecutionAsync(long testExecutionId, ExecutionActionDto dto, long userId, long branchId, long organizationId);
        Task<TestExecutionDto> RejectExecutionAsync(long testExecutionId, ExecutionActionDto dto, long userId, long branchId, long organizationId);

        // Extended Phase C.1 Operations
        Task<TestExecutionDto> UpdateConfigurationAsync(long testExecutionId, TestExecutionConfigSnapshotDto updatedConfig, long userId, long branchId, long organizationId);
        Task<ExecutionCalculationTraceDto> GetCalculationTraceAsync(long testExecutionId, long branchId, long organizationId);
        Task<ResultsOverviewDto> GetResultsOverviewAsync(long testExecutionId, long branchId, long organizationId);
        Task<NablScopeSummaryDto> GetNablScopeSummaryAsync(long testExecutionId, long branchId, long organizationId);
        Task<FormulaPreviewResponseDto> PreviewFormulaAsync(FormulaPreviewRequestDto request);
        Task<TestExecutionDto> AddAttachmentAsync(long testExecutionId, ExecutionAttachmentUploadDto uploadDto, long userId, long branchId, long organizationId);
        Task<byte[]> GenerateReportPdfAsync(long testExecutionId, long branchId, long organizationId);
    }
}
