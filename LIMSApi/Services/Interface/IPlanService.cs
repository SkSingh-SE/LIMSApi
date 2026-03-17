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
    }
}
