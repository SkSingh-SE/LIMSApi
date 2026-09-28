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
    public class FactorConversionService : IFactorConversionService
    {
        // Review-frozen (Phase 1E): exactly the four backend-supported operations.
        // MULTIPLICATION: Output = Input × Factor (dilutions are multipliers by intent, e.g. 10x).
        // DIVISION: Output = Input ÷ Factor (Factor ≠ 0).
        // ADDITIVE_OFFSET: Output = Input + Offset. SUBTRACTIVE_OFFSET: Output = Input − Offset.
        public static readonly HashSet<string> AllowedFactorTypes =
            new(StringComparer.OrdinalIgnoreCase) { "MULTIPLICATION", "DIVISION", "ADDITIVE_OFFSET", "SUBTRACTIVE_OFFSET" };

        // Review-frozen: unambiguous pipeline stage. Measured Value = normalize the raw
        // observation before formula evaluation (default). Reported Value = apply at reporting.
        public static readonly HashSet<string> AllowedAppliedOn =
            new(StringComparer.OrdinalIgnoreCase) { "Measured Value", "Reported Value" };

        private readonly IFactorConversionRepository _repository;
        private readonly LIMSContext _context;
        private readonly ILogger<FactorConversionService> _logger;
        private LoggedInUserDTO loggedInUser;

        public FactorConversionService(
            IFactorConversionRepository repository,
            LIMSContext context,
            ILogger<FactorConversionService> logger)
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

        public static decimal ApplyTransformation(string factorType, decimal input, decimal value)
        {
            return factorType.ToUpperInvariant() switch
            {
                "DIVISION" => input / value,
                "ADDITIVE_OFFSET" => input + value,
                "SUBTRACTIVE_OFFSET" => input - value,
                _ => input * value
            };
        }

        public static string DescribeOperation(string factorType, decimal value)
        {
            return factorType.ToUpperInvariant() switch
            {
                "DIVISION" => $"÷ {value}",
                "ADDITIVE_OFFSET" => $"+ ({value})",
                "SUBTRACTIVE_OFFSET" => $"− ({value})",
                _ => $"× {value}"
            };
        }

        private async Task ValidateReferences(FactorConversionCreateDto dto, string factorType)
        {
            var input = await _context.ParameterMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == dto.InputParameterID && x.CompanyCode == loggedInUser.CompanyCode);
            if (input == null)
                throw new ArgumentException("Selected input parameter is invalid or belongs to another company.");

            if (dto.OutputParameterID.HasValue)
            {
                if (dto.OutputParameterID.Value == dto.InputParameterID)
                    throw new ArgumentException("Output parameter must be different from input parameter, or left empty to reuse the input parameter.");
                var output = await _context.ParameterMasters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == dto.OutputParameterID.Value && x.CompanyCode == loggedInUser.CompanyCode);
                if (output == null)
                    throw new ArgumentException("Selected output parameter is invalid or belongs to another company.");
            }

            if (dto.LaboratoryTestID.HasValue)
            {
                var test = await _context.LaboratoryTests
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == dto.LaboratoryTestID.Value && x.CompanyCode == loggedInUser.CompanyCode);
                if (test == null)
                    throw new ArgumentException("Selected laboratory test is invalid or belongs to another company.");
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
                throw new ArgumentException("Method version requires a test method to be selected.");
            }

            if (factorType == "DIVISION" && dto.FactorValue == 0)
                throw new ArgumentException("Division factor value must not be zero.");
        }

        public async Task CreateFactor(FactorConversionCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Factor code should not be empty!");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Factor name should not be empty!");

            var code = NormalizeCode(dto.Code);
            if (!Regex.IsMatch(code, @"^[A-Z0-9_]+$"))
                throw new ArgumentException("Factor code may contain only letters, numbers and underscore (_).");

            var factorType = (dto.FactorType ?? "MULTIPLICATION").Trim().ToUpperInvariant();
            if (!AllowedFactorTypes.Contains(factorType))
                throw new ArgumentException("Factor type must be one of: MULTIPLICATION, DIVISION, ADDITIVE_OFFSET, SUBTRACTIVE_OFFSET.");

            var appliedOn = string.IsNullOrWhiteSpace(dto.AppliedOn) ? "Measured Value" : dto.AppliedOn.Trim();
            if (!AllowedAppliedOn.Contains(appliedOn))
                throw new ArgumentException("Applied on must be one of: Measured Value, Reported Value.");

            await ValidateReferences(dto, factorType);

            if (await _repository.ExistsByCode(code))
                throw new InvalidOperationException($"Factor code '{code}' already exists!");

            if (await _repository.ExistsCombination(dto.InputParameterID, factorType, dto.LaboratoryTestID, dto.TestMethodSpecificationID, dto.TestMethodSpecificationVersionID))
                throw new InvalidOperationException("An active factor already exists for this input parameter, type, test, method and version combination.");

            var model = new FactorConversionMaster
            {
                Code = code,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                FactorType = factorType,
                FactorValue = dto.FactorValue,
                InputParameterID = dto.InputParameterID,
                OutputParameterID = dto.OutputParameterID,
                LaboratoryTestID = dto.LaboratoryTestID,
                TestMethodSpecificationID = dto.TestMethodSpecificationID,
                TestMethodSpecificationVersionID = dto.TestMethodSpecificationVersionID,
                AppliedOn = appliedOn,
                IsMandatory = dto.IsMandatory,
                DisplayOrder = dto.DisplayOrder,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = loggedInUser.EmployeeID,
                ModifiedOn = DateTime.UtcNow,
                ModifiedBy = loggedInUser.EmployeeID,
                CompanyCode = loggedInUser.CompanyCode,
                IsActive = true
            };

            await _repository.AddFactor(model);
            _logger.LogInformation("Factor/conversion '{Name}' created successfully.", model.Name);
        }

        public async Task ModifyFactor(FactorConversionUpdateDto dto)
        {
            if (dto.ID == 0)
                throw new ArgumentException("Factor ID should not be empty!");

            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Factor code should not be empty!");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Factor name should not be empty!");

            var code = NormalizeCode(dto.Code);
            if (!Regex.IsMatch(code, @"^[A-Z0-9_]+$"))
                throw new ArgumentException("Factor code may contain only letters, numbers and underscore (_).");

            var factorType = (dto.FactorType ?? "MULTIPLICATION").Trim().ToUpperInvariant();
            if (!AllowedFactorTypes.Contains(factorType))
                throw new ArgumentException("Factor type must be one of: MULTIPLICATION, DIVISION, ADDITIVE_OFFSET, SUBTRACTIVE_OFFSET.");

            var appliedOn = string.IsNullOrWhiteSpace(dto.AppliedOn) ? "Measured Value" : dto.AppliedOn.Trim();
            if (!AllowedAppliedOn.Contains(appliedOn))
                throw new ArgumentException("Applied on must be one of: Measured Value, Reported Value.");

            await ValidateReferences(dto, factorType);

            if (await _repository.ExistsByCodeAndNotId(code, dto.ID))
                throw new InvalidOperationException($"Factor code '{code}' already exists!");

            if (await _repository.ExistsCombinationAndNotId(dto.ID, dto.InputParameterID, factorType, dto.LaboratoryTestID, dto.TestMethodSpecificationID, dto.TestMethodSpecificationVersionID))
                throw new InvalidOperationException("An active factor already exists for this input parameter, type, test, method and version combination.");

            var existing = await _context.FactorConversionMasters
                .FirstOrDefaultAsync(x => x.ID == dto.ID && x.CompanyCode == loggedInUser.CompanyCode);

            if (existing == null)
                throw new InvalidOperationException("Factor/conversion not found!");

            existing.Code = code;
            existing.Name = dto.Name.Trim();
            existing.Description = dto.Description?.Trim();
            existing.FactorType = factorType;
            existing.FactorValue = dto.FactorValue;
            existing.InputParameterID = dto.InputParameterID;
            existing.OutputParameterID = dto.OutputParameterID;
            existing.LaboratoryTestID = dto.LaboratoryTestID;
            existing.TestMethodSpecificationID = dto.TestMethodSpecificationID;
            existing.TestMethodSpecificationVersionID = dto.TestMethodSpecificationVersionID;
            existing.AppliedOn = appliedOn;
            existing.IsMandatory = dto.IsMandatory;
            existing.DisplayOrder = dto.DisplayOrder;
            existing.IsActive = dto.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateFactor(existing);
            _logger.LogInformation("Factor/conversion '{Name}' updated successfully.", existing.Name);
        }

        public async Task RemoveFactor(long id)
        {
            var existing = await _context.FactorConversionMasters
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);

            if (existing == null)
                throw new InvalidOperationException("Factor/conversion not found!");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateFactor(existing);
            _logger.LogInformation("Factor/conversion with ID '{Id}' deactivated successfully.", id);
        }

        public async Task<bool> ToggleFactorStatus(long id)
        {
            var existing = await _context.FactorConversionMasters
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);

            if (existing == null)
                throw new InvalidOperationException("Factor/conversion not found!");

            if (!existing.IsActive)
            {
                if (!string.IsNullOrWhiteSpace(existing.Code)
                    && await _repository.ExistsByCodeAndNotId(existing.Code.Trim(), existing.ID))
                    throw new InvalidOperationException($"Cannot activate: factor code '{existing.Code.Trim()}' is already in use by another active factor.");
                if (await _repository.ExistsCombinationAndNotId(existing.ID, existing.InputParameterID, existing.FactorType, existing.LaboratoryTestID, existing.TestMethodSpecificationID, existing.TestMethodSpecificationVersionID))
                    throw new InvalidOperationException("Cannot activate: another active factor already covers this input parameter, type, test, method and version combination.");
                if (existing.FactorType == "DIVISION" && existing.FactorValue == 0)
                    throw new InvalidOperationException("Cannot activate: division factor value must not be zero.");
            }

            existing.IsActive = !existing.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateFactor(existing);
            _logger.LogInformation("Factor/conversion '{Name}' status toggled to {Status}.", existing.Name, existing.IsActive ? "Active" : "Inactive");
            return existing.IsActive;
        }

        public async Task<FactorConversionDetailDto> GetFactorDetails(long id)
        {
            var entity = await _repository.GetFactorById(id);
            if (entity == null)
                throw new InvalidOperationException("Factor/conversion not found!");

            var modName = await ResolveEmployeeName(entity.ModifiedBy, entity.CreatedBy);
            var creName = await ResolveEmployeeName(entity.CreatedBy, null);

            string? inputUnit = null;
            if (entity.InputParameter?.ParameterUnitID != null)
            {
                var unit = await _context.ParameterUnitMasters.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == entity.InputParameter.ParameterUnitID.Value);
                inputUnit = unit != null ? $"{unit.Name} ({unit.Symbol})" : null;
            }

            string? outputUnit = null;
            if (entity.OutputParameter?.ParameterUnitID != null)
            {
                var unit = await _context.ParameterUnitMasters.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == entity.OutputParameter.ParameterUnitID.Value);
                outputUnit = unit != null ? $"{unit.Name} ({unit.Symbol})" : null;
            }

            return new FactorConversionDetailDto
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                FactorType = entity.FactorType,
                FactorValue = entity.FactorValue,
                InputParameterID = entity.InputParameterID,
                InputParameterName = entity.InputParameter?.Name ?? "-",
                InputParameterUnit = inputUnit,
                OutputParameterID = entity.OutputParameterID,
                OutputParameterName = entity.OutputParameter?.Name,
                OutputParameterUnit = outputUnit,
                LaboratoryTestID = entity.LaboratoryTestID,
                LaboratoryTestName = entity.LaboratoryTest?.Name,
                TestMethodSpecificationID = entity.TestMethodSpecificationID,
                TestMethodName = entity.TestMethodSpecification?.Name,
                TestMethodSpecificationVersionID = entity.TestMethodSpecificationVersionID,
                TestMethodVersion = entity.TestMethodSpecificationVersion?.Version,
                AppliedOn = entity.AppliedOn,
                TransformationText = FactorConversionRepository.BuildTransformationText(
                    entity.FactorType, entity.FactorValue,
                    entity.InputParameter?.Name ?? "-", entity.OutputParameter?.Name),
                IsMandatory = entity.IsMandatory,
                DisplayOrder = entity.DisplayOrder,
                IsActive = entity.IsActive,
                CompanyCode = entity.CompanyCode,
                CreatedByName = creName,
                CreatedOn = entity.CreatedOn,
                ModifiedByName = modName,
                ModifiedOn = entity.ModifiedOn
            };
        }

        private async Task<string> ResolveEmployeeName(long? employeeId, long? fallbackId)
        {
            var id = employeeId ?? fallbackId;
            if (!id.HasValue || id.Value == 0)
                return "-";
            var emp = await _context.EmployeeMasters.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id.Value);
            return emp?.Name ?? "-";
        }

        public async Task<PagedResponse<object>> FetchFactorList(FactorConversionListRequest request)
        {
            return await _repository.GetPagedFactors(request ?? new FactorConversionListRequest());
        }

        public async Task<List<DropdwonSelector>> GetFactorDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetFactorDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<FactorValidateResponse> ValidateFactor(FactorValidateRequest request)
        {
            string factorType;
            decimal factorValue;

            if (request.FactorID.HasValue && request.FactorID.Value > 0)
            {
                var entity = await _context.FactorConversionMasters.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == request.FactorID.Value && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
                if (entity == null)
                    throw new InvalidOperationException("Factor/conversion not found!");
                factorType = entity.FactorType;
                factorValue = entity.FactorValue;
            }
            else
            {
                factorType = (request.FactorType ?? "MULTIPLICATION").Trim().ToUpperInvariant();
                if (!AllowedFactorTypes.Contains(factorType))
                    throw new ArgumentException("Factor type must be one of: MULTIPLICATION, DIVISION, ADDITIVE_OFFSET, SUBTRACTIVE_OFFSET.");
                factorValue = request.FactorValue ?? 1.0m;
            }

            if (factorType == "DIVISION" && factorValue == 0)
                throw new ArgumentException("Division factor value must not be zero.");

            var output = ApplyTransformation(factorType, request.InputValue, factorValue);

            return new FactorValidateResponse
            {
                FactorType = factorType,
                FactorValue = factorValue,
                InputValue = request.InputValue,
                OutputValue = output,
                AppliedOperation = DescribeOperation(factorType, factorValue)
            };
        }
    }
}
