using LIMSApi.Dtos;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.ServiceWORepo
{
    public interface IReportService
    {
        Task<PagedResponse<object>> GetReportDashboardList(PageFilter filter);
        Task<ReportReadDto> CreateReportFromSampleAsync(ReportCreateFromSampleDto dto);
        Task<string> GeneratePdfForSampleAsync(long sampleId);
        Task<ReportReadDto> GetReportAsync(long id);
        Task<bool> PerformAction(WorkflowActionRequestDto dto);
        Task<ReportPreviewDto> GetReportPreviewAsync(long reportHeaderId);

        /// <summary>
        /// Builds the ReportDataDto by joining Report + TestResultHeader + TestResultParameter + Customer + Sample data.
        /// </summary>
        Task<ReportDataDto> BuildReportDataAsync(long reportHeaderId);

        /// <summary>
        /// Generates a professional PDF using the EnhancedReportDocument and returns the file path.
        /// </summary>
        Task<string> GenerateEnhancedPdfAsync(long reportHeaderId);

        Task RequestAmendmentAsync([FromQuery] long reportHeaderId, string reason, IFormFile file);

        /// <summary>
        /// Generates a PDF using the specified format type.
        /// </summary>
        Task<byte[]> GenerateReportByFormatAsync(long reportHeaderId, ReportFormatType formatType, string? watermark = null);

        /// <summary>
        /// Public verification: returns basic report info for QR code scan (no auth).
        /// </summary>
        Task<object?> GetReportVerificationAsync(string reportNo);
    }
}
