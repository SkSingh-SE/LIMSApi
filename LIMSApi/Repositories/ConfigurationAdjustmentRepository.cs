using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class ConfigurationAdjustmentRepository : IConfigurationAdjustmentRepository
    {
        private readonly LIMSContext _context;

        public ConfigurationAdjustmentRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task<ConfigurationAdjustment?> GetByIdAsync(long id)
        {
            return await _context.ConfigurationAdjustments
                .Include(a => a.Items)
                .Include(a => a.UniversalTestGroup)
                .Include(a => a.Branch)
                .FirstOrDefaultAsync(a => a.ID == id && a.IsActive);
        }

        public async Task<ConfigurationAdjustment?> GetActiveByTestGroupIdAsync(long utgId)
        {
            // Returns latest adjustment for this UTG (Draft, Applied, or Approved)
            return await _context.ConfigurationAdjustments
                .Include(a => a.Items)
                .Include(a => a.UniversalTestGroup)
                .Include(a => a.Branch)
                .Where(a => a.UniversalTestGroupID == utgId && a.IsActive)
                .OrderByDescending(a => a.AdjustmentNumber)
                .ThenByDescending(a => a.CreatedOn)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ConfigurationAdjustment>> GetHistoryByTestGroupIdAsync(long utgId)
        {
            return await _context.ConfigurationAdjustments
                .Include(a => a.Items)
                .Where(a => a.UniversalTestGroupID == utgId && a.IsActive)
                .OrderByDescending(a => a.AdjustmentNumber)
                .ToListAsync();
        }

        public async Task<List<ConfigurationAdjustmentItem>> GetAuditItemsByTestGroupIdAsync(
            long utgId, string? section = null, string? changeType = null)
        {
            var query = _context.ConfigurationAdjustmentItems
                .Include(i => i.ConfigurationAdjustment)
                .Where(i => i.UniversalTestGroupID == utgId && i.IsActive);

            if (!string.IsNullOrWhiteSpace(section))
                query = query.Where(i => i.Section == section);

            if (!string.IsNullOrWhiteSpace(changeType))
                query = query.Where(i => i.ChangeType == changeType);

            return await query.OrderByDescending(i => i.CreatedOn).ToListAsync();
        }

        public async Task<ConfigurationAdjustment> CreateAsync(ConfigurationAdjustment adjustment)
        {
            await _context.ConfigurationAdjustments.AddAsync(adjustment);
            await _context.SaveChangesAsync();
            return adjustment;
        }

        public async Task<ConfigurationAdjustment> UpdateAsync(ConfigurationAdjustment adjustment)
        {
            _context.ConfigurationAdjustments.Update(adjustment);
            await _context.SaveChangesAsync();
            return adjustment;
        }

        public async Task<int> GetNextAdjustmentNumberAsync(long utgId)
        {
            var max = await _context.ConfigurationAdjustments
                .Where(a => a.UniversalTestGroupID == utgId)
                .MaxAsync(a => (int?)a.AdjustmentNumber);

            return (max ?? 0) + 1;
        }

        public async Task<bool> HasExecutionStartedAsync(long utgId)
        {
            // Check if test execution already exists or UTG is in progress / completed
            return await _context.TestExecutions.AnyAsync(e => e.UniversalTestGroupID == utgId && e.IsActive);
        }
    }
}
