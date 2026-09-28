using System.Threading.Tasks;
using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IUniversalReviewService
    {
        Task<UniversalResultDetailDto> AssignReviewerAsync(long resultId, long reviewerId, long userId, long branchId, long organizationId, string? remarks = null);
        Task<UniversalReviewFindingDto> CreateFindingAsync(long resultId, CreateFindingRequestDto dto, long userId, long branchId, long organizationId);
        Task<UniversalReviewFindingDto> ResolveFindingAsync(long findingId, string resolution, long userId, long branchId, long organizationId);
        Task<UniversalResultDetailDto> RequestReworkAsync(long resultId, string concurrencyToken, long userId, long branchId, long organizationId, string? remarks = null);
        Task<UniversalResultDetailDto> VerifyAsync(long resultId, string concurrencyToken, long userId, long branchId, long organizationId, string? remarks = null);
        Task<UniversalResultDetailDto> ApproveAsync(long resultId, string concurrencyToken, long userId, long branchId, long organizationId, string? remarks = null);
        Task<UniversalResultDetailDto> RejectApprovalAsync(long resultId, string concurrencyToken, long userId, long branchId, long organizationId, string? remarks = null);
    }
}
