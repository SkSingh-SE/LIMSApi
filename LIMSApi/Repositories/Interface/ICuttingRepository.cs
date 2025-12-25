using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ICuttingRepository
    {
        Task<CuttingChargeHeader> CreateAsync(CuttingChargeHeader payload);
        Task<PagedResponse<object>> GetAllCuttingList(PageFilter filter);
        Task<CuttingChargeHeader?> GetByIdAsync(long id);
        Task<CuttingChargeHeader?> GetByInwardIdAsync(long inwardId);

        Task UpdateAsync(CuttingChargeHeader model);

        Task<bool> ExistsByInwardIdAsync(long inwardId);
    }
}
