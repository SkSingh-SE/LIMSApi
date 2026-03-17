using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ICuttingService
    {
        Task CreateAsync(CuttingChargeHeader payload);
        Task<PagedResponse<object>> GetAllAsync(PageFilter filter);
        Task<CuttingChargeHeader?> GetByIdAsync(long id);
        Task<CuttingChargeHeader?> GetByInwardIdAsync(long inwardId);
        Task UpdateAsync(CuttingChargeHeader model);
        Task UpdatePreparationStatusAsync(long sampleId, string status);
    }
}
