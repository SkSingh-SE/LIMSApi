using System.Text.Json;
using System.Text.RegularExpressions;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class MeasurementUncertaintyService : IMeasurementUncertaintyService
    {
        public static readonly HashSet<string> AllowedUncertaintyTypes =
            new(StringComparer.OrdinalIgnoreCase) { "STANDARD", "COMBINED", "EXPANDED" };

        public static readonly HashSet<string> AllowedBasis =
            new(StringComparer.Ordinal) { "Type A", "Type B", "Type A+B", "Combined" };

        public static readonly HashSet<string> AllowedEvalTypes =
            new(StringComparer.Ordinal) { "Type A", "Type B" };

        private readonly IMeasurementUncertaintyRepository _repository;
        private readonly LIMSContext _context;
        private readonly ILogger<MeasurementUncertaintyService> _logger;
        private LoggedInUserDTO loggedInUser;

        public MeasurementUncertaintyService(
            IMeasurementUncertaintyRepository repository,
            LIMSContext context,
            ILogger<MeasurementUncertaintyService> logger)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        private static string NormalizeCode(string code)
        {
            var normalized = code.Trim().ToUpperInvariant();
            normalized = Regex.Replace(normalized, @"\s+", "_");
            return normalized;
        }

        private static string NormalizeUncertaintyType(string value)
        {
            var v = value.Trim().ToUpperInvariant();
            if (v.StartsWith("EXPANDED")) return "EXPANDED";
            if (v.StartsWith("COMBINED")) return "COMBINED";
            if (v.StartsWith("STANDARD")) return "STANDARD";
            return v;
        }

        private static string NormalizeBasis(string value)
        {
            var v = value.Trim();
            if (v.Equals("Type A", StringComparison.OrdinalIgnoreCase)) return "Type A";
            if (v.Equals("Type B", StringComparison.OrdinalIgnoreCase)) return "Type B";
            if (v.Equals("Type A+B", StringComparison.OrdinalIgnoreCase)
                || v.Equals("Type A + B", StringComparison.OrdinalIgnoreCase)
                || v.Equals("Type A/B", StringComparison.OrdinalIgnoreCase)) return "Type A+B";
            if (v.Equals("Combined", StringComparison.OrdinalIgnoreCase)) return "Combined";
            return v;
        }

        public static decimal ComputeCombined(IEnumerable<MeasurementUncertaintyComponentDto> components)
        {
            double sum = 0;
            foreach (var c in components)
            {
                double u = (double)c.StdUncertainty;
                double sens = (double)(c.SensitivityCoefficient == 0 ? 1.0m : c.SensitivityCoefficient);
                sum += u * sens * u * sens;
            }
            return (decimal)Math.Sqrt(sum);
        }

        public static decimal ComputeExpanded(decimal combined, decimal coverageFactor)
        {
            return combined * coverageFactor;
        }

        public static List<MeasurementUncertaintyComponentDto> ParseComponents(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new();
            try
            {
                var list = JsonSerializer.Deserialize<List<MeasurementUncertaintyComponentDto>>(json);
                return list ?? new();
            }
            catch
            {
                return new();
            }
        }

        public static string SerializeComponents(List<MeasurementUncertaintyComponentDto>? components)
        {
            var list = (components ?? new()).Select(c => new MeasurementUncertaintyComponentDto
            {
                Source = (c.Source ?? string.Empty).Trim(),
                EvalType = c.EvalType,
                Distribution = c.Distribution,
                StdUncertainty = c.StdUncertainty,
                SensitivityCoefficient = c.SensitivityCoefficient == 0 ? 1.0m : c.SensitivityCoefficient,
                Contribution = c.StdUncertainty * (c.SensitivityCoefficient == 0 ? 1.0m : c.SensitivityCoefficient)
            }).ToList();
            return JsonSerializer.Serialize(list);
        }

        private async Task ValidateReferences(MeasurementUncertaintyCreateDto dto)
        {
            if (dto.LaboratoryTestID.HasValue)
            {
                var test = await _context.LaboratoryTests
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == dto.LaboratoryTestID.Value && x.CompanyCode == loggedInUser.CompanyCode);
                if (test == null)
                    throw new ArgumentException("Selected laboratory test is invalid or belongs to another company.");
            }

            if (dto.ParameterID.HasValue)
            {
                var param = await _context.ParameterMasters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == dto.ParameterID.Value && x.CompanyCode == loggedInUser.CompanyCode);
                if (param == null)
                    throw new ArgumentException("Selected parameter is invalid or belongs to another company.");
            }

            if (dto.TestMethodSpecificationID.HasValue)
            {
                var method = await _context.TestMethodSpecifications
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == dto.TestMethodSpecificationID.Value && x.CompanyCode == loggedInUser.CompanyCode);
                if (method == null)
                    throw new ArgumentException("Selected test method is invalid or belongs to another company.");

                if (dto.TestMethodSpecificationVersionID.HasValue)
                {
                    var version = await _context.TestMethodSpecificationVersions
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ID == dto.TestMethodSpecificationVersionID.Value);
                    if (version == null)
                        throw new ArgumentException("Selected test method version is invalid.");
                    if (version.TestMethodSpecificationID != dto.TestMethodSpecificationID.Value)
                        throw new ArgumentException("Selected method version does not belong to the selected test method.");
                }
            }
            else if (dto.TestMethodSpecificationVersionID.HasValue)
            {
                throw new ArgumentException("Method version cannot be selected without selecting the test method.");
            }

            if (dto.ParameterUnitID.HasValue)
            {
                var unit = await _context.ParameterUnitMasters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == dto.ParameterUnitID.Value);
                if (unit == null)
                    throw new ArgumentException("Selected measurement unit is invalid.");
            }
        }

        private static void ValidateValues(MeasurementUncertaintyCreateDto dto, string uncertaintyType, string basis)
        {
            if (dto.CoverageFactor <= 0)
                throw new ArgumentException("Coverage factor (k) must be greater than zero.");

            if (dto.ConfidenceLevel.HasValue && (dto.ConfidenceLevel.Value < 0 || dto.ConfidenceLevel.Value > 100))
                throw new ArgumentException("Confidence level must be between 0 and 100.");

            if (dto.CombinedUncertainty.HasValue && dto.CombinedUncertainty.Value < 0)
                throw new ArgumentException("Combined uncertainty cannot be negative.");

            if (dto.ExpandedUncertainty.HasValue && dto.ExpandedUncertainty.Value < 0)
                throw new ArgumentException("Expanded uncertainty cannot be negative.");

            var components = dto.Components ?? new();
            foreach (var c in components)
            {
                if (string.IsNullOrWhiteSpace(c.Source))
                    throw new ArgumentException("Each uncertainty component must have a source.");
                if (!AllowedEvalTypes.Contains(c.EvalType))
                    throw new ArgumentException($"Invalid component evaluation type '{c.EvalType}'. Allowed: Type A, Type B.");
                if (c.StdUncertainty < 0)
                    throw new ArgumentException($"Component '{c.Source}' standard uncertainty cannot be negative.");
                if (c.SensitivityCoefficient < 0)
                    throw new ArgumentException($"Component '{c.Source}' sensitivity coefficient cannot be negative.");
            }

            bool hasCombined = dto.CombinedUncertainty.HasValue;
            bool hasExpanded = dto.ExpandedUncertainty.HasValue;
            if (!hasCombined && !hasExpanded && components.Count == 0)
                throw new ArgumentException("Provide combined uncertainty, expanded uncertainty, or at least one uncertainty component.");
        }

        public async Task CreateUncertainty(MeasurementUncertaintyCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Code is required.");
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.");

            dto.Code = NormalizeCode(dto.Code);
            if (!Regex.IsMatch(dto.Code, @"^[A-Z0-9_]+$"))
                throw new ArgumentException("Code must contain only uppercase letters, digits and underscores.");
            dto.Name = dto.Name.Trim();

            var uncertaintyType = NormalizeUncertaintyType(dto.UncertaintyType ?? "EXPANDED");
            if (!AllowedUncertaintyTypes.Contains(uncertaintyType))
                throw new ArgumentException("Invalid uncertainty type. Allowed: STANDARD, COMBINED, EXPANDED.");
            var basis = NormalizeBasis(dto.Basis ?? "Type B");
            if (!AllowedBasis.Contains(basis))
                throw new ArgumentException("Invalid basis. Allowed: Type A, Type B, Type A+B, Combined.");

            if (await _repository.ExistsByCode(dto.Code))
                throw new ArgumentException($"Uncertainty code '{dto.Code}' already exists.");

            if (await _repository.ExistsCombination(dto.LaboratoryTestID, dto.ParameterID, dto.TestMethodSpecificationID, dto.TestMethodSpecificationVersionID))
                throw new ArgumentException("An active uncertainty configuration already exists for this test, parameter, method and version combination.");

            await ValidateReferences(dto);
            ValidateValues(dto, uncertaintyType, basis);

            var model = new MeasurementUncertaintyMaster
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description?.Trim(),
                LaboratoryTestID = dto.LaboratoryTestID,
                ParameterID = dto.ParameterID,
                TestMethodSpecificationID = dto.TestMethodSpecificationID,
                TestMethodSpecificationVersionID = dto.TestMethodSpecificationVersionID,
                ParameterUnitID = dto.ParameterUnitID,
                UncertaintyType = uncertaintyType,
                Basis = basis,
                CombinedUncertainty = dto.CombinedUncertainty,
                ExpandedUncertainty = dto.ExpandedUncertainty,
                CoverageFactor = dto.CoverageFactor <= 0 ? 2.0m : dto.CoverageFactor,
                ConfidenceLevel = dto.ConfidenceLevel,
                ComponentsJson = SerializeComponents(dto.Components),
                DisplayOrder = dto.DisplayOrder,
                CreatedBy = loggedInUser.EmployeeID,
                CreatedOn = DateTime.UtcNow,
                ModifiedBy = loggedInUser.EmployeeID,
                ModifiedOn = DateTime.UtcNow,
                CompanyCode = loggedInUser.CompanyCode ?? "LIMS",
                IsActive = true
            };

            await _repository.AddUncertainty(model);
        }

        public async Task ModifyUncertainty(MeasurementUncertaintyUpdateDto dto)
        {
            var existing = await _repository.GetUncertaintyById(dto.ID)
                ?? throw new KeyNotFoundException($"Measurement uncertainty {dto.ID} not found.");

            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Code is required.");
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.");

            dto.Code = NormalizeCode(dto.Code);
            if (!Regex.IsMatch(dto.Code, @"^[A-Z0-9_]+$"))
                throw new ArgumentException("Code must contain only uppercase letters, digits and underscores.");
            dto.Name = dto.Name.Trim();

            var uncertaintyType = NormalizeUncertaintyType(dto.UncertaintyType ?? existing.UncertaintyType);
            if (!AllowedUncertaintyTypes.Contains(uncertaintyType))
                throw new ArgumentException("Invalid uncertainty type. Allowed: STANDARD, COMBINED, EXPANDED.");
            var basis = NormalizeBasis(dto.Basis ?? existing.Basis);
            if (!AllowedBasis.Contains(basis))
                throw new ArgumentException("Invalid basis. Allowed: Type A, Type B, Type A+B, Combined.");

            if (await _repository.ExistsByCodeAndNotId(dto.Code, dto.ID))
                throw new ArgumentException($"Uncertainty code '{dto.Code}' already exists.");

            if (await _repository.ExistsCombinationAndNotId(dto.ID, dto.LaboratoryTestID, dto.ParameterID, dto.TestMethodSpecificationID, dto.TestMethodSpecificationVersionID))
                throw new ArgumentException("An active uncertainty configuration already exists for this test, parameter, method and version combination.");

            await ValidateReferences(dto);
            ValidateValues(dto, uncertaintyType, basis);

            existing.Code = dto.Code;
            existing.Name = dto.Name;
            existing.Description = dto.Description?.Trim();
            existing.LaboratoryTestID = dto.LaboratoryTestID;
            existing.ParameterID = dto.ParameterID;
            existing.TestMethodSpecificationID = dto.TestMethodSpecificationID;
            existing.TestMethodSpecificationVersionID = dto.TestMethodSpecificationVersionID;
            existing.ParameterUnitID = dto.ParameterUnitID;
            existing.UncertaintyType = uncertaintyType;
            existing.Basis = basis;
            existing.CombinedUncertainty = dto.CombinedUncertainty;
            existing.ExpandedUncertainty = dto.ExpandedUncertainty;
            existing.CoverageFactor = dto.CoverageFactor <= 0 ? 2.0m : dto.CoverageFactor;
            existing.ConfidenceLevel = dto.ConfidenceLevel;
            existing.ComponentsJson = SerializeComponents(dto.Components);
            existing.DisplayOrder = dto.DisplayOrder;
            existing.IsActive = dto.IsActive;
            existing.ModifiedBy = loggedInUser.EmployeeID;
            existing.ModifiedOn = DateTime.UtcNow;

            await _repository.UpdateUncertainty(existing);
        }

        public async Task RemoveUncertainty(long id)
        {
            var existing = await _repository.GetUncertaintyById(id)
                ?? throw new KeyNotFoundException($"Measurement uncertainty {id} not found.");
            existing.IsActive = false;
            existing.ModifiedBy = loggedInUser.EmployeeID;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repository.UpdateUncertainty(existing);
        }

        public async Task<bool> ToggleUncertaintyStatus(long id)
        {
            var existing = await _context.MeasurementUncertaintyMasters
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode)
                ?? throw new KeyNotFoundException($"Measurement uncertainty {id} not found.");

            existing.IsActive = !existing.IsActive;

            if (existing.IsActive)
            {
                if (await _repository.ExistsByCodeAndNotId(existing.Code, existing.ID))
                    throw new ArgumentException($"Cannot reactivate: uncertainty code '{existing.Code}' already exists.");
                if (await _repository.ExistsCombinationAndNotId(existing.ID, existing.LaboratoryTestID, existing.ParameterID, existing.TestMethodSpecificationID, existing.TestMethodSpecificationVersionID))
                    throw new ArgumentException("Cannot reactivate: an active uncertainty configuration already exists for this test, parameter, method and version combination.");
            }

            existing.ModifiedBy = loggedInUser.EmployeeID;
            existing.ModifiedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existing.IsActive;
        }

        public async Task<MeasurementUncertaintyDetailDto> GetUncertaintyDetails(long id)
        {
            var e = await _repository.GetUncertaintyById(id)
                ?? throw new KeyNotFoundException($"Measurement uncertainty {id} not found.");

            var components = ParseComponents(e.ComponentsJson);

            var createdByName = await _context.EmployeeMasters
                .AsNoTracking()
                .Where(x => x.ID == e.CreatedBy)
                .Select(x => x.Name)
                .FirstOrDefaultAsync() ?? "-";
            var modifiedByName = e.ModifiedBy.HasValue
                ? await _context.EmployeeMasters
                    .AsNoTracking()
                    .Where(x => x.ID == e.ModifiedBy.Value)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync() ?? "-"
                : "-";

            return new MeasurementUncertaintyDetailDto
            {
                ID = e.ID,
                Code = e.Code,
                Name = e.Name,
                Description = e.Description,
                LaboratoryTestID = e.LaboratoryTestID,
                LaboratoryTestName = e.LaboratoryTest?.Name,
                ParameterID = e.ParameterID,
                ParameterName = e.Parameter?.Name,
                ParameterUnit = e.ParameterUnit?.Symbol,
                TestMethodSpecificationID = e.TestMethodSpecificationID,
                TestMethodName = e.TestMethodSpecification?.Name,
                TestMethodSpecificationVersionID = e.TestMethodSpecificationVersionID,
                TestMethodVersion = e.TestMethodSpecificationVersion?.Version,
                ParameterUnitID = e.ParameterUnitID,
                UnitSymbol = e.ParameterUnit?.Symbol,
                UncertaintyType = e.UncertaintyType,
                Basis = e.Basis,
                CombinedUncertainty = e.CombinedUncertainty,
                ExpandedUncertainty = e.ExpandedUncertainty,
                CoverageFactor = e.CoverageFactor,
                ConfidenceLevel = e.ConfidenceLevel,
                Components = components,
                SummaryText = MeasurementUncertaintyRepository.BuildSummaryText(e.UncertaintyType, e.ExpandedUncertainty, e.CombinedUncertainty, e.CoverageFactor, e.ParameterUnit?.Symbol),
                DisplayOrder = e.DisplayOrder,
                IsActive = e.IsActive,
                CompanyCode = e.CompanyCode,
                CreatedByName = createdByName,
                CreatedOn = e.CreatedOn,
                ModifiedByName = modifiedByName,
                ModifiedOn = e.ModifiedOn
            };
        }

        public async Task<PagedResponse<object>> FetchUncertaintyList(MeasurementUncertaintyListRequest request)
        {
            return await _repository.GetPagedUncertainties(request ?? new MeasurementUncertaintyListRequest());
        }

        public async Task<List<DropdwonSelector>> GetUncertaintyDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetUncertaintyDropdown(searchTerm, pageNo, pageSize);
        }

        public Task<MeasurementUncertaintyValidateResponse> ValidateUncertainty(MeasurementUncertaintyValidateRequest request)
        {
            if (request == null) throw new ArgumentException("Validation request is required.");
            if (request.CoverageFactor <= 0)
                throw new ArgumentException("Coverage factor (k) must be greater than zero.");

            var components = (request.Components ?? new()).Select(c => new MeasurementUncertaintyComponentDto
            {
                Source = (c.Source ?? string.Empty).Trim(),
                EvalType = string.IsNullOrWhiteSpace(c.EvalType) ? "Type B" : c.EvalType.Trim(),
                Distribution = c.Distribution,
                StdUncertainty = c.StdUncertainty,
                SensitivityCoefficient = c.SensitivityCoefficient == 0 ? 1.0m : c.SensitivityCoefficient,
                Contribution = c.StdUncertainty * (c.SensitivityCoefficient == 0 ? 1.0m : c.SensitivityCoefficient)
            }).ToList();

            decimal combined;
            string operation;
            if (components.Count > 0)
            {
                combined = ComputeCombined(components);
                operation = $"uc = √Σ(uᵢ×cᵢ)² over {components.Count} component(s); U = k × uc";
            }
            else if (request.CombinedUncertainty.HasValue)
            {
                if (request.CombinedUncertainty.Value < 0)
                    throw new ArgumentException("Combined uncertainty cannot be negative.");
                combined = request.CombinedUncertainty.Value;
                operation = "U = k × uc (stored combined uncertainty)";
            }
            else
            {
                throw new ArgumentException("Provide combined uncertainty or at least one component for validation.");
            }

            var expanded = ComputeExpanded(combined, request.CoverageFactor);

            return Task.FromResult(new MeasurementUncertaintyValidateResponse
            {
                CombinedUncertainty = Math.Round(combined, 6),
                CoverageFactor = request.CoverageFactor,
                ExpandedUncertainty = Math.Round(expanded, 6),
                AppliedOperation = operation,
                Components = components
            });
        }
    }
}
