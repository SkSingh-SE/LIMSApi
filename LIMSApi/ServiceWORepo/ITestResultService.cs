using LIMSApi.Dtos;
using LIMSApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.ServiceWORepo
{
    public interface ITestResultService
    {
        // -------------------------------------------------------------
        // Dashboard List (Paging + Search + Filters)
        // -------------------------------------------------------------
        Task<PagedResponse<object>> GetTestingDashboardList(PageFilter filter);

        // -------------------------------------------------------------
        // Full Sample Result Payload (General + Chemical)
        // -------------------------------------------------------------
        Task<object?> GetSampleDetailsForResult(long sampleId);

        // -------------------------------------------------------------
        // Load a Specific TestResultHeader
        // -------------------------------------------------------------
        Task<TestResultHeader?> GetHeaderAsync(long headerId);

        // -------------------------------------------------------------
        // Save Complete Result Payload (General + Chemical)
        // -------------------------------------------------------------
        Task SaveTestResult(TestResultSaveDto dto);

        // -------------------------------------------------------------
        // Update Single Parameter (Inline update)
        // -------------------------------------------------------------
        Task<object> UpdateParameterAsync(long headerId, long paramId, TestResultParameterDto dto);

        // -------------------------------------------------------------
        // Start Test
        // -------------------------------------------------------------
        Task StartTest(long headerId);

        // -------------------------------------------------------------
        // Complete Test
        // -------------------------------------------------------------
        Task CompleteTest(long headerId);

        // -------------------------------------------------------------
        // Move Test to Long-Term Testing Queue
        // -------------------------------------------------------------
        Task MoveToLongTerm(MoveToLongTermDto dto);
        Task<PagedResponse<object>> GetLongTermList(PageFilter filter);
        Task<object?> GetLongTermDetail(long longTermTestId);

        Task<object?> GetParametersForHeader(long headerId);
        Task RecordLongTermReading(LongTermRecordDto dto);
        Task<string> UploadTestImageAsync(long headerId, IFormFile file, string? caption);
        Task<List<TestResultImageDto>> UploadTestImagesAsync(long headerId, List<IFormFile> files, List<string>? captions);
        Task<List<TestResultImageDto>> UploadedTestImages(long headerId);


    }
}
