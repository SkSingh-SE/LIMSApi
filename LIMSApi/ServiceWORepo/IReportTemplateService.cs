using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.ServiceWORepo
{
    public interface IReportTemplateService
    {
        Task<List<ReportTemplateDto>> GetTemplatesAsync(string? testType = null);

        Task<ReportTemplateDto?> GetTemplateByIdAsync(long templateId);

        Task<long> CreateTemplateAsync(ReportTemplateDto dto);

        Task UpdateTemplateAsync(long templateId, ReportTemplateDto dto);

        Task DeleteTemplateAsync(long templateId);

        Task<long> CloneTemplateAsync(long templateId);
    }
}
