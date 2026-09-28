using System.Text.RegularExpressions;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class AcceptanceCriteriaService : IAcceptanceCriteriaService
    {
        private readonly IAcceptanceCriteriaRepository _repository;
        private readonly LIMSContext _context;
        private readonly ILogger<AcceptanceCriteriaService> _logger;
        private LoggedInUserDTO loggedInUser;

        public static readonly HashSet<string> AllowedEvaluationTypes =
            new(StringComparer.OrdinalIgnoreCase) { "TEST", "PARAMETER" };

        public static readonly HashSet<string> AllowedComparisonTypes =
            new(StringComparer.OrdinalIgnoreCase) { "RANGE", "GE", "LE", "BETWEEN", "EQUAL", "TARGET_TOLERANCE" };

        public static readonly HashSet<string> AllowedDecisionRules =
            new(StringComparer.OrdinalIgnoreCase) { "ALL_REQUIRED_PASS", "WITH_MOU_GUARD", "INFORMATIONAL" };

        public static readonly HashSet<string> AllowedRoundingRules =
            new(StringComparer.OrdinalIgnoreCase) { "ROUND_NEAREST", "FLOOR", "CEILING", "TRUNCATE" };

        public AcceptanceCriteriaService(IAcceptanceCriteriaRepository repository, LIMSContext context, ILogger<AcceptanceCriteriaService> logger)
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

        private static void ValidateRuleFields(string evaluationType, string comparisonType, string decisionRule, string roundingRule)
        {
            if (string.IsNullOrWhiteSpace(evaluationType) || !AllowedEvaluationTypes.Contains(evaluationType.Trim()))
                throw new ArgumentException("Evaluation type must be one of: TEST, PARAMETER.");

            if (string.IsNullOrWhiteSpace(comparisonType) || !AllowedComparisonTypes.Contains(comparisonType.Trim()))
                throw new ArgumentException("Comparison type must be one of: RANGE, GE, LE, BETWEEN, EQUAL, TARGET_TOLERANCE.");

            if (string.IsNullOrWhiteSpace(decisionRule) || !AllowedDecisionRules.Contains(decisionRule.Trim()))
                throw new ArgumentException("Decision rule must be one of: ALL_REQUIRED_PASS, WITH_MOU_GUARD, INFORMATIONAL.");

            if (string.IsNullOrWhiteSpace(roundingRule) || !AllowedRoundingRules.Contains(roundingRule.Trim()))
                throw new ArgumentException("Rounding rule must be one of: ROUND_NEAREST, FLOOR, CEILING, TRUNCATE.");
        }

        public async Task CreateAcceptanceCriteria(AcceptanceCriteriaMasterCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Acceptance criteria code should not be empty!");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Acceptance criteria name should not be empty!");

            var code = NormalizeCode(dto.Code);
            if (!Regex.IsMatch(code, @"^[A-Z0-9_]+$"))
                throw new ArgumentException("Acceptance criteria code may contain only letters, numbers and underscore (_).");

            ValidateRuleFields(dto.EvaluationType, dto.ComparisonType, dto.DecisionRule, dto.RoundingRule);

            if (await _repository.ExistsByCode(code))
                throw new InvalidOperationException($"Acceptance criteria code '{code}' already exists!");

            if (await _repository.ExistsByName(dto.Name.Trim()))
                throw new InvalidOperationException("Acceptance criteria name already exists!");

            var model = new AcceptanceCriteriaMaster
            {
                Code = code,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                EvaluationType = dto.EvaluationType.Trim().ToUpperInvariant(),
                ComparisonType = dto.ComparisonType.Trim().ToUpperInvariant(),
                DecisionRule = dto.DecisionRule.Trim().ToUpperInvariant(),
                RoundingRule = dto.RoundingRule.Trim().ToUpperInvariant(),
                DisplayOrder = dto.DisplayOrder,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = loggedInUser.EmployeeID,
                CompanyCode = loggedInUser.CompanyCode,
                IsActive = true
            };

            await _repository.AddAcceptanceCriteria(model);
            _logger.LogInformation("Acceptance criteria '{Name}' created successfully.", model.Name);
        }

        public async Task ModifyAcceptanceCriteria(AcceptanceCriteriaMasterUpdateDto dto)
        {
            if (dto.ID == 0)
                throw new ArgumentException("Acceptance criteria ID should not be empty!");

            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Acceptance criteria code should not be empty!");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Acceptance criteria name should not be empty!");

            var code = NormalizeCode(dto.Code);
            if (!Regex.IsMatch(code, @"^[A-Z0-9_]+$"))
                throw new ArgumentException("Acceptance criteria code may contain only letters, numbers and underscore (_).");

            ValidateRuleFields(dto.EvaluationType, dto.ComparisonType, dto.DecisionRule, dto.RoundingRule);

            if (await _repository.ExistsByCodeAndNotId(code, dto.ID))
                throw new InvalidOperationException($"Acceptance criteria code '{code}' already exists!");

            if (await _repository.ExistsByNameAndNotId(dto.Name.Trim(), dto.ID))
                throw new InvalidOperationException("Acceptance criteria name already exists!");

            var existing = await _context.AcceptanceCriteriaMasters
                .FirstOrDefaultAsync(x => x.ID == dto.ID && x.CompanyCode == loggedInUser.CompanyCode);

            if (existing == null)
                throw new InvalidOperationException("Acceptance criteria not found!");

            existing.Code = code;
            existing.Name = dto.Name.Trim();
            existing.Description = dto.Description?.Trim();
            existing.EvaluationType = dto.EvaluationType.Trim().ToUpperInvariant();
            existing.ComparisonType = dto.ComparisonType.Trim().ToUpperInvariant();
            existing.DecisionRule = dto.DecisionRule.Trim().ToUpperInvariant();
            existing.RoundingRule = dto.RoundingRule.Trim().ToUpperInvariant();
            existing.DisplayOrder = dto.DisplayOrder;
            existing.IsActive = dto.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateAcceptanceCriteria(existing);
            _logger.LogInformation("Acceptance criteria '{Name}' updated successfully.", existing.Name);
        }

        public async Task RemoveAcceptanceCriteria(long id)
        {
            var existing = await _context.AcceptanceCriteriaMasters
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);

            if (existing == null)
                throw new InvalidOperationException("Acceptance criteria not found!");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateAcceptanceCriteria(existing);
            _logger.LogInformation("Acceptance criteria with ID '{Id}' deactivated successfully.", id);
        }

        public async Task<bool> ToggleAcceptanceCriteriaStatus(long id)
        {
            var existing = await _context.AcceptanceCriteriaMasters
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);

            if (existing == null)
                throw new InvalidOperationException("Acceptance criteria not found!");

            if (!existing.IsActive && !string.IsNullOrWhiteSpace(existing.Code))
            {
                if (await _repository.ExistsByCodeAndNotId(existing.Code.Trim(), existing.ID))
                    throw new InvalidOperationException($"Cannot activate: Acceptance criteria code '{existing.Code.Trim()}' is already in use by another active criteria.");
            }

            existing.IsActive = !existing.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateAcceptanceCriteria(existing);
            _logger.LogInformation("Acceptance criteria '{Name}' status toggled to {Status}.", existing.Name, existing.IsActive ? "Active" : "Inactive");
            return existing.IsActive;
        }

        public async Task<AcceptanceCriteriaMasterDto> GetAcceptanceCriteriaDetails(long id)
        {
            var entity = await _repository.GetAcceptanceCriteriaById(id);
            if (entity == null)
                throw new InvalidOperationException("Acceptance criteria not found!");

            return new AcceptanceCriteriaMasterDto
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                EvaluationType = entity.EvaluationType,
                ComparisonType = entity.ComparisonType,
                DecisionRule = entity.DecisionRule,
                RoundingRule = entity.RoundingRule,
                DisplayOrder = entity.DisplayOrder,
                IsActive = entity.IsActive,
                CompanyCode = entity.CompanyCode
            };
        }

        public async Task<PagedResponse<object>> FetchAcceptanceCriteriaList(PageFilter filter)
        {
            return await _repository.GetAllAcceptanceCriteria(filter);
        }

        public async Task<List<DropdwonSelector>> GetAcceptanceCriteriaDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetAcceptanceCriteriaDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
