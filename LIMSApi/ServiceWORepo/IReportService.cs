using LIMSApi.Dtos;
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


        Task RequestAmendmentAsync([FromQuery] long reportHeaderId, string reason, IFormFile file);
    }
}
