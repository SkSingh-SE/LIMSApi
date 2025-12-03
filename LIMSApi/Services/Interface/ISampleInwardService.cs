using LIMSApi.Dtos;
using LIMSApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Services.Interface
{
    public interface ISampleInwardService
    {
        Task CreateSampleInward(SampleInwardDto model);
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

    }
}
