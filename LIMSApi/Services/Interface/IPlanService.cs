using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IPlanService
    {
        Task<List<PlanHistory>> GetPlanHistory(long planId);
        Task RequestReplan(long planId, string reason);
        Task ApproveReplan(long requestId, string? remarks);
        Task RejectReplan(long requestId, string? remarks);
        Task CreatePlanHistoryEntry(long planId, string changeType, string? previousDataJson, string? newDataJson, string? fieldChangesJson, string? remarks);

        // Plan Tab 6-Tier Decision Engine Cascade API Methods
        Task<object> GetProductMasterCascadeAsync(long productMasterId);
        Task<object> GetProductMasterSizeLimitsAsync(long productMasterId, long sizeId);
        Task<object> GetMetalClassificationCascadeAsync(long metalClassificationId);
        Task<object> GetMaterialSpecCascadeAsync(long materialSpecId);
        Task<object> GetLabTestCascadeAsync(long labTestId);
        Task<object> GetTechniqueAnalysisTypesAsync(long techniqueId, long metalId);
    }
}
