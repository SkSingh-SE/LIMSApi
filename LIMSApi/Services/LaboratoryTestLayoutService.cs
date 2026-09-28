using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class LaboratoryTestLayoutService : ILaboratoryTestLayoutService
    {
        private readonly ILaboratoryTestLayoutRepository _repository;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public LaboratoryTestLayoutService(ILaboratoryTestLayoutRepository repository, LIMSContext context)
        {
            _repository = repository;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        private async Task<LaboratoryTest> RequireTestAsync(long testId)
        {
            var t = await _context.LaboratoryTests.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == testId && x.CompanyCode == loggedInUser.CompanyCode);
            if (t == null) throw new KeyNotFoundException($"Laboratory Test ID {testId} not found.");
            if (!t.IsActive) throw new InvalidOperationException($"Laboratory Test '{t.Code}' is inactive.");
            return t;
        }

        private async Task<ExecutionLayoutMaster> RequireLayoutAsync(long layoutId)
        {
            var l = await _context.ExecutionLayoutMasters.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == layoutId && x.CompanyCode == loggedInUser.CompanyCode);
            if (l == null) throw new KeyNotFoundException($"Execution Layout ID {layoutId} not found.");
            if (!l.IsActive) throw new InvalidOperationException($"Execution Layout '{l.Code}' is inactive.");
            return l;
        }

        private async Task ValidateMethodScopeAsync(long testId, long? methodId, long? versionId)
        {
            if (versionId != null && methodId == null)
                throw new ArgumentException("Method Version requires Method (MethodVersionID != NULL implies MethodID != NULL).");
            if (methodId == null) return;
            var m = await _context.TestMethodSpecifications.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == methodId.Value && x.CompanyCode == loggedInUser.CompanyCode);
            if (m == null) throw new KeyNotFoundException($"Test Method ID {methodId} not found.");
            if (m.IsDisabled) throw new InvalidOperationException($"Test Method '{m.Code}' is inactive.");
            if (versionId != null)
            {
                var v = await _context.TestMethodSpecificationVersions.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == versionId.Value);
                if (v == null) throw new KeyNotFoundException($"Method Version ID {versionId} not found.");
                if (v.TestMethodSpecificationID != methodId.Value)
                    throw new ArgumentException($"Method Version ID {versionId} does not belong to Method ID {methodId}.");
                if (v.Status != VersionStatus.Active)
                    throw new InvalidOperationException($"Method Version '{v.Version}' (ID {versionId}) is not Active.");
            }
            var testMethod = await _context.LaboratoryTestMethods.AsNoTracking()
                .FirstOrDefaultAsync(x => x.LaboratoryTestID == testId
                    && x.TestMethodSpecificationID == methodId.Value
                    && (versionId == null || x.TestMethodSpecificationVersionID == null || x.TestMethodSpecificationVersionID == versionId)
                    && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (testMethod == null)
                throw new InvalidOperationException($"Method ID {methodId} is not configured for Laboratory Test ID {testId}.");
        }

        private static LaboratoryTestLayoutItemDto MapItem(LaboratoryTestLayout x) => new()
        {
            ID = x.ID, LaboratoryTestID = x.LaboratoryTestID, ExecutionLayoutID = x.ExecutionLayoutID,
            LayoutCode = x.ExecutionLayout?.Code ?? string.Empty, LayoutName = x.ExecutionLayout?.Name ?? string.Empty,
            RendererType = x.ExecutionLayout?.RendererType,
            TestMethodSpecificationID = x.TestMethodSpecificationID,
            MethodName = x.TestMethodSpecification?.Code ?? x.TestMethodSpecification?.Name,
            TestMethodSpecificationVersionID = x.TestMethodSpecificationVersionID,
            VersionName = x.TestMethodSpecificationVersion?.Version,
            Priority = x.Priority, IsDefault = x.IsDefault, IsActive = x.IsActive
        };

        public async Task<List<LaboratoryTestLayoutItemDto>> GetLayoutsAsync(long testId)
        {
            await RequireTestAsync(testId);
            var rows = await _repository.GetByTestAsync(testId, loggedInUser.CompanyCode);
            return rows.Select(MapItem).ToList();
        }

        public async Task<LaboratoryTestLayoutItemDto> AddLayoutAsync(long testId, LaboratoryTestLayoutCreateDto dto)
        {
            await RequireTestAsync(testId);
            await RequireLayoutAsync(dto.ExecutionLayoutID);
            await ValidateMethodScopeAsync(testId, dto.TestMethodSpecificationID, dto.TestMethodSpecificationVersionID);
            if (await _repository.ExistsDuplicateAsync(testId, dto.ExecutionLayoutID, dto.TestMethodSpecificationID, dto.TestMethodSpecificationVersionID, 0, loggedInUser.CompanyCode))
                throw new InvalidOperationException("Duplicate layout assignment for this test, method and version scope.");
            if (dto.IsDefault && await _repository.ExistsDefaultInScopeAsync(testId, dto.TestMethodSpecificationID, dto.TestMethodSpecificationVersionID, 0, loggedInUser.CompanyCode))
                throw new InvalidOperationException("An active default layout already exists in this assignment scope.");
            var model = new LaboratoryTestLayout
            {
                LaboratoryTestID = testId, ExecutionLayoutID = dto.ExecutionLayoutID,
                TestMethodSpecificationID = dto.TestMethodSpecificationID,
                TestMethodSpecificationVersionID = dto.TestMethodSpecificationVersionID,
                Priority = dto.Priority, IsDefault = dto.IsDefault,
                CompanyCode = loggedInUser.CompanyCode, IsActive = true,
                CreatedBy = loggedInUser.EmployeeID, CreatedOn = DateTime.UtcNow
            };
            await _repository.AddAsync(model);
            var rows = await _repository.GetByTestAsync(testId, loggedInUser.CompanyCode);
            return MapItem(rows.First(x => x.ID == model.ID));
        }

        public async Task<LaboratoryTestLayoutItemDto> UpdateLayoutAsync(long testId, long assignmentId, LaboratoryTestLayoutUpdateDto dto)
        {
            await RequireTestAsync(testId);
            var existing = await _repository.GetByIdAsync(assignmentId, loggedInUser.CompanyCode)
                ?? throw new KeyNotFoundException($"Layout assignment ID {assignmentId} not found.");
            if (existing.LaboratoryTestID != testId)
                throw new InvalidOperationException($"Assignment ID {assignmentId} does not belong to Laboratory Test ID {testId}.");
            await RequireLayoutAsync(dto.ExecutionLayoutID);
            await ValidateMethodScopeAsync(testId, dto.TestMethodSpecificationID, dto.TestMethodSpecificationVersionID);
            if (await _repository.ExistsDuplicateAsync(testId, dto.ExecutionLayoutID, dto.TestMethodSpecificationID, dto.TestMethodSpecificationVersionID, assignmentId, loggedInUser.CompanyCode))
                throw new InvalidOperationException("Duplicate layout assignment for this test, method and version scope.");
            if (dto.IsDefault && await _repository.ExistsDefaultInScopeAsync(testId, dto.TestMethodSpecificationID, dto.TestMethodSpecificationVersionID, assignmentId, loggedInUser.CompanyCode))
                throw new InvalidOperationException("An active default layout already exists in this assignment scope.");
            existing.ExecutionLayoutID = dto.ExecutionLayoutID;
            existing.TestMethodSpecificationID = dto.TestMethodSpecificationID;
            existing.TestMethodSpecificationVersionID = dto.TestMethodSpecificationVersionID;
            existing.Priority = dto.Priority;
            existing.IsDefault = dto.IsDefault;
            existing.ModifiedBy = loggedInUser.EmployeeID;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repository.UpdateAsync(existing);
            var rows = await _repository.GetByTestAsync(testId, loggedInUser.CompanyCode);
            return MapItem(rows.First(x => x.ID == existing.ID));
        }

        public async Task<bool> ToggleLayoutStatusAsync(long testId, long assignmentId)
        {
            await RequireTestAsync(testId);
            var existing = await _repository.GetByIdAsync(assignmentId, loggedInUser.CompanyCode)
                ?? throw new KeyNotFoundException($"Layout assignment ID {assignmentId} not found.");
            if (existing.LaboratoryTestID != testId)
                throw new InvalidOperationException($"Assignment ID {assignmentId} does not belong to Laboratory Test ID {testId}.");
            existing.IsActive = !existing.IsActive;
            if (existing.IsActive && await _repository.ExistsDuplicateAsync(testId, existing.ExecutionLayoutID, existing.TestMethodSpecificationID, existing.TestMethodSpecificationVersionID, assignmentId, loggedInUser.CompanyCode))
                throw new InvalidOperationException("Reactivating this assignment would duplicate another active assignment in the same scope.");
            existing.ModifiedBy = loggedInUser.EmployeeID;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repository.UpdateAsync(existing);
            return existing.IsActive;
        }

        public async Task<EffectiveLayoutResponseDto> ResolveEffectiveLayoutAsync(long testId, long? methodId, long? versionId)
        {
            await RequireTestAsync(testId);
            var rows = (await _repository.GetByTestAsync(testId, loggedInUser.CompanyCode))
                .Where(x => x.IsActive && x.ExecutionLayout != null && x.ExecutionLayout.IsActive)
                .ToList();
            List<LaboratoryTestLayout> level;
            string resolutionLevel;
            if (versionId != null && (level = rows.Where(x => x.TestMethodSpecificationID == methodId && x.TestMethodSpecificationVersionID == versionId).ToList()).Count > 0)
                resolutionLevel = "TEST_METHOD_VERSION";
            else if (methodId != null && (level = rows.Where(x => x.TestMethodSpecificationID == methodId && x.TestMethodSpecificationVersionID == null).ToList()).Count > 0)
                resolutionLevel = "TEST_METHOD";
            else if ((level = rows.Where(x => x.TestMethodSpecificationID == null && x.TestMethodSpecificationVersionID == null).ToList()).Count > 0)
                resolutionLevel = "TEST";
            else
                return new EffectiveLayoutResponseDto { ResolutionLevel = "Unassigned", Reason = "No active layout assignment in scope." };
            var ordered = level.OrderBy(x => x.Priority).ToList();
            var topPriority = ordered.First().Priority;
            var contenders = ordered.Where(x => x.Priority == topPriority).ToList();
            LaboratoryTestLayout winner;
            if (contenders.Count == 1)
                winner = contenders[0];
            else
            {
                var defaults = contenders.Where(x => x.IsDefault).ToList();
                if (defaults.Count == 1)
                    winner = defaults[0];
                else
                    throw new InvalidOperationException($"Ambiguous layout configuration at {resolutionLevel} scope (Priority {topPriority}): conflicting assignment IDs {string.Join(", ", contenders.Select(x => x.ID))}.");
            }
            return new EffectiveLayoutResponseDto
            {
                AssignmentID = winner.ID, ExecutionLayoutID = winner.ExecutionLayoutID,
                LayoutCode = winner.ExecutionLayout?.Code, LayoutName = winner.ExecutionLayout?.Name,
                RendererType = winner.ExecutionLayout?.RendererType,
                ResolutionLevel = resolutionLevel, Priority = winner.Priority,
                Reason = $"{(winner.IsDefault ? "Default" : "Priority")} assignment at {resolutionLevel} scope."
            };
        }
    }
}
