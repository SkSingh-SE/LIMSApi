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

            bool hasLabScope = await _context.LabScopeMasters.AnyAsync(s => s.LaboratoryTestID == id && s.IsActive);
            if (hasLabScope)
                throw new InvalidOperationException("Cannot delete: Laboratory Test is linked to Lab Scope.");

            bool hasProductSpec = await _context.ProductSpecifications.AnyAsync(s => s.LaboratoryTestID == id && s.IsActive);
            if (hasProductSpec)
                throw new InvalidOperationException("Cannot delete: Laboratory Test is linked to Product Specifications.");

            bool hasTestResult = await _context.TestResultHeaders.AnyAsync(t => t.LaboratoryTestID == id && t.IsActive);
            if (hasTestResult)
                throw new InvalidOperationException("Cannot delete: Laboratory Test is linked to Test Results.");

            bool hasSamplePrep = await _context.SamplePreparationMasters.AnyAsync(s => s.LaboratoryTestID == id && s.IsActive);
            if (hasSamplePrep)
                throw new InvalidOperationException("Cannot delete: Laboratory Test is linked to Sample Preparations.");

            bool hasProductTestGroup = await _context.ProductTestGroups.AnyAsync(p => p.LaboratoryTestID == id && p.IsActive);
            if (hasProductTestGroup)
                throw new InvalidOperationException("Cannot delete: Laboratory Test is linked to Product Test Groups.");

            bool hasGeneralTestMethod = await _context.GeneralTestMethods.AnyAsync(g => g.LaboratoryTestID == id);
            if (hasGeneralTestMethod)
                throw new InvalidOperationException("Cannot delete: Laboratory Test is linked to General Test Methods.");

            existingTestMethod.IsActive = false;
            existingTestMethod.ModifiedOn = DateTime.UtcNow;
            existingTestMethod.ModifiedBy = loggedInUser.EmployeeID;

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
                        SpecificationGradeID = s.SpecificationGradeID,
                        ProductSpecificationID = s.ProductSpecificationID
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
                            SpecificationGradeID = s.SpecificationGradeID,
                            ProductSpecificationID = s.ProductSpecificationID
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
    }
}
