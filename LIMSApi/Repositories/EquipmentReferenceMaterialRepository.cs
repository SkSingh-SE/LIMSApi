using LIMSApi.Data;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class EquipmentReferenceMaterialRepository : IEquipmentReferenceMaterialRepository
    {
        private readonly LIMSContext _context;
        private readonly LoggedInUserDTO loggedInUser;

        public EquipmentReferenceMaterialRepository(LIMSContext context)
        {
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<List<EquipmentReferenceMaterial>> GetByEquipmentId(long equipmentMasterID)
        {
            return await _context.EquipmentReferenceMaterials
                .Where(x => x.EquipmentMasterID == equipmentMasterID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode)
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();
        }

        public async Task<EquipmentReferenceMaterial?> GetById(long id)
        {
            return await _context.EquipmentReferenceMaterials
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
        }

        public async Task Add(EquipmentReferenceMaterial model)
        {
            model.CompanyCode = loggedInUser.CompanyCode;
            model.CreatedBy = loggedInUser.UserId;
            model.CreatedOn = DateTime.UtcNow;
            model.IsActive = true;
            await _context.EquipmentReferenceMaterials.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(EquipmentReferenceMaterial model)
        {
            model.ModifiedBy = loggedInUser.UserId;
            model.ModifiedOn = DateTime.UtcNow;
            _context.EquipmentReferenceMaterials.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                entity.IsActive = false;
                entity.ModifiedBy = loggedInUser.UserId;
                entity.ModifiedOn = DateTime.UtcNow;
                _context.EquipmentReferenceMaterials.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
