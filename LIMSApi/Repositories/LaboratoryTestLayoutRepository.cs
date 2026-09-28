using LIMSApi.Data;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class LaboratoryTestLayoutRepository : ILaboratoryTestLayoutRepository
    {
        private readonly LIMSContext _context;

        public LaboratoryTestLayoutRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task<List<LaboratoryTestLayout>> GetByTestAsync(long testId, string companyCode)
        {
            return await _context.LaboratoryTestLayouts.AsNoTracking()
                .Include(x => x.ExecutionLayout)
                .Include(x => x.TestMethodSpecification)
                .Include(x => x.TestMethodSpecificationVersion)
                .Where(x => x.LaboratoryTestID == testId && x.CompanyCode == companyCode)
                .OrderBy(x => x.Priority).ThenBy(x => x.ID)
                .ToListAsync();
        }

        public async Task<LaboratoryTestLayout?> GetByIdAsync(long id, string companyCode)
        {
            return await _context.LaboratoryTestLayouts
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == companyCode);
        }

        public async Task AddAsync(LaboratoryTestLayout model)
        {
            await _context.LaboratoryTestLayouts.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LaboratoryTestLayout model)
        {
            _context.LaboratoryTestLayouts.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsDuplicateAsync(long testId, long layoutId, long? methodId, long? versionId, long excludeId, string companyCode)
        {
            return await _context.LaboratoryTestLayouts.AsNoTracking()
                .AnyAsync(x => x.ID != excludeId && x.IsActive
                    && x.LaboratoryTestID == testId && x.ExecutionLayoutID == layoutId
                    && x.TestMethodSpecificationID == methodId
                    && x.TestMethodSpecificationVersionID == versionId
                    && x.CompanyCode == companyCode);
        }

        public async Task<bool> ExistsDefaultInScopeAsync(long testId, long? methodId, long? versionId, long excludeId, string companyCode)
        {
            return await _context.LaboratoryTestLayouts.AsNoTracking()
                .AnyAsync(x => x.ID != excludeId && x.IsActive && x.IsDefault
                    && x.LaboratoryTestID == testId
                    && x.TestMethodSpecificationID == methodId
                    && x.TestMethodSpecificationVersionID == versionId
                    && x.CompanyCode == companyCode);
        }
    }
}
