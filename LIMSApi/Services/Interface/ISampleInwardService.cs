using LIMSApi.Dtos;
using LIMSApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Services.Interface
{
    public interface ISampleInwardService
    {
        Task<long> CreateSampleInward(SampleInwardDto model);
        Task ModifySampleInward(SampleInwardDto model);
        Task ModifySamplePlan(PlanDto model);
        Task SubmitPlanForReview(PlanDto model);
        Task RemoveSampleInward(long id);
        Task<SampleInwardDto> GetSampleInwardDetails(long id);
        Task<SampleInwardDto> GetSampleInwardWithPlans(long id);
        Task<PagedResponse<object>> FetchSampleInwardList(PageFilter filter);
        Task<PagedResponse<object>> FetchPlanList(PageFilter filter);
        Task<PagedResponse<object>> FetchReviewList(PageFilter filter);
        Task<object> GetCaseNoAndSampleNo();
        Task<List<DropdwonSelector>> GetSampleInwardDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetSamplePreparationInwardDropdown(string? searchTerm, int pageNo, int pageSize);

        Task<byte[]> GeneratePIPdfAsync(long piId);
        Task<byte[]> GenerateInwardChallanPdfAsync(long inwardId);
        Task CancelSampleAsync(long sampleDetailId, string reason);
        Task DeleteSampleAsync(long sampleDetailId);
        Task<PaymentInfoDto> UpdatePaymentInfoAsync(long id, PaymentInfoDto dto);
        Task UpdateSamplePrepAsync(long sampleId, SamplePrepReviewDto dto);
        Task<string> CompleteSamplePreparationAsync(long inwardId);

        Task StopReportAsync(long inwardId, string reason);
        Task UnstopReportAsync(long inwardId);

        Task<string> VerifyAndLockReviewOfRequestAsync(long inwardId, string? remarks = null);
        Task RequestReplanAsync(long inwardId, string reason);
        Task ApproveReplanAsync(long replanRequestId, string remarks);
        Task<LifecycleSummaryDto?> GetLifecycleSummaryAsync(long id);
    }
}

