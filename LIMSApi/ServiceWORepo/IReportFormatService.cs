using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.ServiceWORepo
{
    public interface IReportFormatService
    {
        Task<PagedResponse<object>> GetFormatListAsync(PageFilter filter);
        Task<ReportFormatDto?> GetFormatByIdAsync(long id);
        Task<long> CreateFormatAsync(ReportFormatDto dto);
        Task UpdateFormatAsync(long id, ReportFormatDto dto);
        Task DeleteFormatAsync(long id);
        Task<long> CloneFormatAsync(long id);
        Task<ReportFormat?> ResolveFormatForSampleAsync(long sampleId);
        Task<List<ReportFormatMappingDto>> GetMappingsForFormatAsync(long formatId);
        Task SaveMappingsAsync(long formatId, List<ReportFormatMappingDto> mappings);
        Task<List<object>> GetFormatDropdownAsync(string? searchTerm, int pageNo, int pageSize);
        List<object> GetSectionTypes();
    }
}
