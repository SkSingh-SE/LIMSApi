using System.Collections.Generic;
using System.Threading.Tasks;
using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IUniversalReportService
    {
        Task<UniversalReportPreviewDto> PreviewAsync(long testExecutionId, long userId, long branchId, long organizationId, string? requestedFormatCode = null, bool canViewAllBranches = false);
        Task<UniversalReportDetailDto> GenerateAsync(long testExecutionId, long userId, long branchId, long organizationId, string? remarks = null, string? requestedFormatCode = null, bool canViewAllBranches = false);
        Task<UniversalReportDetailDto> ReleaseAsync(long reportId, long userId, long branchId, long organizationId, string? remarks = null);
        Task<UniversalReportDetailDto> ReissueAsync(long reportId, long userId, long branchId, long organizationId, string? reason = null);
        Task<UniversalReportDetailDto> VoidAsync(long reportId, long userId, long branchId, long organizationId, string? reason = null);
        Task<UniversalReportDetailDto?> GetByIdAsync(long reportId, long branchId, long organizationId, bool canViewAllBranches = false);
        Task<List<UniversalReportListItemDto>> ListByExecutionAsync(long testExecutionId, long branchId, long organizationId, bool canViewAllBranches = false);
        Task<(byte[] PdfBytes, string ReportNo, string FileName)> GeneratePdfBytesAsync(long reportId, long branchId, long organizationId, bool canViewAllBranches = false, string? watermark = null);
        Task<List<ReportFormatOptionDto>> GetAvailableFormatsAsync(long organizationId, string companyCode);
    }
}
