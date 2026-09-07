using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class LaboratoryTestService : ILaboratoryTestService
    {
        private readonly ILaboratoryTestRepository _testMethodRepository;
        private readonly ILogger<LaboratoryTestService> _logger;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public LaboratoryTestService(ILaboratoryTestRepository testMethodRepo, ILogger<LaboratoryTestService> logger, LIMSContext context)
        {
            _testMethodRepository = testMethodRepo;
            _logger = logger;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateTestMethod(LaboratoryTest model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("TestMethod name should not be empty!");

            bool exists = await _testMethodRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException($"Test name '{model.Name}' already exists!");

            await ApplyDepartmentChemicalFlag(model);

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _testMethodRepository.AddTestMethod(model);
            _logger.LogInformation("LaboratoryTest '{Name}' created successfully.", model.Name);
        }

        private async Task ApplyDepartmentChemicalFlag(LaboratoryTest model)
        {
            var isChemicalDept = await _context.DepartmentMasters
                .AsNoTracking()
                .Where(d => d.ID == model.LabDepartmentID)
                .Select(d => (bool?)d.IsChemical)
                .FirstOrDefaultAsync() ?? false;

            model.IsChemicalTest = isChemicalDept;
            model.IsMechanical = !isChemicalDept;
        }

        public async Task ModifyTestMethod(LaboratoryTest model)
        {
            if (model.ID == 0)
                throw new ArgumentException("TestMethod ID should not be empty!");

            if (await _testMethodRepository.ExistsByNameAndNotId(model.Name, model.ID))
                throw new InvalidOperationException($"Test name '{model.Name}' already exists!");

            var existingTestMethod = await _testMethodRepository.GetTestMethodById(model.ID);
            if (existingTestMethod == null)
                throw new InvalidOperationException("Laboratory Test not found!");

            existingTestMethod.Name = model.Name;
            existingTestMethod.LabDepartmentID = model.LabDepartmentID;
            existingTestMethod.DisciplineID = model.DisciplineID;
            existingTestMethod.Equation = model.Equation;
            existingTestMethod.TestDuration = model.TestDuration;
            
            await ApplyDepartmentChemicalFlag(existingTestMethod);
            
            existingTestMethod.ModifiedOn = DateTime.UtcNow;
            existingTestMethod.ModifiedBy = loggedInUser.EmployeeID;

            await _testMethodRepository.UpdateTestMethod(existingTestMethod);
            _logger.LogInformation("LaboratoryTest '{Name}' updated successfully.", model.Name);
        }

        public async Task RemoveTestMethod(long id)
        {
            var existingTestMethod = await _testMethodRepository.GetTestMethodById(id);
            if (existingTestMethod == null)
                throw new InvalidOperationException("Laboratory Test not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<LaboratoryTest>(_context, id, "Laboratory Test", existingTestMethod.Name);

            existingTestMethod.IsActive = false;
            existingTestMethod.ModifiedOn = DateTime.UtcNow;
            existingTestMethod.ModifiedBy = loggedInUser?.EmployeeID ?? 0;

            await _testMethodRepository.UpdateTestMethod(existingTestMethod);
            _logger.LogInformation("Laboratory Test with ID '{TestMethodId}' deleted successfully.", id);
        }

        public async Task<LaboratoryTest> GetTestMethodDetails(long id)
        {
            var existingTestMethod = await _testMethodRepository.GetTestMethodById(id);
            if (existingTestMethod == null)
                throw new InvalidOperationException("Laboratory Test not found!");

            return existingTestMethod;
        }

        public async Task<PagedResponse<object>> FetchTestMethodList(PageFilter filter)
        {
            return await _testMethodRepository.GetAllTestMethods(filter);
        }

        // Screen 13: Universal Test Definition Methods
        public async Task<PagedResponse<LaboratoryTestListDto>> GetPagedTestsAsync(PageFilter filter, long? disciplineId = null, long? departmentId = null, bool? isActive = null)
        {
            return await _testMethodRepository.GetPagedTestsAsync(filter, disciplineId, departmentId, isActive);
        }

        public async Task<LaboratoryTestDetailDto> GetUniversalTestByIdAsync(long id)
        {
            var test = await _testMethodRepository.GetUniversalTestByIdAsync(id);
            if (test == null)
                throw new InvalidOperationException($"Laboratory Test with ID {id} not found.");

            return test;
        }

        public static string NormalizeCode(string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return string.Empty;
            var trimmed = code.Trim();
            var normalized = System.Text.RegularExpressions.Regex.Replace(trimmed, @"[\s\-]+", "_");
            return normalized.ToUpperInvariant();
        }

        public async Task<bool> CheckCodeUniqueAsync(string code, long? excludeId = null)
        {
            var normalized = NormalizeCode(code);
            return await _testMethodRepository.IsCodeUniqueAsync(normalized, excludeId);
        }

        public async Task<List<LaboratoryTestDropdownDto>> GetUniversalDropdownAsync(long? disciplineId = null)
        {
            return await _testMethodRepository.GetUniversalDropdownAsync(disciplineId);
        }

        private async Task ValidateActivationGateAsync(
            long? testId,
            string code,
            string name,
            long? disciplineId,
            long? departmentId,
            List<LaboratoryTestParameterItemDto> parameters,
            List<LaboratoryTestMethodItemDto> methods,
            List<LaboratoryTestConditionItemDto> conditions)
        {
            code = NormalizeCode(code);
            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException("Test Code is required.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(code, @"^[A-Z0-9_]+$"))
                throw new InvalidOperationException("Test Code must contain only uppercase letters, numbers, and underscores (e.g. TENSILE_TEST).");

            if (!await _testMethodRepository.IsCodeUniqueAsync(code, testId))
                throw new InvalidOperationException($"Test Code '{code}' already exists for this organization.");

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Test Name is required.");

            // Discipline validation (MANDATORY for active tests)
            if (!disciplineId.HasValue || disciplineId.Value <= 0)
                throw new InvalidOperationException("Discipline is required to activate a Laboratory Test.");

            var discipline = await _context.DisciplineMasters.AsNoTracking()
                .FirstOrDefaultAsync(d => d.ID == disciplineId.Value && d.IsActive && d.CompanyCode == loggedInUser.CompanyCode);
            if (discipline == null)
                throw new InvalidOperationException("Selected Discipline does not exist or is inactive.");

            // Department validation (Advisory)
            if (departmentId.HasValue && departmentId.Value > 0)
            {
                var dept = await _context.DepartmentMasters.AsNoTracking()
                    .FirstOrDefaultAsync(d => d.ID == departmentId.Value && d.IsActive && d.CompanyCode == loggedInUser.CompanyCode);
                if (dept == null)
                    throw new InvalidOperationException("Selected Default Advisory Department does not exist or is inactive.");
            }

            // Parameter validation
            var activeParams = parameters.Where(p => p.IsActive).ToList();
            if (activeParams.Count == 0)
                throw new InvalidOperationException("At least one active parameter must be defined to activate a Laboratory Test.");

            if (activeParams.Select(p => p.ParameterID).Distinct().Count() != activeParams.Count)
                throw new InvalidOperationException("Duplicate parameters detected. Each parameter can only be added once.");

            var paramIds = activeParams.Select(p => p.ParameterID).ToList();
            var existingActiveParams = await _context.ParameterMasters.AsNoTracking()
                .Where(p => paramIds.Contains(p.ID) && p.IsActive && p.CompanyCode == loggedInUser.CompanyCode)
                .Select(p => p.ID)
                .ToListAsync();
            if (existingActiveParams.Count != paramIds.Count)
                throw new InvalidOperationException("One or more selected parameters are invalid, inactive, or belong to another organization.");

            // Method validation
            var activeMethods = methods.Where(m => m.IsActive).ToList();
            if (activeMethods.Count == 0)
                throw new InvalidOperationException("At least one active test method must be defined to activate a Laboratory Test.");

            if (activeMethods.Select(m => m.TestMethodSpecificationID).Distinct().Count() != activeMethods.Count)
                throw new InvalidOperationException("Duplicate test methods detected. Each method specification can only be added once.");

            var defaultMethods = activeMethods.Where(m => m.IsDefault).ToList();
            if (defaultMethods.Count != 1)
                throw new InvalidOperationException("Exactly one test method must be designated as the default method.");

            var methodIds = activeMethods.Select(m => m.TestMethodSpecificationID).ToList();
            var existingActiveMethods = await _context.TestMethodSpecifications.AsNoTracking()
                .Where(m => methodIds.Contains(m.ID) && m.IsActive && m.CompanyCode == loggedInUser.CompanyCode)
                .Select(m => m.ID)
                .ToListAsync();
            if (existingActiveMethods.Count != methodIds.Count)
                throw new InvalidOperationException("One or more selected test methods are invalid, inactive, or belong to another organization.");

            // Condition validation (optional)
            var activeConditions = conditions.Where(c => c.IsActive).ToList();
            if (activeConditions.Count > 0)
            {
                if (activeConditions.Select(c => c.ConditionMasterID).Distinct().Count() != activeConditions.Count)
                    throw new InvalidOperationException("Duplicate test conditions detected. Each condition can only be added once.");

                var condIds = activeConditions.Select(c => c.ConditionMasterID).ToList();
                var existingActiveConds = await _context.ConditionMasters.AsNoTracking()
                    .Where(c => condIds.Contains(c.ID) && c.IsActive && c.CompanyCode == loggedInUser.CompanyCode)
                    .Select(c => c.ID)
                    .ToListAsync();
                if (existingActiveConds.Count != condIds.Count)
                    throw new InvalidOperationException("One or more selected test conditions are invalid, inactive, or belong to another organization.");
            }
        }

        public async Task<long> CreateUniversalTestAsync(LaboratoryTestCreateDto dto)
        {
            dto.Code = NormalizeCode(dto.Code);
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new InvalidOperationException("Test Code is required.");
            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Code, @"^[A-Z0-9_]+$"))
                throw new InvalidOperationException("Test Code must contain only uppercase letters, numbers, and underscores (e.g. TENSILE_TEST).");
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new InvalidOperationException("Test Name is required.");

            if (!await _testMethodRepository.IsCodeUniqueAsync(dto.Code))
                throw new InvalidOperationException($"Test Code '{dto.Code}' already exists for this organization.");

            if (dto.IsActive)
            {
                await ValidateActivationGateAsync(
                    null,
                    dto.Code,
                    dto.Name,
                    dto.DisciplineID,
                    dto.LabDepartmentID,
                    dto.Parameters,
                    dto.Methods,
                    dto.Conditions);
            }

            var entity = new LaboratoryTest
            {
                Code = dto.Code,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                DisciplineID = dto.DisciplineID,
                LabDepartmentID = dto.LabDepartmentID,
                TestDuration = dto.TestDuration,
                IsActive = dto.IsActive,
                CompanyCode = loggedInUser.CompanyCode,
                CreatedBy = loggedInUser.EmployeeID,
                CreatedOn = DateTime.UtcNow
            };

            if (dto.LabDepartmentID.HasValue)
            {
                var dept = await _context.DepartmentMasters.AsNoTracking()
                    .FirstOrDefaultAsync(d => d.ID == dto.LabDepartmentID.Value);
                if (dept != null)
                {
                    entity.IsChemicalTest = dept.IsChemical;
                    entity.IsMechanical = !dept.IsChemical;
                }
            }

            await _context.LaboratoryTests.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Sync child collections (server enforces tenant company code)
            if (dto.Parameters.Any())
            {
                await _testMethodRepository.SyncParametersAsync(entity.ID, dto.Parameters, entity.CompanyCode);
            }
            if (dto.Methods.Any())
            {
                await _testMethodRepository.SyncMethodsAsync(entity.ID, dto.Methods, entity.CompanyCode);
            }
            if (dto.Conditions.Any())
            {
                await _testMethodRepository.SyncConditionsAsync(entity.ID, dto.Conditions, entity.CompanyCode);
            }

            _logger.LogInformation("Universal LaboratoryTest '{Code}' (ID: {ID}) created successfully.", entity.Code, entity.ID);
            return entity.ID;
        }

        public async Task<long> UpdateUniversalTestAsync(LaboratoryTestUpdateDto dto)
        {
            var existing = await _context.LaboratoryTests
                .FirstOrDefaultAsync(t => t.ID == dto.ID && t.CompanyCode == loggedInUser.CompanyCode);
            if (existing == null)
                throw new InvalidOperationException($"Laboratory Test with ID {dto.ID} not found.");

            dto.Code = NormalizeCode(dto.Code);
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new InvalidOperationException("Test Code is required.");
            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Code, @"^[A-Z0-9_]+$"))
                throw new InvalidOperationException("Test Code must contain only uppercase letters, numbers, and underscores (e.g. TENSILE_TEST).");
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new InvalidOperationException("Test Name is required.");

            if (!await _testMethodRepository.IsCodeUniqueAsync(dto.Code, dto.ID))
                throw new InvalidOperationException($"Test Code '{dto.Code}' already exists for this organization.");

            if (dto.IsActive)
            {
                await ValidateActivationGateAsync(
                    dto.ID,
                    dto.Code,
                    dto.Name,
                    dto.DisciplineID,
                    dto.LabDepartmentID,
                    dto.Parameters,
                    dto.Methods,
                    dto.Conditions);
            }

            existing.Code = dto.Code;
            existing.Name = dto.Name.Trim();
            existing.Description = dto.Description?.Trim();
            existing.DisciplineID = dto.DisciplineID;
            existing.LabDepartmentID = dto.LabDepartmentID;
            existing.TestDuration = dto.TestDuration;
            existing.IsActive = dto.IsActive;
            existing.ModifiedBy = loggedInUser.EmployeeID;
            existing.ModifiedOn = DateTime.UtcNow;

            if (dto.LabDepartmentID.HasValue)
            {
                var dept = await _context.DepartmentMasters.AsNoTracking()
                    .FirstOrDefaultAsync(d => d.ID == dto.LabDepartmentID.Value);
                if (dept != null)
                {
                    existing.IsChemicalTest = dept.IsChemical;
                    existing.IsMechanical = !dept.IsChemical;
                }
            }

            _context.LaboratoryTests.Update(existing);
            await _context.SaveChangesAsync();

            // Sync child collections (server enforces tenant company code)
            await _testMethodRepository.SyncParametersAsync(existing.ID, dto.Parameters, existing.CompanyCode);
            await _testMethodRepository.SyncMethodsAsync(existing.ID, dto.Methods, existing.CompanyCode);
            await _testMethodRepository.SyncConditionsAsync(existing.ID, dto.Conditions, existing.CompanyCode);

            _logger.LogInformation("Universal LaboratoryTest '{Code}' (ID: {ID}) updated successfully.", existing.Code, existing.ID);
            return existing.ID;
        }

        public async Task<bool> ToggleTestStatusAsync(long id)
        {
            var existing = await _testMethodRepository.GetUniversalTestByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"Laboratory Test with ID {id} not found.");

            bool newStatus = !existing.IsActive;

            if (newStatus)
            {
                await ValidateActivationGateAsync(
                    id,
                    existing.Code,
                    existing.Name,
                    existing.DisciplineID,
                    existing.LabDepartmentID,
                    existing.Parameters,
                    existing.Methods,
                    existing.Conditions);
            }

            var entity = await _context.LaboratoryTests.FindAsync(id);
            if (entity != null)
            {
                entity.IsActive = newStatus;
                entity.ModifiedBy = loggedInUser.EmployeeID;
                entity.ModifiedOn = DateTime.UtcNow;
                _context.LaboratoryTests.Update(entity);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Universal LaboratoryTest ID {ID} status toggled to {Status}.", id, newStatus);
            return newStatus;
        }

        public async Task DeleteUniversalTestAsync(long id)
        {
            var existing = await _context.LaboratoryTests
                .FirstOrDefaultAsync(t => t.ID == id && t.CompanyCode == loggedInUser.CompanyCode);
            if (existing == null)
                throw new InvalidOperationException($"Laboratory Test with ID {id} not found.");

            // Check FK dependencies
            await DeleteValidationHelper.ValidateDeleteAsync<LaboratoryTest>(_context, id, "Laboratory Test", existing.Name);

            // Check execution history
            if (await _testMethodRepository.HasExecutionHistoryAsync(id))
                throw new InvalidOperationException($"Cannot delete Laboratory Test '{existing.Name}' because active test execution history exists.");

            existing.IsActive = false;
            existing.ModifiedBy = loggedInUser.EmployeeID;
            existing.ModifiedOn = DateTime.UtcNow;
            _context.LaboratoryTests.Update(existing);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Universal LaboratoryTest '{Name}' (ID: {ID}) soft-deleted.", existing.Name, id);
        }

        public async Task<List<DropdwonSelector>> GetTestMethodDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _testMethodRepository.GetTestMethodDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<List<DropdwonSelector>> GetGeneralTestMethodDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _testMethodRepository.GetGeneralTestMethodDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<List<DropdwonSelector>> GetChemicalTestMethodDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _testMethodRepository.GetChemicalTestMethodDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<List<DropdwonSelector>> GetUnifiedTestMethodDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _testMethodRepository.GetUnifiedTestMethodDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<List<object>> GetTestCases(long labTestId)
        {
            return await _testMethodRepository.GetTestCases(labTestId);
        }

        public async Task<List<string>> GetDistinctTestNames(string? searchTerm, int pageSize)
        {
            return await _testMethodRepository.GetDistinctTestNames(searchTerm, pageSize);
        }

        public async Task<long> DuplicateLaboratoryTest(long id)
        {
            var original = await _context.LaboratoryTests
                .Include(lt => lt.SubGroups)
                    .ThenInclude(sg => sg.Parameters)
                .Include(lt => lt.SubGroups)
                    .ThenInclude(sg => sg.TestMethods)
                .Include(lt => lt.SubGroups)
                    .ThenInclude(sg => sg.Equipments)
                .Include(lt => lt.SubGroups)
                    .ThenInclude(sg => sg.Specifications)
                .Include(lt => lt.SubGroups)
                    .ThenInclude(sg => sg.InvoiceCases)
                .Include(lt => lt.SubGroups)
                    .ThenInclude(sg => sg.AnalysisTypes)
                        .ThenInclude(at => at.AllowedTechniques)
                .Include(lt => lt.SubGroups)
                    .ThenInclude(sg => sg.AnalysisTypes)
                        .ThenInclude(at => at.Parameters)
                .Include(lt => lt.SubGroups)
                    .ThenInclude(sg => sg.AnalysisTypes)
                        .ThenInclude(at => at.TestMethods)
                .Include(lt => lt.SubGroups)
                    .ThenInclude(sg => sg.AnalysisTypes)
                        .ThenInclude(at => at.Equipments)
                .Include(lt => lt.SubGroups)
                    .ThenInclude(sg => sg.AnalysisTypes)
                        .ThenInclude(at => at.Specifications)
                .Include(lt => lt.SubGroups)
                    .ThenInclude(sg => sg.AnalysisTypes)
                        .ThenInclude(at => at.InvoiceCases)
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

            if (original == null)
                throw new KeyNotFoundException("Laboratory Test not found!");

            var copyName = original.Name + " - Copy";
            int copyCount = 1;
            while (await _testMethodRepository.ExistsByName(copyName))
            {
                copyName = $"{original.Name} - Copy ({copyCount++})";
            }

            var duplicate = new LaboratoryTest
            {
                Name = copyName,
                LabDepartmentID = original.LabDepartmentID,
                DisciplineID = original.DisciplineID,
                IsChemicalTest = original.IsChemicalTest,
                IsMechanical = original.IsMechanical,
                Equation = original.Equation,
                TestDuration = original.TestDuration,
                IsActive = true,
                CreatedBy = loggedInUser.EmployeeID,
                CreatedOn = DateTime.UtcNow,
                CompanyCode = loggedInUser.CompanyCode
            };

            foreach (var sg in original.SubGroups.Where(x => x.IsActive))
            {
                var sgCopy = new LaboratoryTestSubGroup
                {
                    Name = sg.Name,
                    ReportTestName = sg.ReportTestName,
                    TestDuration = sg.TestDuration,
                    MetalClassificationID = sg.MetalClassificationID,
                    IsActive = true,
                    CreatedBy = loggedInUser.EmployeeID,
                    CreatedOn = DateTime.UtcNow,
                    CompanyCode = loggedInUser.CompanyCode
                };

                // Subgroup Parameters
                foreach (var p in sg.Parameters)
                {
                    sgCopy.Parameters.Add(new LaboratoryTestSubGroupParameter
                    {
                        ParameterID = p.ParameterID,
                        Sequence = p.Sequence,
                        IsMandatory = p.IsMandatory,
                        IsReportable = p.IsReportable
                    });
                }

                // Subgroup Methods
                foreach (var m in sg.TestMethods)
                {
                    sgCopy.TestMethods.Add(new LaboratoryTestSubGroupMethod
                    {
                        TestMethodSpecificationID = m.TestMethodSpecificationID,
                        TestMethodSpecificationVersionID = m.TestMethodSpecificationVersionID,
                        IsDefault = m.IsDefault
                    });
                }

                // Subgroup Equipments
                foreach (var e in sg.Equipments)
                {
                    sgCopy.Equipments.Add(new LaboratoryTestSubGroupEquipment
                    {
                        EquipmentID = e.EquipmentID,
                        IsDefault = e.IsDefault
                    });
                }

                // Subgroup Specs
                foreach (var s in sg.Specifications)
                {
                    sgCopy.Specifications.Add(new LaboratoryTestSubGroupSpecification
                    {
                        SpecificationHeaderID = s.SpecificationHeaderID,
                        SpecificationGradeID = s.SpecificationGradeID
                    });
                }

                // Subgroup InvoiceCases
                foreach (var ic in sg.InvoiceCases)
                {
                    sgCopy.InvoiceCases.Add(new LaboratoryTestSubGroupInvoiceCase
                    {
                        InvoiceCaseConfigID = ic.InvoiceCaseConfigID
                    });
                }

                // Subgroup AnalysisTypes
                foreach (var at in sg.AnalysisTypes.Where(x => x.IsActive))
                {
                    var atCopy = new LaboratoryTestAnalysisType
                    {
                        Name = at.Name,
                        TestDuration = at.TestDuration,
                        MetalClassificationID = at.MetalClassificationID,
                        IsActive = true,
                        CreatedBy = loggedInUser.EmployeeID,
                        CreatedOn = DateTime.UtcNow,
                        CompanyCode = loggedInUser.CompanyCode
                    };

                    // AnalysisType Techniques
                    foreach (var tech in at.AllowedTechniques)
                    {
                        atCopy.AllowedTechniques.Add(new LaboratoryTestAnalysisTypeTechnique
                        {
                            AnalysisTechniqueID = tech.AnalysisTechniqueID
                        });
                    }

                    // AnalysisType Parameters
                    foreach (var p in at.Parameters)
                    {
                        atCopy.Parameters.Add(new LaboratoryTestAnalysisTypeParameter
                        {
                            ParameterID = p.ParameterID,
                            Sequence = p.Sequence,
                            IsMandatory = p.IsMandatory,
                            IsReportable = p.IsReportable
                        });
                    }

                    // AnalysisType Methods
                    foreach (var m in at.TestMethods)
                    {
                        atCopy.TestMethods.Add(new LaboratoryTestAnalysisTypeMethod
                        {
                            TestMethodSpecificationID = m.TestMethodSpecificationID,
                            TestMethodSpecificationVersionID = m.TestMethodSpecificationVersionID,
                            IsDefault = m.IsDefault
                        });
                    }

                    // AnalysisType Equipments
                    foreach (var e in at.Equipments)
                    {
                        atCopy.Equipments.Add(new LaboratoryTestAnalysisTypeEquipment
                        {
                            EquipmentID = e.EquipmentID,
                            IsDefault = e.IsDefault
                        });
                    }

                    // AnalysisType Specs
                    foreach (var s in at.Specifications)
                    {
                        atCopy.Specifications.Add(new LaboratoryTestAnalysisTypeSpecification
                        {
                            SpecificationHeaderID = s.SpecificationHeaderID,
                            SpecificationGradeID = s.SpecificationGradeID
                        });
                    }

                    // AnalysisType InvoiceCases
                    foreach (var ic in at.InvoiceCases)
                    {
                        atCopy.InvoiceCases.Add(new LaboratoryTestAnalysisTypeInvoiceCase
                        {
                            InvoiceCaseConfigID = ic.InvoiceCaseConfigID
                        });
                    }

                    sgCopy.AnalysisTypes.Add(atCopy);
                }

                duplicate.SubGroups.Add(sgCopy);
            }

            await _context.LaboratoryTests.AddAsync(duplicate);
            await _context.SaveChangesAsync();
            return duplicate.ID;
        }

        public async Task<List<PricingTemplateRowDto>> GetPricingTemplate(long labTestId, long? analysisTypeId)
        {
            return await _testMethodRepository.GetPricingTemplate(labTestId, analysisTypeId);
        }

        public async Task<List<DropdwonSelector>> GetTestMethodSpecificationByLabTestId(long labTestId)
        {
            // 1. Test Method Specifications from SubGroup TestMethods
            var fromSubGroups = await _context.LaboratoryTestSubGroupMethods
                .AsNoTracking()
                .Include(m => m.TestMethodSpecification)
                .Include(m => m.SubGroup)
                .Where(m => m.SubGroup != null && m.SubGroup.LaboratoryTestID == labTestId && m.TestMethodSpecification != null && !m.TestMethodSpecification.IsDisabled)
                .Select(m => new DropdwonSelector
                {
                    Id = m.TestMethodSpecificationID,
                    Name = m.TestMethodSpecification!.DisplayTitle ?? m.TestMethodSpecification.Name
                })
                .ToListAsync();

            // 2. Test Method Specifications from AnalysisType TestMethods
            var fromAnalysisTypes = await _context.LaboratoryTestAnalysisTypeMethods
                .AsNoTracking()
                .Include(m => m.TestMethodSpecification)
                .Include(m => m.AnalysisType)
                    .ThenInclude(at => at!.SubGroup)
                .Where(m => m.AnalysisType != null && m.AnalysisType.SubGroup != null && m.AnalysisType.SubGroup.LaboratoryTestID == labTestId && m.TestMethodSpecification != null && !m.TestMethodSpecification.IsDisabled)
                .Select(m => new DropdwonSelector
                {
                    Id = m.TestMethodSpecificationID,
                    Name = m.TestMethodSpecification!.DisplayTitle ?? m.TestMethodSpecification.Name
                })
                .ToListAsync();

            // 3. Test Method Specifications from LabScopeSpecifications
            var fromLabScope = await _context.LabScopeSpecifications
                .AsNoTracking()
                .Include(s => s.TestMethodSpecification)
                .Include(s => s.LabScope)
                .Where(s => s.LabScope != null && s.LabScope.LaboratoryTestID == labTestId && s.TestMethodSpecification != null && !s.TestMethodSpecification.IsDisabled && s.IsActive)
                .Select(s => new DropdwonSelector
                {
                    Id = s.TestMethodSpecificationID,
                    Name = s.TestMethodSpecification!.DisplayTitle ?? s.TestMethodSpecification.Name
                })
                .ToListAsync();

            // 4. Test Method Specifications from SpecificationLineTestMethods
            var fromSpecLines = await _context.Set<SpecificationLineTestMethod>()
                .AsNoTracking()
                .Include(s => s.TestMethodSpecification)
                .Where(s => s.LaboratoryTestID == labTestId && s.TestMethodSpecification != null && !s.TestMethodSpecification.IsDisabled)
                .Select(s => new DropdwonSelector
                {
                    Id = s.TestMethodSpecificationID ?? 0,
                    Name = s.TestMethodSpecification!.DisplayTitle ?? s.TestMethodSpecification.Name
                })
                .Where(s => s.Id > 0)
                .ToListAsync();

            return fromSubGroups.Concat(fromAnalysisTypes).Concat(fromLabScope).Concat(fromSpecLines)
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .ToList();
        }
    }
}
