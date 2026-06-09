using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class ParameterUnitService : IParameterUnitService
    {
        private readonly IParameterUnitRepository _ParameterUnitRepository;
        private readonly ILogger<ParameterUnitService> _logger;
        private readonly LIMSContext _context;

        public ParameterUnitService(IParameterUnitRepository ParameterUnitRepo, ILogger<ParameterUnitService> logger, LIMSContext context)
        {
            _ParameterUnitRepository = ParameterUnitRepo;
            _logger = logger;
            _context = context;
        }

        public async Task CreateParameterUnit(ParameterUnitMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("ParameterUnit name should not be empty!");

            bool exists = await _ParameterUnitRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("ParameterUnit already exists!");

            // Normalize incoming equivalents (fresh insert) + sync inline columns for legacy readers.
            var order = 0;
            foreach (var e in model.Equivalents ?? new List<ParameterUnitEquivalent>())
            {
                e.ID = 0;
                e.Name = e.Name?.Trim() ?? string.Empty;
                e.DisplayOrder = ++order;
                e.IsActive = true;
            }
            SyncInlineFromEquivalents(model);

            await _ParameterUnitRepository.AddParameterUnit(model);
            _logger.LogInformation("ParameterUnit '{ParameterUnitName}' created successfully.", model.Name);
        }

        public async Task ModifyParameterUnit(ParameterUnitMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("ParameterUnit ID should not be empty!");

            bool exists = await _ParameterUnitRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same ParameterUnit already exists!");

            var existingParameterUnit = await _ParameterUnitRepository.GetParameterUnitById(model.ID);
            if (existingParameterUnit == null)
                throw new InvalidOperationException("ParameterUnit not found!");

            existingParameterUnit.Name = model.Name;
            existingParameterUnit.ConversaionFactor = model.ConversaionFactor;

            // Equivalents = source of truth. Upsert by ID, soft-delete the removed ones
            // (so future FK references survive), then sync inline columns for legacy readers.
            ApplyEquivalents(existingParameterUnit, model.Equivalents);
            SyncInlineFromEquivalents(existingParameterUnit);
            existingParameterUnit.ModifiedOn = DateTime.UtcNow;

            await _ParameterUnitRepository.UpdateParameterUnit(existingParameterUnit);
            _logger.LogInformation("ParameterUnit '{ParameterUnitName}' updated successfully.", model.Name);
        }

        // Upsert child equivalents by ID; soft-delete the ones no longer present.
        private static void ApplyEquivalents(ParameterUnitMaster existing, ICollection<ParameterUnitEquivalent> incoming)
        {
            incoming ??= new List<ParameterUnitEquivalent>();
            var order = 0;
            foreach (var inc in incoming)
            {
                order++;
                var match = inc.ID > 0 ? existing.Equivalents.FirstOrDefault(e => e.ID == inc.ID) : null;
                if (match == null)
                {
                    existing.Equivalents.Add(new ParameterUnitEquivalent
                    {
                        BaseParameterUnitID = existing.ID,
                        Name = inc.Name?.Trim() ?? string.Empty,
                        ConversionFactor = inc.ConversionFactor,
                        DisplayOrder = order,
                        IsActive = true
                    });
                }
                else
                {
                    match.Name = inc.Name?.Trim() ?? string.Empty;
                    match.ConversionFactor = inc.ConversionFactor;
                    match.DisplayOrder = order;
                    match.IsActive = true;
                }
            }
            // Soft-delete equivalents not present in the incoming set.
            var incomingIds = incoming.Where(i => i.ID > 0).Select(i => i.ID).ToHashSet();
            foreach (var e in existing.Equivalents.Where(e => e.IsActive && e.ID > 0 && !incomingIds.Contains(e.ID)))
                e.IsActive = false;
        }

        // Mirror the first 7 active equivalents back into inline SimilarUnit1-7 so legacy
        // readers (TestResult/Report) keep working until they migrate to the child table.
        private static void SyncInlineFromEquivalents(ParameterUnitMaster unit)
        {
            var active = unit.Equivalents
                .Where(e => e.IsActive && !string.IsNullOrWhiteSpace(e.Name))
                .OrderBy(e => e.DisplayOrder ?? int.MaxValue)
                .ThenBy(e => e.ID)
                .Take(7)
                .ToList();

            string?[] names = new string?[7];
            decimal?[] factors = new decimal?[7];
            for (int i = 0; i < active.Count; i++) { names[i] = active[i].Name; factors[i] = active[i].ConversionFactor; }

            unit.SimilarUnit1 = names[0]; unit.ConversionFactor1 = factors[0];
            unit.SimilarUnit2 = names[1]; unit.ConversionFactor2 = factors[1];
            unit.SimilarUnit3 = names[2]; unit.ConversionFactor3 = factors[2];
            unit.SimilarUnit4 = names[3]; unit.ConversionFactor4 = factors[3];
            unit.SimilarUnit5 = names[4]; unit.ConversionFactor5 = factors[4];
            unit.SimilarUnit6 = names[5]; unit.ConversionFactor6 = factors[5];
            unit.SimilarUnit7 = names[6]; unit.ConversionFactor7 = factors[6];
        }

        public async Task RemoveParameterUnit(long id)
        {
            var existingParameterUnit = await _ParameterUnitRepository.GetParameterUnitById(id);
            if (existingParameterUnit == null)
                throw new InvalidOperationException("ParameterUnit not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<ParameterUnitMaster>(_context, id, "Parameter Unit");

            existingParameterUnit.IsActive = false;
            existingParameterUnit.ModifiedOn = DateTime.UtcNow;

            await _ParameterUnitRepository.UpdateParameterUnit(existingParameterUnit);
            _logger.LogInformation("ParameterUnit with ID '{ParameterUnitId}' deleted successfully.", id);
        }

        public async Task<ParameterUnitMaster> GetParameterUnitDetails(long id)
        {
            var classification = await _ParameterUnitRepository.GetParameterUnitById(id);
            if (classification == null)
                throw new InvalidOperationException("ParameterUnit not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchParameterUnitList(PageFilter filter)
        {
            return await _ParameterUnitRepository.GetAllParameterUnits(filter);
        }

        public async Task<List<DropdwonSelector>> GetParameterUnitDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _ParameterUnitRepository.GetParameterUnitDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<List<DropdwonSelector>> GetEquivalentUnits(long unitId)
        {
            var baseUnit = await _context.ParameterUnitMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ID == unitId && u.IsActive);

            // Always return the base unit first so the parameter's default unit binds even with no equivalents.
            var result = new List<DropdwonSelector>();
            if (baseUnit == null) return result;
            result.Add(new DropdwonSelector { Id = baseUnit.ID, Name = baseUnit.Name });

            // Equivalents can be stored in EITHER direction, so match both:
            //  (a) other units whose Name is listed in THIS unit's SimilarUnit1-7, and
            //  (b) other units that list THIS unit's Name in THEIR SimilarUnit1-7.
            var baseName = baseUnit.Name.Trim();
            var similarNames = new[]
            {
                baseUnit.SimilarUnit1, baseUnit.SimilarUnit2, baseUnit.SimilarUnit3, baseUnit.SimilarUnit4,
                baseUnit.SimilarUnit5, baseUnit.SimilarUnit6, baseUnit.SimilarUnit7
            }
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n!.Trim())
            .ToList();

            var equivalents = await _context.ParameterUnitMasters
                .AsNoTracking()
                .Where(u => u.IsActive && u.ID != baseUnit.ID && (
                    similarNames.Contains(u.Name) ||
                    u.SimilarUnit1 == baseName || u.SimilarUnit2 == baseName || u.SimilarUnit3 == baseName ||
                    u.SimilarUnit4 == baseName || u.SimilarUnit5 == baseName || u.SimilarUnit6 == baseName ||
                    u.SimilarUnit7 == baseName))
                .Select(u => new DropdwonSelector { Id = u.ID, Name = u.Name })
                .ToListAsync();
            result.AddRange(equivalents);

            return result;
        }
    }
}
