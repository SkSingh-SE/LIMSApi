using System.Collections.Generic;
using System.Threading.Tasks;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IConfigurationAdjustmentRepository
    {
        Task<ConfigurationAdjustment?> GetByIdAsync(long id);
        Task<ConfigurationAdjustment?> GetActiveByTestGroupIdAsync(long utgId);
        Task<List<ConfigurationAdjustment>> GetHistoryByTestGroupIdAsync(long utgId);
        Task<List<ConfigurationAdjustmentItem>> GetAuditItemsByTestGroupIdAsync(long utgId, string? section = null, string? changeType = null);
        Task<ConfigurationAdjustment> CreateAsync(ConfigurationAdjustment adjustment);
        Task<ConfigurationAdjustment> UpdateAsync(ConfigurationAdjustment adjustment);
        Task<int> GetNextAdjustmentNumberAsync(long utgId);
        Task<bool> HasExecutionStartedAsync(long utgId);
    }
}
