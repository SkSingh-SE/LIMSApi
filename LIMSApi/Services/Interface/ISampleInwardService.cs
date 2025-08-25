using LIMSApi.Dtos;
using LIMSApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Services.Interface
{
    public interface ISampleInwardService
    {
        Task CreateSampleInward([FromForm] SampleInward model);
        Task ModifySampleInward(SampleInward model);
        Task RemoveSampleInward(long id);
        Task<SampleInward> GetSampleInwardDetails(long id);
        Task<PagedResponse<object>> FetchSampleInwardList(PageFilter filter);
        Task<object> GetCaseNoAndSampleNo();

    }
}
