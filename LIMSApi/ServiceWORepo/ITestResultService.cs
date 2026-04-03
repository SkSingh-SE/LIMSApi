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
        Task<StartTestResponse> StartTest(long headerId);

        // -------------------------------------------------------------
        // Complete Test
        // -------------------------------------------------------------
        Task<CompleteTestResponse> CompleteTest(long headerId);

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

        // -------------------------------------------------------------
        // Calculation Engine
        // -------------------------------------------------------------
        Task<CalculateParametersResultDto> CalculateParameters(long headerId);

        // -------------------------------------------------------------
        // Standalone Parameter
        // -------------------------------------------------------------
        Task<object> AddStandaloneParameter(long headerId, AddStandaloneParameterDto dto);

        // -------------------------------------------------------------
        // Add Parameter from Another Test Method
        // -------------------------------------------------------------
        Task<object> AddParameterFromMethod(long headerId, AddParameterFromMethodDto dto);

        // -------------------------------------------------------------
        // Environment at Test Time
        // -------------------------------------------------------------
        Task<EnvironmentAtTimeDto> GetEnvironmentAtTime(long headerId);

        // -------------------------------------------------------------
        // Load Parameters from Specification into existing header
        // -------------------------------------------------------------
        Task<object> LoadParametersFromSpecAsync(long headerId);

        // -------------------------------------------------------------
        // Auto-create TestResultHeaders from approved plan
        // -------------------------------------------------------------
        Task<AutoCreateHeadersResponse> AutoCreateHeadersFromPlanAsync(long planId);

        // -------------------------------------------------------------
        // Phase 5: Test Verification Workflow
        // -------------------------------------------------------------
        Task SubmitForVerification(long headerId);
        Task<object> SubmitSampleForVerification(long sampleId);
        Task<PagedResponse<object>> GetVerificationList(PageFilter filter);
        Task VerifyTest(long headerId, string? comments);
        Task RejectVerification(long headerId, string? comments);

        // -------------------------------------------------------------
        // Phase 5: Preparation Status
        // -------------------------------------------------------------
        Task<PreparationStatusDto> GetPreparationStatus(long sampleId);

        // -------------------------------------------------------------
        // Phase 6: Unified Price Summary
        // -------------------------------------------------------------
        Task<UnifiedPriceSummaryDto> GetUnifiedPriceSummary(long sampleId);

        // -------------------------------------------------------------
        // Orientation Mismatch Check
        // -------------------------------------------------------------
        Task<OrientationCheckResultDto> CheckOrientationMismatch(long headerId);

        // -------------------------------------------------------------
        // Machining Charge Items (simple line items per sample)
        // -------------------------------------------------------------
        Task<List<MachiningChargeLineDto>> GetMachiningItems(long sampleId);
        Task<MachiningChargeLineDto> AddMachiningItem(long sampleId, MachiningChargeItemDto dto);
        Task<MachiningChargeLineDto> UpdateMachiningItem(long itemId, MachiningChargeItemDto dto);
        Task DeleteMachiningItem(long itemId);

    }
}
