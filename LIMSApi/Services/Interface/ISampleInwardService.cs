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
        Task RemoveSampleInward(long id);
        Task<SampleInwardDto> GetSampleInwardDetails(long id);
        Task<SampleInwardDto> GetSampleInwardWithPlans(long id);
        Task<PagedResponse<object>> FetchSampleInwardList(PageFilter filter);
        Task<object> GetCaseNoAndSampleNo();

    }
}
