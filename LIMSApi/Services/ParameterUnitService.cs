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

            var validEquivalents = (model.Equivalents ?? new List<ParameterUnitEquivalent>())
                .Where(e => !string.IsNullOrWhiteSpace(e.Name))
                .ToList();

            var order = 0;
            foreach (var e in validEquivalents)
            {
                e.ID = 0;
                e.Name = e.Name.Trim();
                e.DisplayOrder = ++order;
                e.IsActive = true;
            }
            model.Equivalents = validEquivalents;

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

            var existingParameterUnit = await _context.ParameterUnitMasters
                .Include(x => x.Equivalents)
                .FirstOrDefaultAsync(x => x.ID == model.ID && x.IsActive);

            if (existingParameterUnit == null)
                throw new InvalidOperationException("ParameterUnit not found!");

            existingParameterUnit.Name = model.Name?.Trim() ?? string.Empty;
            existingParameterUnit.ConversionFactor = model.ConversionFactor;
            existingParameterUnit.ModifiedOn = DateTime.UtcNow;

            var incoming = (model.Equivalents ?? new List<ParameterUnitEquivalent>())
                .Where(e => !string.IsNullOrWhiteSpace(e.Name))
                .ToList();

            var order = 0;
            var incomingIds = new HashSet<long>();

            foreach (var inc in incoming)
            {
                order++;
                if (inc.ID > 0)
                {
                    incomingIds.Add(inc.ID);
                    var match = existingParameterUnit.Equivalents.FirstOrDefault(e => e.ID == inc.ID);
                    if (match != null)
                    {
                        match.Name = inc.Name.Trim();
                        match.ConversionFactor = inc.ConversionFactor;
                        match.DisplayOrder = order;
                        match.IsActive = true;
                    }
                    else
                    {
                        existingParameterUnit.Equivalents.Add(new ParameterUnitEquivalent
                        {
                            BaseParameterUnitID = existingParameterUnit.ID,
                            Name = inc.Name.Trim(),
                            ConversionFactor = inc.ConversionFactor,
                            DisplayOrder = order,
                            IsActive = true
                        });
                    }
                }
                else
                {
                    existingParameterUnit.Equivalents.Add(new ParameterUnitEquivalent
                    {
                        BaseParameterUnitID = existingParameterUnit.ID,
                        Name = inc.Name.Trim(),
                        ConversionFactor = inc.ConversionFactor,
                        DisplayOrder = order,
                        IsActive = true
                    });
                }
            }

            // Soft-delete equivalents that were in DB as active but not present in incoming list
            foreach (var e in existingParameterUnit.Equivalents.Where(e => e.IsActive && e.ID > 0 && !incomingIds.Contains(e.ID)))
            {
                e.IsActive = false;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("ParameterUnit '{ParameterUnitName}' updated successfully.", model.Name);
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

        public async Task<List<GroupedUnitDropdownOption>> GetGroupedParameterUnitDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _ParameterUnitRepository.GetGroupedParameterUnitDropdown(searchTerm, pageNo, pageSize);
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
