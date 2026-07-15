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
            existingParameterUnit.ConversionFactor = model.ConversionFactor;

            // Equivalents = source of truth. Upsert by ID, soft-delete the removed ones
            // (so future FK references survive), then sync inline columns for legacy readers.
            ApplyEquivalents(existingParameterUnit, model.Equivalents);
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

        public async Task<List<EquivalentUnitOption>> GetEquivalentUnits(long unitId)
        {
            var baseUnit = await _context.ParameterUnitMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ID == unitId && u.IsActive);

            var result = new List<EquivalentUnitOption>();
            if (baseUnit == null) return result;

            // Base unit first (EquivalentId = null) so the parameter's default unit binds.
            result.Add(new EquivalentUnitOption
            {
                EquivalentId = null,
                BaseUnitId = baseUnit.ID,
                Name = baseUnit.Name,
                ConversionFactor = baseUnit.ConversionFactor,
                IsBase = true
            });

            // Then the normalized child equivalents (stable IDs).
            var equivalents = await _context.ParameterUnitEquivalents
                .AsNoTracking()
                .Where(e => e.BaseParameterUnitID == baseUnit.ID && e.IsActive)
                .OrderBy(e => e.DisplayOrder ?? int.MaxValue).ThenBy(e => e.ID)
                .Select(e => new EquivalentUnitOption
                {
                    EquivalentId = e.ID,
                    BaseUnitId = baseUnit.ID,
                    Name = e.Name,
                    ConversionFactor = e.ConversionFactor,
                    IsBase = false
                })
                .ToListAsync();
            result.AddRange(equivalents);

            return result;
        }
    }
}
