using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class LabScopeService : ILabScopeService
    {
        private readonly ILabScopeRepository _labScopeRepository;
        private readonly ILogger<LabScopeService> _logger;
        private readonly LIMSContext _context;
        private readonly LoggedInUserDTO _loggedInUser;

        public LabScopeService(ILabScopeRepository labScopeRepo, ILogger<LabScopeService> logger, LIMSContext context)
        {
            _labScopeRepository = labScopeRepo;
            _logger = logger;
            _context = context;
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateLabScope(LabScopeMaster model)
        {
            // Prevent duplicate scope for same Laboratory Test + SubGroup + AnalysisType
            var exists = await _context.LabScopeMasters
                .AnyAsync(x => x.LaboratoryTestID == model.LaboratoryTestID
                    && x.LaboratoryTestSubGroupID == model.LaboratoryTestSubGroupID
                    && x.AnalysisTypeID == model.AnalysisTypeID
                    && x.IsActive
                    && x.CompanyCode == _loggedInUser.CompanyCode);
            if (exists)
                throw new InvalidOperationException("A scope already exists for this Laboratory Test, Sub-Group, and Analysis Technique combination. Edit the existing scope instead.");

            await _labScopeRepository.AddLabScope(model);

            // G10: Log creation
            _context.LabScopeChangeLogs.Add(new LabScopeChangeLog
            {
                LabScopeID = model.ID,
                ChangeType = "Created",
                EntityName = $"LabTestID: {model.LaboratoryTestID}, SubGroupID: {model.LaboratoryTestSubGroupID}, AnalysisTypeID: {model.AnalysisTypeID}",
                NewValue = $"{model.Specifications.Count} specifications, {model.Specifications.SelectMany(s => s.Parameters).Count()} parameters",
                ChangedBy = _loggedInUser.EmployeeID,
                ChangedOn = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            _logger.LogInformation("LabScope '{LabScopeName}' created successfully.", model.LaboratoryTestID);
        }

        public async Task ModifyLabScope(LabScopeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("LabScope ID should not be empty!");

            var existingLabScope = await _labScopeRepository.GetLabScopeById(model.ID);
            if (existingLabScope == null)
                throw new InvalidOperationException("LabScope not found!");

            var duplicate = await _context.LabScopeMasters
                .AnyAsync(x => x.LaboratoryTestID == model.LaboratoryTestID
                    && x.LaboratoryTestSubGroupID == model.LaboratoryTestSubGroupID
                    && x.AnalysisTypeID == model.AnalysisTypeID
                    && x.IsActive
                    && x.ID != model.ID
                    && x.CompanyCode == _loggedInUser.CompanyCode);
            if (duplicate)
                throw new InvalidOperationException("A scope already exists for this Laboratory Test, Sub-Group, and Analysis Technique combination.");

            existingLabScope.LaboratoryTestID = model.LaboratoryTestID;
            existingLabScope.LaboratoryTestSubGroupID = model.LaboratoryTestSubGroupID;
            existingLabScope.AnalysisTypeID = model.AnalysisTypeID;
            existingLabScope.ValidFrom = model.ValidFrom;
            existingLabScope.ValidUntil = model.ValidUntil;
            existingLabScope.NextReviewDate = model.NextReviewDate;
            existingLabScope.ScopeRemarks = model.ScopeRemarks;
            existingLabScope.ModifiedOn = DateTime.UtcNow;
            existingLabScope.ModifiedBy = _loggedInUser.EmployeeID;

            var changeLogs = new List<LabScopeChangeLog>();

            // --- Remove missing Specifications ---
            var toRemoveSpec = existingLabScope.Specifications
                .Where(x => !model.Specifications.Any(y => y.ID == x.ID))
                .ToList();

            foreach (var spec in toRemoveSpec)
            {
                changeLogs.Add(new LabScopeChangeLog
                {
                    LabScopeID = model.ID,
                    ChangeType = "SpecificationRemoved",
                    EntityName = spec.TestMethodSpecification?.Name ?? $"SpecID: {spec.TestMethodSpecificationID}",
                    OldValue = $"{spec.Parameters.Count} parameters",
                    ChangedBy = _loggedInUser.EmployeeID,
                    ChangedOn = DateTime.UtcNow
                });
                existingLabScope.Specifications.Remove(spec);
            }

            // --- Add or Update Specifications ---
            foreach (var incomingSpec in model.Specifications)
            {
                var existingSpec = existingLabScope.Specifications
                    .FirstOrDefault(x => x.ID == incomingSpec.ID);

                if (existingSpec == null)
                {
                    // New specification
                    changeLogs.Add(new LabScopeChangeLog
                    {
                        LabScopeID = model.ID,
                        ChangeType = "SpecificationAdded",
                        EntityName = $"SpecID: {incomingSpec.TestMethodSpecificationID}",
                        NewValue = $"{incomingSpec.Parameters.Count} parameters",
                        ChangedBy = _loggedInUser.EmployeeID,
                        ChangedOn = DateTime.UtcNow
                    });
                    existingLabScope.Specifications.Add(incomingSpec);
                }
                else
                {
                    existingSpec.TestMethodSpecificationID = incomingSpec.TestMethodSpecificationID;
                    existingSpec.TestMethodSpecificationVersionID = incomingSpec.TestMethodSpecificationVersionID;
                    existingSpec.ModifiedOn = DateTime.UtcNow;
                    existingSpec.ModifiedBy = _loggedInUser.EmployeeID;

                    // --- Remove missing Parameters ---
                    var toRemoveParams = existingSpec.Parameters
                        .Where(p => !incomingSpec.Parameters.Any(ip => ip.ID == p.ID))
                        .ToList();

                    foreach (var param in toRemoveParams)
                    {
                        changeLogs.Add(new LabScopeChangeLog
                        {
                            LabScopeID = model.ID,
                            ChangeType = "ParameterRemoved",
                            EntityName = $"ParamID: {param.ParameterID}",
                            OldValue = $"Limits: {param.LowerLimitValue} - {param.UpperLimitValue}",
                            ChangedBy = _loggedInUser.EmployeeID,
                            ChangedOn = DateTime.UtcNow
                        });
                        existingSpec.Parameters.Remove(param);
                    }

                    // --- Add or Update Parameters ---
                    foreach (var incomingParam in incomingSpec.Parameters)
                    {
                        var existingParam = existingSpec.Parameters
                            .FirstOrDefault(p => p.ID == incomingParam.ID);

                        if (existingParam == null)
                        {
                            changeLogs.Add(new LabScopeChangeLog
                            {
                                LabScopeID = model.ID,
                                ChangeType = "ParameterAdded",
                                EntityName = $"ParamID: {incomingParam.ParameterID}",
                                NewValue = $"Limits: {incomingParam.LowerLimitValue} - {incomingParam.UpperLimitValue}, ISO: {incomingParam.IsUnderISO}",
                                ChangedBy = _loggedInUser.EmployeeID,
                                ChangedOn = DateTime.UtcNow
                            });
                            existingSpec.Parameters.Add(incomingParam);
                        }
                        else
                        {
                            // Track limit changes
                            if (existingParam.LowerLimitValue != incomingParam.LowerLimitValue ||
                                existingParam.UpperLimitValue != incomingParam.UpperLimitValue)
                            {
                                changeLogs.Add(new LabScopeChangeLog
                                {
                                    LabScopeID = model.ID,
                                    ChangeType = "LimitsChanged",
                                    EntityName = $"ParamID: {existingParam.ParameterID}",
                                    OldValue = $"{existingParam.LowerLimitValue} - {existingParam.UpperLimitValue}",
                                    NewValue = $"{incomingParam.LowerLimitValue} - {incomingParam.UpperLimitValue}",
                                    ChangedBy = _loggedInUser.EmployeeID,
                                    ChangedOn = DateTime.UtcNow
                                });
                            }

                            existingParam.ParameterID = incomingParam.ParameterID;
                            existingParam.ParameterUnitID = incomingParam.ParameterUnitID;
                            existingParam.QualitativeQuantitative = incomingParam.QualitativeQuantitative;
                            existingParam.IsUnderISO = incomingParam.IsUnderISO;
                            existingParam.LowerLimit = incomingParam.LowerLimit;
                            existingParam.LowerLimitValue = incomingParam.LowerLimitValue;
                            existingParam.UpperLimit = incomingParam.UpperLimit;
                            existingParam.UpperLimitValue = incomingParam.UpperLimitValue;
                            existingParam.DisciplineID = incomingParam.DisciplineID;
                            existingParam.GroupID = incomingParam.GroupID;
                            existingParam.SubGroupID = incomingParam.SubGroupID;
                            existingParam.ModifiedOn = DateTime.UtcNow;
                            existingParam.ModifiedBy = _loggedInUser.EmployeeID;

                            // --- Remove missing Equipments ---
                            if (existingParam.Equipments == null) existingParam.Equipments = new List<LabScopeSpecificationParameterEquipment>();
                            var toRemoveEquip = existingParam.Equipments
                                .Where(e => !incomingParam.Equipments.Any(ie => ie.EquipmentID == e.EquipmentID))
                                .ToList();

                            foreach (var equip in toRemoveEquip)
                            {
                                existingParam.Equipments.Remove(equip);
                            }

                            // --- Add new Equipments ---
                            foreach (var incomingEquip in incomingParam.Equipments)
                            {
                                if (!existingParam.Equipments.Any(e => e.EquipmentID == incomingEquip.EquipmentID))
                                {
                                    existingParam.Equipments.Add(incomingEquip);
                                }
                            }
                        }
                    }
                }
            }

            // Save changelog
            if (changeLogs.Any())
            {
                _context.LabScopeChangeLogs.AddRange(changeLogs);
            }

            await _labScopeRepository.UpdateLabScope(existingLabScope);
            _logger.LogInformation("LabScope '{LabScopeName}' updated successfully. {ChangeCount} changes logged.", model.LaboratoryTestID, changeLogs.Count);
        }


        public async Task RemoveLabScope(long id)
        {
            var existingLabScope = await _labScopeRepository.GetLabScopeById(id);
            if (existingLabScope == null)
                throw new InvalidOperationException("LabScope not found!");

            // G12: Check FK dependencies before delete
            await DeleteValidationHelper.ValidateDeleteAsync<LabScopeMaster>(_context, id, "Lab Scope");

            existingLabScope.IsActive = false;
            existingLabScope.ModifiedOn = DateTime.UtcNow;
            existingLabScope.ModifiedBy = _loggedInUser.EmployeeID;

            await _labScopeRepository.UpdateLabScope(existingLabScope);
            _logger.LogInformation("LabScope with ID '{LabScopeId}' deleted successfully.", id);
        }

        public async Task<LabScopeMaster> GetLabScopeDetails(long id)
        {
            var classification = await _labScopeRepository.GetLabScopeById(id);
            if (classification == null)
                throw new InvalidOperationException("LabScope not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchLabScopeList(PageFilter filter)
        {
            return await _labScopeRepository.GetAllLabScopes(filter);
        }

        public async Task<List<UnifiedTestHierarchyDto>> GetUnifiedTestHierarchy(string? searchTerm = null, int pageNo = 1, int pageSize = 50)
        {
            var rawTests = await _context.LaboratoryTests.AsNoTracking()
                .Where(t => t.IsActive)
                .Select(t => new
                {
                    TestId = t.ID,
                    TestName = t.Name,
                    IsChemical = t.IsChemicalTest
                })
                .ToListAsync();

            var rawSubGroups = await _context.LaboratoryTestSubGroups.AsNoTracking()
                .Where(sg => sg.IsActive)
                .Select(sg => new
                {
                    SubGroupId = sg.ID,
                    SubGroupName = sg.Name,
                    LaboratoryTestId = sg.LaboratoryTestID
                })
                .ToListAsync();

            var rawAnalysisTypes = await _context.LaboratoryTestAnalysisTypes.AsNoTracking()
                .Where(at => at.IsActive)
                .Select(at => new
                {
                    AnalysisTypeId = at.ID,
                    AnalysisTypeName = at.Name,
                    LaboratoryTestSubGroupId = at.LaboratoryTestSubGroupID
                })
                .ToListAsync();

            var term = searchTerm?.Trim().ToLower();

            // Direct key/ID lookup (e.g. on rebind)
            if (!string.IsNullOrWhiteSpace(term) && (term.StartsWith("chem_at_") || term.StartsWith("chem_sg_") || term.StartsWith("gen_sg_") || term.StartsWith("test_")))
            {
                foreach (var test in rawTests)
                {
                    var matchingSubGroups = rawSubGroups.Where(sg => sg.LaboratoryTestId == test.TestId).ToList();
                    foreach (var sg in matchingSubGroups)
                    {
                        var matchingAts = rawAnalysisTypes.Where(at => at.LaboratoryTestSubGroupId == sg.SubGroupId).ToList();
                        foreach (var at in matchingAts)
                        {
                            var key = $"chem_at_{at.AnalysisTypeId}".ToLower();
                            if (key == term)
                            {
                                return new List<UnifiedTestHierarchyDto>
                                {
                                    new UnifiedTestHierarchyDto
                                    {
                                        Key = $"chem_at_{at.AnalysisTypeId}",
                                        Id = $"chem_at_{at.AnalysisTypeId}",
                                        LaboratoryTestID = test.TestId,
                                        LaboratoryTestSubGroupID = sg.SubGroupId,
                                        AnalysisTypeID = at.AnalysisTypeId,
                                        TestName = test.TestName,
                                        SubGroupName = sg.SubGroupName,
                                        AnalysisTypeName = at.AnalysisTypeName,
                                        IsChemicalTest = true,
                                        Name = at.AnalysisTypeName,
                                        HierarchyPath = $"{test.TestName} > {sg.SubGroupName} > {at.AnalysisTypeName}",
                                        Category = "Chemical",
                                        IsHeader = false,
                                        Level = 2,
                                        Selectable = true,
                                        AdditionalValues = new Dictionary<string, object>
                                        {
                                            { "fullDisplayName", $"{test.TestName} > {sg.SubGroupName} > {at.AnalysisTypeName}" }
                                        }
                                    }
                                };
                            }
                        }

                        var sgKey = (test.IsChemical ? $"chem_sg_{sg.SubGroupId}" : $"gen_sg_{sg.SubGroupId}").ToLower();
                        if (sgKey == term)
                        {
                            return new List<UnifiedTestHierarchyDto>
                            {
                                new UnifiedTestHierarchyDto
                                {
                                    Key = test.IsChemical ? $"chem_sg_{sg.SubGroupId}" : $"gen_sg_{sg.SubGroupId}",
                                    Id = test.IsChemical ? $"chem_sg_{sg.SubGroupId}" : $"gen_sg_{sg.SubGroupId}",
                                    LaboratoryTestID = test.TestId,
                                    LaboratoryTestSubGroupID = sg.SubGroupId,
                                    TestName = test.TestName,
                                    SubGroupName = sg.SubGroupName,
                                    IsChemicalTest = test.IsChemical,
                                    Name = sg.SubGroupName,
                                    HierarchyPath = $"{test.TestName} > {sg.SubGroupName}",
                                    Category = test.IsChemical ? "Chemical" : "Mechanical / General",
                                    IsHeader = false,
                                    Level = 1,
                                    Selectable = true,
                                    AdditionalValues = new Dictionary<string, object>
                                    {
                                        { "fullDisplayName", $"{test.TestName} > {sg.SubGroupName}" }
                                    }
                                }
                            };
                        }
                    }

                    var testKey = $"test_{test.TestId}".ToLower();
                    if (testKey == term)
                    {
                        return new List<UnifiedTestHierarchyDto>
                        {
                            new UnifiedTestHierarchyDto
                            {
                                Key = $"test_{test.TestId}",
                                Id = $"test_{test.TestId}",
                                LaboratoryTestID = test.TestId,
                                TestName = test.TestName,
                                IsChemicalTest = test.IsChemical,
                                Name = test.TestName,
                                HierarchyPath = test.TestName,
                                Category = test.IsChemical ? "Chemical" : "Mechanical / General",
                                IsHeader = false,
                                Level = 0,
                                Selectable = true,
                                AdditionalValues = new Dictionary<string, object>
                                {
                                    { "fullDisplayName", test.TestName }
                                }
                            }
                        };
                    }
                }
            }

            var result = new List<UnifiedTestHierarchyDto>();

            foreach (var test in rawTests)
            {
                var matchingSubGroups = rawSubGroups.Where(sg => sg.LaboratoryTestId == test.TestId).ToList();

                if (test.IsChemical)
                {
                    var testMatches = string.IsNullOrEmpty(term) || test.TestName.ToLower().Contains(term);
                    var relevantSubGroups = matchingSubGroups.Where(sg =>
                    {
                        if (testMatches) return true;
                        if (sg.SubGroupName.ToLower().Contains(term)) return true;
                        return rawAnalysisTypes.Any(at => at.LaboratoryTestSubGroupId == sg.SubGroupId && at.AnalysisTypeName.ToLower().Contains(term));
                    }).ToList();

                    if (!testMatches && relevantSubGroups.Count == 0) continue;

                    // Level 0 Header: Test Name
                    result.Add(new UnifiedTestHierarchyDto
                    {
                        Key = $"header_test_{test.TestId}",
                        Id = $"header_test_{test.TestId}",
                        LaboratoryTestID = test.TestId,
                        TestName = test.TestName,
                        IsChemicalTest = true,
                        Name = test.TestName,
                        HierarchyPath = test.TestName,
                        Category = "Chemical",
                        IsHeader = true,
                        Level = 0,
                        Selectable = false
                    });

                    if (relevantSubGroups.Count == 0)
                    {
                        result.Add(new UnifiedTestHierarchyDto
                        {
                            Key = $"test_{test.TestId}",
                            Id = $"test_{test.TestId}",
                            LaboratoryTestID = test.TestId,
                            TestName = test.TestName,
                            IsChemicalTest = true,
                            Name = test.TestName,
                            HierarchyPath = test.TestName,
                            Category = "Chemical",
                            IsHeader = false,
                            Level = 1,
                            Selectable = true,
                            AdditionalValues = new Dictionary<string, object>
                            {
                                { "fullDisplayName", test.TestName }
                            }
                        });
                    }
                    else
                    {
                        foreach (var sg in relevantSubGroups)
                        {
                            var sgMatches = testMatches || sg.SubGroupName.ToLower().Contains(term);
                            var matchingAnalysisTypes = rawAnalysisTypes
                                .Where(at => at.LaboratoryTestSubGroupId == sg.SubGroupId && (sgMatches || at.AnalysisTypeName.ToLower().Contains(term)))
                                .ToList();

                            if (matchingAnalysisTypes.Count == 0)
                            {
                                result.Add(new UnifiedTestHierarchyDto
                                {
                                    Key = $"chem_sg_{sg.SubGroupId}",
                                    Id = $"chem_sg_{sg.SubGroupId}",
                                    LaboratoryTestID = test.TestId,
                                    LaboratoryTestSubGroupID = sg.SubGroupId,
                                    TestName = test.TestName,
                                    SubGroupName = sg.SubGroupName,
                                    IsChemicalTest = true,
                                    Name = sg.SubGroupName,
                                    HierarchyPath = $"{test.TestName} > {sg.SubGroupName}",
                                    Category = "Chemical",
                                    IsHeader = false,
                                    Level = 1,
                                    Selectable = true,
                                    AdditionalValues = new Dictionary<string, object>
                                    {
                                        { "fullDisplayName", $"{test.TestName} > {sg.SubGroupName}" }
                                    }
                                });
                            }
                            else
                            {
                                // Level 1 Header: SubGroup Name
                                result.Add(new UnifiedTestHierarchyDto
                                {
                                    Key = $"header_sg_{sg.SubGroupId}",
                                    Id = $"header_sg_{sg.SubGroupId}",
                                    LaboratoryTestID = test.TestId,
                                    LaboratoryTestSubGroupID = sg.SubGroupId,
                                    TestName = test.TestName,
                                    SubGroupName = sg.SubGroupName,
                                    IsChemicalTest = true,
                                    Name = sg.SubGroupName,
                                    HierarchyPath = $"{test.TestName} > {sg.SubGroupName}",
                                    Category = "Chemical",
                                    IsHeader = true,
                                    Level = 1,
                                    Selectable = false
                                });

                                // Level 2 Selectable Leaves: Analysis Types
                                foreach (var at in matchingAnalysisTypes)
                                {
                                    result.Add(new UnifiedTestHierarchyDto
                                    {
                                        Key = $"chem_at_{at.AnalysisTypeId}",
                                        Id = $"chem_at_{at.AnalysisTypeId}",
                                        LaboratoryTestID = test.TestId,
                                        LaboratoryTestSubGroupID = sg.SubGroupId,
                                        AnalysisTypeID = at.AnalysisTypeId,
                                        TestName = test.TestName,
                                        SubGroupName = sg.SubGroupName,
                                        AnalysisTypeName = at.AnalysisTypeName,
                                        IsChemicalTest = true,
                                        Name = at.AnalysisTypeName,
                                        HierarchyPath = $"{test.TestName} > {sg.SubGroupName} > {at.AnalysisTypeName}",
                                        Category = "Chemical",
                                        IsHeader = false,
                                        Level = 2,
                                        Selectable = true,
                                        AdditionalValues = new Dictionary<string, object>
                                        {
                                            { "fullDisplayName", $"{test.TestName} > {sg.SubGroupName} > {at.AnalysisTypeName}" }
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Mechanical / General
                    var testMatches = string.IsNullOrEmpty(term) || test.TestName.ToLower().Contains(term);
                    var relevantSubGroups = matchingSubGroups
                        .Where(sg => testMatches || sg.SubGroupName.ToLower().Contains(term))
                        .ToList();

                    if (!testMatches && relevantSubGroups.Count == 0) continue;

                    if (relevantSubGroups.Count == 0)
                    {
                        result.Add(new UnifiedTestHierarchyDto
                        {
                            Key = $"test_{test.TestId}",
                            Id = $"test_{test.TestId}",
                            LaboratoryTestID = test.TestId,
                            TestName = test.TestName,
                            IsChemicalTest = false,
                            Name = test.TestName,
                            HierarchyPath = test.TestName,
                            Category = "Mechanical / General",
                            IsHeader = false,
                            Level = 0,
                            Selectable = true,
                            AdditionalValues = new Dictionary<string, object>
                            {
                                { "fullDisplayName", test.TestName }
                            }
                        });
                    }
                    else
                    {
                        // Level 0 Header: Test Name
                        result.Add(new UnifiedTestHierarchyDto
                        {
                            Key = $"header_test_{test.TestId}",
                            Id = $"header_test_{test.TestId}",
                            LaboratoryTestID = test.TestId,
                            TestName = test.TestName,
                            IsChemicalTest = false,
                            Name = test.TestName,
                            HierarchyPath = test.TestName,
                            Category = "Mechanical / General",
                            IsHeader = true,
                            Level = 0,
                            Selectable = false
                        });

                        // Level 1 Selectable Leaves: SubGroups
                        foreach (var sg in relevantSubGroups)
                        {
                            result.Add(new UnifiedTestHierarchyDto
                            {
                                Key = $"gen_sg_{sg.SubGroupId}",
                                Id = $"gen_sg_{sg.SubGroupId}",
                                LaboratoryTestID = test.TestId,
                                LaboratoryTestSubGroupID = sg.SubGroupId,
                                TestName = test.TestName,
                                SubGroupName = sg.SubGroupName,
                                IsChemicalTest = false,
                                Name = sg.SubGroupName,
                                HierarchyPath = $"{test.TestName} > {sg.SubGroupName}",
                                Category = "Mechanical / General",
                                IsHeader = false,
                                Level = 1,
                                Selectable = true,
                                AdditionalValues = new Dictionary<string, object>
                                {
                                    { "fullDisplayName", $"{test.TestName} > {sg.SubGroupName}" }
                                }
                            });
                        }
                    }
                }
            }

            var effectivePageSize = Math.Max(pageSize, 120);
            return result
                .Skip((pageNo - 1) * effectivePageSize)
                .Take(effectivePageSize)
                .ToList();
        }

        public async Task<NablValidityDto> GetNablValidity()
        {
            var nabl = await _context.NablAccreditations.AsNoTracking().FirstOrDefaultAsync();

            DateTime fromDate;
            DateTime toDate;

            if (nabl != null && nabl.IssueDate > new DateTime(2000, 1, 1) && nabl.ExpiryDate > new DateTime(2000, 1, 1))
            {
                fromDate = nabl.IssueDate;
                toDate = nabl.ExpiryDate;
            }
            else
            {
                var today = DateTime.Today;
                fromDate = new DateTime(today.Year, 4, 1);
                if (today < fromDate) fromDate = fromDate.AddYears(-1);
                toDate = fromDate.AddYears(2).AddDays(-1);
            }

            var reviewDate = fromDate.AddYears(1);
            var isExpired = DateTime.Today > toDate;
            var isActive = (nabl?.IsActive ?? true) && !isExpired;
            var status = isExpired ? "Certificate Expired" : "Certificate Effective";
            var validityPeriodFormatted = $"{fromDate:dd MMM yyyy} – {toDate:dd MMM yyyy}";

            return new NablValidityDto
            {
                ValidFrom = fromDate.ToString("yyyy-MM-dd"),
                ValidUntil = toDate.ToString("yyyy-MM-dd"),
                NextReviewDate = reviewDate.ToString("yyyy-MM-dd"),
                CertificateNumber = nabl?.CertificateNumber ?? "",
                IsActive = isActive,
                Status = status,
                ValidityPeriodFormatted = validityPeriodFormatted
            };
        }

    }
}
