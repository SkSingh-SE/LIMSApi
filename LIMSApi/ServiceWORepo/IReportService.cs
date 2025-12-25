using LIMSApi.Dtos;

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

    }
}
