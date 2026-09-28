using System.Threading.Tasks;
using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IUniversalResultService
    {
        Task<UniversalResultDetailDto> EvaluateAsync(long testExecutionId, long userId, long branchId, long organizationId, string? remarks = null);
        Task<UniversalResultDetailDto?> GetLatestByExecutionAsync(long testExecutionId, long branchId, long organizationId, bool canViewAllBranches = false);
        Task<UniversalResultDetailDto?> GetByIdAsync(long resultId, long branchId, long organizationId, bool canViewAllBranches = false);
        Task<UniversalResultDetailDto> FinalizeAsync(long resultId, string concurrencyToken, long userId, long branchId, long organizationId, string? remarks = null);
        Task<Phase9HandoffDto> GetPhase9HandoffAsync(long testExecutionId, long branchId, long organizationId, bool canViewAllBranches = false);
    }
}
