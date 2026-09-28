using LIMSApi.Data;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class LabScopeRepository : ILabScopeRepository
    {
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public LabScopeRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task AddLabScope(LabScopeMaster model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            await _context.LabScopeMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateLabScope(LabScopeMaster model)
        {
            _context.LabScopeMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<LabScopeMaster?> GetLabScopeById(long id)
        {
            return await _context.LabScopeMasters
            .Include(x => x.Specifications)
                .ThenInclude(s => s.TestMethodSpecification)
            .Include(x => x.Specifications)
                .ThenInclude(s => s.TestMethodSpecificationVersion)
            .Include(x => x.Specifications)
                .ThenInclude(s => s.Parameters)
                    .ThenInclude(p => p.Equipments)
            .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);
        }
    }
}
