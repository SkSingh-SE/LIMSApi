using System.Linq.Dynamic.Core;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Repositories
{
    public class MetalClassificationRepository : IMetalClassificationRepository
    {
        private readonly LIMSContext _context;

        public MetalClassificationRepository(LIMSContext context)
        {
            _context = context;
        }

        public async Task AddMetalClassification(MetalClassificationMaster model)
        {
            await _context.MetalClassificationMasters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMetalClassification(long id)
        {
            var existingMetalClassification = await _context.MetalClassificationMasters.FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (existingMetalClassification != null)
            {
                existingMetalClassification.IsActive = false;
                existingMetalClassification.ModifiedOn = DateTime.UtcNow;
                _context.MetalClassificationMasters.Update(existingMetalClassification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<MetalClassificationMaster?> GetMetalClassificationById(long id)
        {
            return await _context.MetalClassificationMasters
                .Include(p => p.Parameters)
                .Include(p => p.Parent)
                .Include(p => p.CompatibleTechniques)
                .ThenInclude(t => t.AnalysisTechnique)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
        }

        /// <summary>
        /// Returns the analysis techniques valid for a metal base. If the metal has none configured,
        /// walks up the ParentID chain (base/parent → children inherit). Returns id + name.
        /// </summary>
        public async Task<List<DropdwonSelector>> GetTechniquesForMetal(long metalId)
        {
            long? currentId = metalId;
            // Walk up at most a few levels to find configured techniques (guard against cycles).
            for (int depth = 0; depth < 10 && currentId.HasValue; depth++)
            {
                var node = await _context.MetalClassificationMasters
                    .AsNoTracking()
                    .Where(m => m.ID == currentId.Value && m.IsActive)
                    .Select(m => new { m.ID, m.ParentID })
                    .FirstOrDefaultAsync();

                if (node == null) break;

                var techniques = await _context.MetalClassificationAnalysisTechniques
                    .AsNoTracking()
                    .Where(j => j.MetalClassificationID == node.ID && j.IsActive
                                && j.AnalysisTechnique != null && j.AnalysisTechnique.IsActive)
                    .OrderBy(j => j.AnalysisTechnique!.SortOrder).ThenBy(j => j.AnalysisTechnique!.Name)
                    .Select(j => new DropdwonSelector { Id = j.AnalysisTechniqueID, Name = j.AnalysisTechnique!.Name })
                    .ToListAsync();

                if (techniques.Any())
                    return techniques;

                currentId = node.ParentID; // inherit from parent
            }

            return new List<DropdwonSelector>();
        }

        public async Task UpdateMetalClassification(MetalClassificationMaster model)
        {
            _context.MetalClassificationMasters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<object>> GetAllMetalClassifications(PageFilter filter)
        {
            var _query = (from c in _context.MetalClassificationMasters where c.IsActive select c).AsQueryable().ApplyFilters(filter.Filter);


            if (!string.IsNullOrWhiteSpace(filter.searchTerm))
            {
                var search = filter.searchTerm.Trim();
                _query = _query.Where(x => (x.Name != null && x.Name.Contains(search)) || (x.Code != null && x.Code.Contains(search)));
            }

            if (filter.SortByColumn != null)
            {
                _query = _query.OrderBy($"{filter.SortByColumn} {(filter.SortOrder == "asc" ? "ascending" : "descending")}");
            }

            return await _query.Cast<object>().ToPagedAsync(filter);
        }

        public async Task<List<DropdwonSelector>> GetMetalClassificationDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20)
        {
            if (pageNo < 0) pageNo = 0;

            var _query = from a in _context.MetalClassificationMasters where a.IsActive select a;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (FilterHelper.IsExactIdSearch(searchTerm, out long exactId))
                {
                    _query = _query.Where(x => x.ID == exactId);
                }
                else
                {
                    var search = searchTerm.Trim();
                    _query = _query.Where(x => (x.Name != null && x.Name.Contains(search)));
                }
            }

            var skip = pageNo * pageSize;

            var data = await (_query.Skip(skip).Take(pageSize).Select(x => new DropdwonSelector
            {
                Id = x.ID,
                Name = x.Name,
            })).ToListAsync();

            return data;
        }

        public async Task<List<ParameterMaster>> GetParameterByMetalId(long Id)
        {
            var metal = await _context.MetalClassificationMasters.Include(x => x.Parameters).ThenInclude(y => y.Parameter).FirstOrDefaultAsync(x => x.ID == Id && x.IsActive);

            var data = metal?.Parameters?.Select(x => x.Parameter).ToList();

            return data;

        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _context.MetalClassificationMasters.AnyAsync(x => x.Name == name && x.IsActive);
        }

        public async Task<bool> ExistsByNameAndNotId(string name, long Id)
        {
            return await _context.MetalClassificationMasters.AnyAsync(x => x.Name == name && x.ID != Id && x.IsActive);
        }

        public async Task<bool> ExistsByCode(string code)
        {
            return await _context.MetalClassificationMasters.AnyAsync(x => x.Code == code && x.IsActive);
        }

        public async Task<bool> ExistsByCodeAndNotId(string code, long Id)
        {
            return await _context.MetalClassificationMasters.AnyAsync(x => x.Code == code && x.ID != Id && x.IsActive);
        }
    }
}
