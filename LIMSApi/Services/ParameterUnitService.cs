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

        public async Task CreateParameterUnit(ParameterUnitCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Unit Code is required.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Unit Name is required.");

            if (string.IsNullOrWhiteSpace(dto.Symbol))
                throw new ArgumentException("Unit Symbol is required.");

            if (dto.ConversionFactor.HasValue && dto.ConversionFactor.Value <= 0)
                throw new ArgumentException("Conversion factor must be greater than 0.");

            var upperCode = dto.Code.Trim().ToUpper();
            var trimmedName = dto.Name.Trim();

            if (await _ParameterUnitRepository.ExistsByCode(upperCode))
                throw new InvalidOperationException($"Unit code '{upperCode}' already exists!");

            if (await _ParameterUnitRepository.ExistsByName(trimmedName))
                throw new InvalidOperationException($"Unit name '{trimmedName}' already exists!");

            var validEquivalents = new List<ParameterUnitEquivalent>();
            if (dto.Equivalents != null)
            {
                var order = 0;
                foreach (var eq in dto.Equivalents.Where(e => !string.IsNullOrWhiteSpace(e.Name)))
                {
                    if (eq.ConversionFactor.HasValue && eq.ConversionFactor.Value <= 0)
                        throw new ArgumentException($"Conversion factor for equivalent '{eq.Name}' must be greater than 0.");

                    validEquivalents.Add(new ParameterUnitEquivalent
                    {
                        ID = 0,
                        Name = eq.Name.Trim(),
                        ConversionFactor = eq.ConversionFactor ?? 1.0m,
                        DisplayOrder = ++order,
                        IsActive = true
                    });
                }
            }

            var model = new ParameterUnitMaster
            {
                Code = upperCode,
                Name = trimmedName,
                Symbol = dto.Symbol.Trim(),
                QuantityType = string.IsNullOrWhiteSpace(dto.QuantityType) ? null : dto.QuantityType.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                ConversionFactor = dto.ConversionFactor ?? 1.0m,
                IsActive = dto.IsActive,
                Equivalents = validEquivalents,
                CreatedOn = DateTime.UtcNow,
                CompanyCode = "LIMS"
            };

            await _ParameterUnitRepository.AddParameterUnit(model);
            _logger.LogInformation("ParameterUnit '{ParameterUnitName}' ({Code}) created successfully.", model.Name, model.Code);
        }

        public async Task ModifyParameterUnit(ParameterUnitUpdateDto dto)
        {
            if (dto.ID <= 0)
                throw new ArgumentException("Unit ID is required.");

            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Unit Code is required.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Unit Name is required.");

            if (string.IsNullOrWhiteSpace(dto.Symbol))
                throw new ArgumentException("Unit Symbol is required.");

            if (dto.ConversionFactor.HasValue && dto.ConversionFactor.Value <= 0)
                throw new ArgumentException("Conversion factor must be greater than 0.");

            var upperCode = dto.Code.Trim().ToUpper();
            var trimmedName = dto.Name.Trim();

            if (await _ParameterUnitRepository.ExistsByCodeAndNotId(upperCode, dto.ID))
                throw new InvalidOperationException($"Unit code '{upperCode}' already exists!");

            if (await _ParameterUnitRepository.ExistsByNameAndNotId(trimmedName, dto.ID))
                throw new InvalidOperationException($"Unit name '{trimmedName}' already exists!");

            var existingParameterUnit = await _context.ParameterUnitMasters
                .Include(x => x.Equivalents)
                .FirstOrDefaultAsync(x => x.ID == dto.ID);

            if (existingParameterUnit == null)
                throw new InvalidOperationException("Parameter Unit not found!");

            existingParameterUnit.Code = upperCode;
            existingParameterUnit.Name = trimmedName;
            existingParameterUnit.Symbol = dto.Symbol.Trim();
            existingParameterUnit.QuantityType = string.IsNullOrWhiteSpace(dto.QuantityType) ? null : dto.QuantityType.Trim();
            existingParameterUnit.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            existingParameterUnit.ConversionFactor = dto.ConversionFactor ?? 1.0m;
            existingParameterUnit.IsActive = dto.IsActive;
            existingParameterUnit.ModifiedOn = DateTime.UtcNow;

            var incoming = (dto.Equivalents ?? new List<ParameterUnitEquivalentDto>())
                .Where(e => !string.IsNullOrWhiteSpace(e.Name))
                .ToList();

            var order = 0;
            var incomingIds = new HashSet<long>();

            foreach (var inc in incoming)
            {
                order++;
                if (inc.ConversionFactor.HasValue && inc.ConversionFactor.Value <= 0)
                    throw new ArgumentException($"Conversion factor for equivalent '{inc.Name}' must be greater than 0.");

                if (inc.ID > 0)
                {
                    incomingIds.Add(inc.ID);
                    var match = existingParameterUnit.Equivalents.FirstOrDefault(e => e.ID == inc.ID);
                    if (match != null)
                    {
                        match.Name = inc.Name.Trim();
                        match.ConversionFactor = inc.ConversionFactor ?? 1.0m;
                        match.DisplayOrder = order;
                        match.IsActive = true;
                    }
                    else
                    {
                        existingParameterUnit.Equivalents.Add(new ParameterUnitEquivalent
                        {
                            BaseParameterUnitID = existingParameterUnit.ID,
                            Name = inc.Name.Trim(),
                            ConversionFactor = inc.ConversionFactor ?? 1.0m,
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
                        ConversionFactor = inc.ConversionFactor ?? 1.0m,
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
            _logger.LogInformation("ParameterUnit '{ParameterUnitName}' ({Code}) updated successfully.", existingParameterUnit.Name, existingParameterUnit.Code);
        }

        public async Task<bool> ToggleParameterUnitStatus(long id)
        {
            var unit = await _context.ParameterUnitMasters.FirstOrDefaultAsync(x => x.ID == id);
            if (unit == null)
                throw new InvalidOperationException("Parameter Unit not found!");

            // If currently inactive and will be activated, check collision
            if (!unit.IsActive)
            {
                if (await _ParameterUnitRepository.ExistsByCodeAndNotId(unit.Code, id))
                    throw new InvalidOperationException($"Cannot activate unit because code '{unit.Code}' is already active on another record.");

                if (await _ParameterUnitRepository.ExistsByNameAndNotId(unit.Name, id))
                    throw new InvalidOperationException($"Cannot activate unit because name '{unit.Name}' is already active on another record.");
            }

            unit.IsActive = !unit.IsActive;
            unit.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("ParameterUnit '{ParameterUnitName}' status toggled to {IsActive}.", unit.Name, unit.IsActive);
            return unit.IsActive;
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

        public async Task<ParameterUnitDetailDto> GetParameterUnitDetails(long id)
        {
            var entity = await _context.ParameterUnitMasters
                .Include(x => x.Equivalents)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id);

            if (entity == null)
                throw new InvalidOperationException("ParameterUnit not found!");

            return new ParameterUnitDetailDto
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.Name,
                Symbol = entity.Symbol,
                QuantityType = entity.QuantityType,
                Description = entity.Description,
                ConversionFactor = entity.ConversionFactor,
                IsActive = entity.IsActive,
                Equivalents = entity.Equivalents
                    .Where(e => e.IsActive)
                    .OrderBy(e => e.DisplayOrder)
                    .Select(e => new ParameterUnitEquivalentDto
                    {
                        ID = e.ID,
                        BaseParameterUnitID = e.BaseParameterUnitID,
                        Name = e.Name,
                        ConversionFactor = e.ConversionFactor,
                        DisplayOrder = e.DisplayOrder,
                        IsActive = e.IsActive
                    }).ToList()
            };
        }

        public async Task<List<string>> GetQuantityTypes()
        {
            return await _ParameterUnitRepository.GetQuantityTypes();
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
