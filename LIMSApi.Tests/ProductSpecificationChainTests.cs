using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Moq;
using LIMSApi.Data;
using LIMSApi.Models;
using LIMSApi.Services;
using LIMSApi.Repositories;
using LIMSApi.Dtos;
using Microsoft.Extensions.Logging;

namespace LIMSApi.Tests
{
    [TestFixture]
    public class ProductSpecificationChainTests
    {
        private LIMSContext _context = null!;

        // Seeded entity IDs for reference across tests
        private const long StdOrgId_BIS = 1;
        private const long MetalClassId_CarbonSteel = 1;
        private const long MetalClassId_StainlessSteel = 2;
        private const long DeptId_Mechanical = 1;
        private const long DeptId_Chemical = 2;
        private const long EquipmentId_UTM = 1;
        private const long EquipmentId_BendMachine = 2;
        private const long EquipmentId_Spectrometer = 3;
        private const long LabTestId_Tensile = 1;
        private const long LabTestId_Bend = 2;
        private const long LabTestId_Chemical = 3;
        private const long TestMethodStdId_IS1608 = 1;
        private const long TestMethodStdId_IS1599 = 2;
        private const long TestMethodStdId_IS228 = 3;
        private const long TestMethodSpecId_IS2062 = 1;
        private const long SpecHeaderId_IS2062 = 1;
        private const long SpecGradeId_E250BR = 1;
        private const long SpecGradeId_E250BR_SS = 2;
        private const long ParamId_UTS = 1;
        private const long ParamId_YS = 2;
        private const long ParamId_Elongation = 3;
        private const long ParamId_Carbon = 4;
        private const long ParamId_Manganese = 5;
        private const long ParamId_Sulphur = 6;
        private const long ParamId_Phosphorus = 7;
        private const long ParamUnitId_MPa = 1;
        private const long ParamUnitId_Percent = 2;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<LIMSContext>()
                .UseInMemoryDatabase(databaseName: $"LIMS_Test_{Guid.NewGuid()}")
                .Options;

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

            _context = new LIMSContext(options, mockHttpContextAccessor.Object);
            SeedTestData();
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        private void SeedTestData()
        {
            // 1. Standard Organization — Bureau of Indian Standards
            _context.StandardOrganizationMasters.Add(new StandardOrganizationMaster
            {
                ID = StdOrgId_BIS,
                Name = "BIS (Bureau of Indian Standards)",
                NumberType = "None"
            });

            // 2. Metal Classifications
            _context.MetalClassificationMasters.Add(new MetalClassificationMaster
            {
                ID = MetalClassId_CarbonSteel,
                Name = "Carbon Steel",
                Code = "CS",
                HasChemicalParams = true,
                HasMechanicalParams = true,
                SortOrder = 1
            });
            _context.MetalClassificationMasters.Add(new MetalClassificationMaster
            {
                ID = MetalClassId_StainlessSteel,
                Name = "Stainless Steel",
                Code = "SS",
                HasChemicalParams = true,
                HasMechanicalParams = true,
                SortOrder = 2
            });

            // 3. Departments
            _context.DepartmentMasters.Add(new DepartmentMaster
            {
                ID = DeptId_Mechanical,
                Name = "Mechanical Testing",
                Code = "MECH"
            });
            _context.DepartmentMasters.Add(new DepartmentMaster
            {
                ID = DeptId_Chemical,
                Name = "Chemical Testing",
                Code = "CHEM"
            });

            // 4. Equipment
            _context.EquipmentMasters.Add(new EquipmentMaster
            {
                ID = EquipmentId_UTM,
                Name = "Universal Testing Machine",
                EquipmentNo = "UTM-001",
                DepartmentID = DeptId_Mechanical,
                EquipmentTypeID = 1,
                OEMID = 1,
                PurchaseDate = "2020-01-15"
            });
            _context.EquipmentMasters.Add(new EquipmentMaster
            {
                ID = EquipmentId_BendMachine,
                Name = "Bend Test Machine",
                EquipmentNo = "BND-001",
                DepartmentID = DeptId_Mechanical,
                EquipmentTypeID = 1,
                OEMID = 1,
                PurchaseDate = "2020-03-10"
            });
            _context.EquipmentMasters.Add(new EquipmentMaster
            {
                ID = EquipmentId_Spectrometer,
                Name = "Optical Emission Spectrometer",
                EquipmentNo = "OES-001",
                DepartmentID = DeptId_Chemical,
                EquipmentTypeID = 1,
                OEMID = 1,
                PurchaseDate = "2019-06-20"
            });

            // 5. Parameter Units
            _context.ParameterUnitMasters.Add(new ParameterUnitMaster
            {
                ID = ParamUnitId_MPa,
                Name = "MPa",
                ConversaionFactor = "1"
            });
            _context.ParameterUnitMasters.Add(new ParameterUnitMaster
            {
                ID = ParamUnitId_Percent,
                Name = "%",
                ConversaionFactor = "1"
            });

            // 6. Parameters — mechanical
            _context.ParameterMasters.Add(new ParameterMaster
            {
                ID = ParamId_UTS,
                Name = "Ultimate Tensile Strength",
                AliasName = "UTS",
                ParameterType = "Mechanical",
                Code = "UTS",
                Symbol = "Rm",
                ParameterUnitID = ParamUnitId_MPa,
                DecimalPrecision = 1
            });
            _context.ParameterMasters.Add(new ParameterMaster
            {
                ID = ParamId_YS,
                Name = "Yield Strength",
                AliasName = "YS",
                ParameterType = "Mechanical",
                Code = "YS",
                Symbol = "Re",
                ParameterUnitID = ParamUnitId_MPa,
                DecimalPrecision = 1
            });
            _context.ParameterMasters.Add(new ParameterMaster
            {
                ID = ParamId_Elongation,
                Name = "Elongation",
                AliasName = "EL",
                ParameterType = "Mechanical",
                Code = "EL",
                Symbol = "A",
                ParameterUnitID = ParamUnitId_Percent,
                DecimalPrecision = 1
            });

            // 6b. Parameters — chemical
            _context.ParameterMasters.Add(new ParameterMaster
            {
                ID = ParamId_Carbon,
                Name = "Carbon",
                AliasName = "C",
                ParameterType = "Chemical",
                Code = "C",
                Symbol = "C",
                ParameterUnitID = ParamUnitId_Percent,
                DecimalPrecision = 3
            });
            _context.ParameterMasters.Add(new ParameterMaster
            {
                ID = ParamId_Manganese,
                Name = "Manganese",
                AliasName = "Mn",
                ParameterType = "Chemical",
                Code = "Mn",
                Symbol = "Mn",
                ParameterUnitID = ParamUnitId_Percent,
                DecimalPrecision = 3
            });
            _context.ParameterMasters.Add(new ParameterMaster
            {
                ID = ParamId_Sulphur,
                Name = "Sulphur",
                AliasName = "S",
                ParameterType = "Chemical",
                Code = "S",
                Symbol = "S",
                ParameterUnitID = ParamUnitId_Percent,
                DecimalPrecision = 3
            });
            _context.ParameterMasters.Add(new ParameterMaster
            {
                ID = ParamId_Phosphorus,
                Name = "Phosphorus",
                AliasName = "P",
                ParameterType = "Chemical",
                Code = "P",
                Symbol = "P",
                ParameterUnitID = ParamUnitId_Percent,
                DecimalPrecision = 3
            });

            // 7. Laboratory Tests
            _context.LaboratoryTests.Add(new LaboratoryTest
            {
                ID = LabTestId_Tensile,
                Name = "Tensile Test",
                SubGroup = "TEN",
                LabDepartmentID = DeptId_Mechanical,
                TestCaption = "Tensile Test as per IS 1608:2022"
            });
            _context.LaboratoryTests.Add(new LaboratoryTest
            {
                ID = LabTestId_Bend,
                Name = "Bend Test",
                SubGroup = "BND",
                LabDepartmentID = DeptId_Mechanical,
                TestCaption = "Bend Test as per IS 1599:2019"
            });
            _context.LaboratoryTests.Add(new LaboratoryTest
            {
                ID = LabTestId_Chemical,
                Name = "Chemical Analysis",
                SubGroup = "CHM",
                LabDepartmentID = DeptId_Chemical,
                TestCaption = "Chemical Analysis by OES"
            });

            // 8. Test Method Standards
            _context.TestMethodStandards.Add(new TestMethodStandard
            {
                ID = TestMethodStdId_IS1608,
                Name = "BIS IS 1608:2022",
                StandardOrganisationID = StdOrgId_BIS,
                TestMethodCode = "IS 1608",
                Year = 2022,
                Group = "Mechanical",
                SubGroup = "Tensile",
                TestCategory = TestType.Quantitative,
                Caption = "Metallic materials - Tensile testing at ambient temperature",
                EquipmentID = EquipmentId_UTM
            });
            _context.TestMethodStandards.Add(new TestMethodStandard
            {
                ID = TestMethodStdId_IS1599,
                Name = "BIS IS 1599:2019",
                StandardOrganisationID = StdOrgId_BIS,
                TestMethodCode = "IS 1599",
                Year = 2019,
                Group = "Mechanical",
                SubGroup = "Bend",
                TestCategory = TestType.Qualitative,
                Caption = "Steel - Method for bend test",
                EquipmentID = EquipmentId_BendMachine
            });
            _context.TestMethodStandards.Add(new TestMethodStandard
            {
                ID = TestMethodStdId_IS228,
                Name = "BIS IS 228:2018",
                StandardOrganisationID = StdOrgId_BIS,
                TestMethodCode = "IS 228",
                Year = 2018,
                Group = "Chemical",
                SubGroup = "Spectrometry",
                TestCategory = TestType.Quantitative,
                Caption = "Methods of chemical analysis of steels",
                EquipmentID = EquipmentId_Spectrometer
            });

            // 9. Test Method Specification (material standard)
            _context.TestMethodSpecifications.Add(new TestMethodSpecification
            {
                ID = TestMethodSpecId_IS2062,
                StandardOrganizationID = StdOrgId_BIS,
                TestMethodStandard = "IS 2062",
                Name = "IS 2062:2011 - Hot Rolled Low, Medium and High Tensile Structural Steel",
                Part = "1"
            });

            // 10. Specification Header (IS 2062:2011)
            _context.SpecificationHeaders.Add(new SpecificationHeader
            {
                ID = SpecHeaderId_IS2062,
                AliasName = "IS 2062:2011",
                StandardOrganizationID = StdOrgId_BIS,
                Part = "1",
                StandardYear = "2011",
                IsCustom = false
            });

            // 11. Specification Grades
            _context.SpecificationGrades.Add(new SpecificationGrade
            {
                ID = SpecGradeId_E250BR,
                SpecificationHeaderID = SpecHeaderId_IS2062,
                Grade = "E250 BR",
                MetalClassificationID = MetalClassId_CarbonSteel
            });
            // A second grade linked to stainless steel (for filtering test)
            _context.SpecificationGrades.Add(new SpecificationGrade
            {
                ID = SpecGradeId_E250BR_SS,
                SpecificationHeaderID = SpecHeaderId_IS2062,
                Grade = "E250 BR-SS",
                MetalClassificationID = MetalClassId_StainlessSteel
            });

            // 12. Specification Lines — mechanical parameters for E250 BR
            _context.SpecificationLines.Add(new SpecificationLine
            {
                ID = 1,
                SpecificationGradeID = SpecGradeId_E250BR,
                ParameterID = ParamId_UTS,
                MinValue = 410m,
                MaxValue = 540m,
                ParameterUnitID = ParamUnitId_MPa,
                Type = "mechanical",
                Notes = "UTS range for E250 BR, thickness <= 20mm"
            });
            _context.SpecificationLines.Add(new SpecificationLine
            {
                ID = 2,
                SpecificationGradeID = SpecGradeId_E250BR,
                ParameterID = ParamId_YS,
                MinValue = 250m,
                MaxValue = null, // YS is min-only
                ParameterUnitID = ParamUnitId_MPa,
                Type = "mechanical",
                Notes = "Min yield strength for E250 BR"
            });
            _context.SpecificationLines.Add(new SpecificationLine
            {
                ID = 3,
                SpecificationGradeID = SpecGradeId_E250BR,
                ParameterID = ParamId_Elongation,
                MinValue = 23m,
                MaxValue = null, // Elongation is min-only
                ParameterUnitID = ParamUnitId_Percent,
                Type = "mechanical",
                Notes = "Min elongation % for E250 BR gauge length 5.65*sqrt(So)"
            });

            // 12b. Specification Lines — chemical parameters for E250 BR
            _context.SpecificationLines.Add(new SpecificationLine
            {
                ID = 4,
                SpecificationGradeID = SpecGradeId_E250BR,
                ParameterID = ParamId_Carbon,
                MinValue = null,
                MaxValue = 0.220m,
                ParameterUnitID = ParamUnitId_Percent,
                Type = "chemical",
                Notes = "Max carbon for E250 BR"
            });
            _context.SpecificationLines.Add(new SpecificationLine
            {
                ID = 5,
                SpecificationGradeID = SpecGradeId_E250BR,
                ParameterID = ParamId_Manganese,
                MinValue = null,
                MaxValue = 1.500m,
                ParameterUnitID = ParamUnitId_Percent,
                Type = "chemical",
                Notes = "Max manganese for E250 BR"
            });
            _context.SpecificationLines.Add(new SpecificationLine
            {
                ID = 6,
                SpecificationGradeID = SpecGradeId_E250BR,
                ParameterID = ParamId_Sulphur,
                MinValue = null,
                MaxValue = 0.045m,
                ParameterUnitID = ParamUnitId_Percent,
                Type = "chemical",
                Notes = "Max sulphur for E250 BR"
            });
            _context.SpecificationLines.Add(new SpecificationLine
            {
                ID = 7,
                SpecificationGradeID = SpecGradeId_E250BR,
                ParameterID = ParamId_Phosphorus,
                MinValue = null,
                MaxValue = 0.045m,
                ParameterUnitID = ParamUnitId_Percent,
                Type = "chemical",
                Notes = "Max phosphorus for E250 BR"
            });

            _context.SaveChanges();
        }

        #region Helper Methods

        private ProductSpecificationRepository CreateProductSpecRepo()
        {
            return new ProductSpecificationRepository(_context);
        }

        private ProductSpecificationService CreateProductSpecService()
        {
            var repo = CreateProductSpecRepo();
            var logger = new Mock<ILogger<ProductSpecificationService>>();
            return new ProductSpecificationService(repo, logger.Object);
        }

        private ProductTestGroupRepository CreateTestGroupRepo()
        {
            return new ProductTestGroupRepository(_context);
        }

        private ProductTestGroupService CreateTestGroupService()
        {
            var repo = CreateTestGroupRepo();
            var logger = new Mock<ILogger<ProductTestGroupService>>();
            return new ProductTestGroupService(repo, logger.Object);
        }

        private ProductSpecificationGradeRepository CreateSpecGradeRepo()
        {
            return new ProductSpecificationGradeRepository(_context);
        }

        private ProductSpecificationGradeService CreateSpecGradeService()
        {
            var repo = CreateSpecGradeRepo();
            var logger = new Mock<ILogger<ProductSpecificationGradeService>>();
            return new ProductSpecificationGradeService(repo, logger.Object);
        }

        #endregion

        // ---------------------------------------------------------------
        // TEST 1: Verify Product Specification creates correctly
        // ---------------------------------------------------------------
        [Test]
        public async Task CreateProductSpecification_WithValidData_ShouldSucceed()
        {
            // Arrange
            var service = CreateProductSpecService();
            var productSpec = new ProductSpecification
            {
                SpecificationName = "IS 2062 E250 BR - Tensile",
                AliasName = "2062-E250-TEN",
                SpecificationCode = "PS-001",
                GradeID = SpecGradeId_E250BR,
                LaboratoryTestID = LabTestId_Tensile,
                MetalClassificationID = MetalClassId_CarbonSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = false
            };

            // Act
            await service.CreateProductSpecification(productSpec);

            // Assert
            var saved = await _context.ProductSpecifications
                .FirstOrDefaultAsync(x => x.SpecificationName == "IS 2062 E250 BR - Tensile" && x.IsActive);

            Assert.That(saved, Is.Not.Null, "Product specification should be saved in the database");
            Assert.That(saved!.SpecificationCode, Is.EqualTo("PS-001"));
            Assert.That(saved.GradeID, Is.EqualTo(SpecGradeId_E250BR));
            Assert.That(saved.LaboratoryTestID, Is.EqualTo(LabTestId_Tensile));
            Assert.That(saved.MetalClassificationID, Is.EqualTo(MetalClassId_CarbonSteel));
            Assert.That(saved.TestMethodSpecificationID, Is.EqualTo(TestMethodSpecId_IS2062));
            Assert.That(saved.IsCustom, Is.False);
            Assert.That(saved.IsActive, Is.True);
        }

        // ---------------------------------------------------------------
        // TEST 2: Verify Product Test Groups link correctly
        // ---------------------------------------------------------------
        [Test]
        public async Task AddTestGroup_ToProductSpec_ShouldLinkCorrectly()
        {
            // Arrange — create the product specification first
            var specService = CreateProductSpecService();
            var productSpec = new ProductSpecification
            {
                SpecificationName = "IS 2062 E250 BR - Full",
                AliasName = "2062-E250-FULL",
                SpecificationCode = "PS-002",
                GradeID = SpecGradeId_E250BR,
                LaboratoryTestID = LabTestId_Tensile,
                MetalClassificationID = MetalClassId_CarbonSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = false
            };
            await specService.CreateProductSpecification(productSpec);
            var savedSpec = await _context.ProductSpecifications
                .FirstAsync(x => x.SpecificationName == "IS 2062 E250 BR - Full");

            // Act — add three test groups (Tensile, Bend, Chemical)
            var testGroupService = CreateTestGroupService();

            var tensileGroup = new ProductTestGroup
            {
                ProductSpecificationID = savedSpec.ID,
                LaboratoryTestID = LabTestId_Tensile,
                TestMethodStandardID = TestMethodStdId_IS1608,
                IsPerBatch = false,
                Year = 2022,
                Remark = "Tensile test as per IS 1608:2022"
            };
            await testGroupService.CreateProductTestGroup(tensileGroup);

            var bendGroup = new ProductTestGroup
            {
                ProductSpecificationID = savedSpec.ID,
                LaboratoryTestID = LabTestId_Bend,
                TestMethodStandardID = TestMethodStdId_IS1599,
                IsPerBatch = false,
                Year = 2019,
                Remark = "Bend test as per IS 1599:2019"
            };
            await testGroupService.CreateProductTestGroup(bendGroup);

            var chemGroup = new ProductTestGroup
            {
                ProductSpecificationID = savedSpec.ID,
                LaboratoryTestID = LabTestId_Chemical,
                TestMethodStandardID = TestMethodStdId_IS228,
                IsPerBatch = true,
                Year = 2018,
                Remark = "Chemical analysis as per IS 228:2018"
            };
            await testGroupService.CreateProductTestGroup(chemGroup);

            // Assert
            var groups = await _context.ProductTestGroups
                .Where(x => x.ProductSpecificationID == savedSpec.ID && x.IsActive)
                .ToListAsync();

            Assert.That(groups, Has.Count.EqualTo(3), "Should have 3 test groups linked");

            var tensile = groups.First(g => g.LaboratoryTestID == LabTestId_Tensile);
            Assert.That(tensile.TestMethodStandardID, Is.EqualTo(TestMethodStdId_IS1608));
            Assert.That(tensile.Year, Is.EqualTo(2022));
            Assert.That(tensile.IsPerBatch, Is.False);

            var chem = groups.First(g => g.LaboratoryTestID == LabTestId_Chemical);
            Assert.That(chem.IsPerBatch, Is.True, "Chemical analysis should be per-batch");
        }

        // ---------------------------------------------------------------
        // TEST 3: Verify Product Specification Grades link correctly
        // ---------------------------------------------------------------
        [Test]
        public async Task AddSpecGrade_ToProductSpec_ShouldMapToCorrectGrade()
        {
            // Arrange
            var specService = CreateProductSpecService();
            var productSpec = new ProductSpecification
            {
                SpecificationName = "IS 2062 E250 BR - Grade Link",
                AliasName = "2062-E250-GL",
                SpecificationCode = "PS-003",
                GradeID = SpecGradeId_E250BR,
                LaboratoryTestID = LabTestId_Tensile,
                MetalClassificationID = MetalClassId_CarbonSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = false
            };
            await specService.CreateProductSpecification(productSpec);
            var savedSpec = await _context.ProductSpecifications
                .FirstAsync(x => x.SpecificationName == "IS 2062 E250 BR - Grade Link");

            // Act
            var gradeService = CreateSpecGradeService();
            var specGrade = new ProductSpecificationGrade
            {
                ProductSpecificationID = savedSpec.ID,
                SpecificationGradeID = SpecGradeId_E250BR,
                AliasName = "E250 BR Carbon Steel"
            };
            await gradeService.CreateProductSpecificationGrade(specGrade);

            // Assert
            var saved = await _context.ProductSpecificationGrades
                .FirstOrDefaultAsync(x => x.ProductSpecificationID == savedSpec.ID && x.IsActive);

            Assert.That(saved, Is.Not.Null);
            Assert.That(saved!.SpecificationGradeID, Is.EqualTo(SpecGradeId_E250BR));
            Assert.That(saved.AliasName, Is.EqualTo("E250 BR Carbon Steel"));

            // Verify the linked specification grade has correct data
            var grade = await _context.SpecificationGrades.FindAsync(saved.SpecificationGradeID);
            Assert.That(grade, Is.Not.Null);
            Assert.That(grade!.Grade, Is.EqualTo("E250 BR"));
            Assert.That(grade.MetalClassificationID, Is.EqualTo(MetalClassId_CarbonSteel));
        }

        // ---------------------------------------------------------------
        // TEST 4: Verify Smart Suggest returns tests for spec grade
        // (Tests the repository GetByProductSpecificationId method)
        // ---------------------------------------------------------------
        [Test]
        public async Task SmartSuggest_WithSpecGradeId_ShouldReturnLinkedTests()
        {
            // Arrange — create product spec with test groups
            var specService = CreateProductSpecService();
            var productSpec = new ProductSpecification
            {
                SpecificationName = "IS 2062 E250 BR - Smart Suggest",
                AliasName = "2062-E250-SS",
                SpecificationCode = "PS-004",
                GradeID = SpecGradeId_E250BR,
                LaboratoryTestID = LabTestId_Tensile,
                MetalClassificationID = MetalClassId_CarbonSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = false
            };
            await specService.CreateProductSpecification(productSpec);
            var savedSpec = await _context.ProductSpecifications
                .FirstAsync(x => x.SpecificationName == "IS 2062 E250 BR - Smart Suggest");

            // Add test groups
            var testGroupService = CreateTestGroupService();
            await testGroupService.CreateProductTestGroup(new ProductTestGroup
            {
                ProductSpecificationID = savedSpec.ID,
                LaboratoryTestID = LabTestId_Tensile,
                TestMethodStandardID = TestMethodStdId_IS1608,
                IsPerBatch = false,
                Year = 2022
            });
            await testGroupService.CreateProductTestGroup(new ProductTestGroup
            {
                ProductSpecificationID = savedSpec.ID,
                LaboratoryTestID = LabTestId_Chemical,
                TestMethodStandardID = TestMethodStdId_IS228,
                IsPerBatch = true,
                Year = 2018
            });

            // Act — fetch test groups by product specification ID
            var results = await testGroupService.GetByProductSpecificationId(savedSpec.ID);

            // Assert
            Assert.That(results, Has.Count.EqualTo(2), "Should return 2 linked test groups");

            // Verify the results contain the correct lab test names (via anonymous projection)
            var resultJson = System.Text.Json.JsonSerializer.Serialize(results);
            Assert.That(resultJson, Does.Contain("Tensile Test"), "Should include Tensile Test");
            Assert.That(resultJson, Does.Contain("Chemical Analysis"), "Should include Chemical Analysis");
        }

        // ---------------------------------------------------------------
        // TEST 5: Verify parameters load with min/max from spec lines
        // ---------------------------------------------------------------
        [Test]
        public async Task LoadParameters_WithSpecGrade_ShouldHaveMinMaxFromSpecLines()
        {
            // Act — query spec lines for E250 BR grade
            var specLines = await _context.SpecificationLines
                .Where(sl => sl.SpecificationGradeID == SpecGradeId_E250BR)
                .Include(sl => sl.Parameter)
                .Include(sl => sl.ParameterUnit)
                .OrderBy(sl => sl.Type)
                .ThenBy(sl => sl.ParameterID)
                .ToListAsync();

            // Assert — verify all 7 parameters seeded
            Assert.That(specLines, Has.Count.EqualTo(7), "E250 BR should have 7 specification lines (3 mechanical + 4 chemical)");

            // Mechanical parameters
            var utsLine = specLines.First(sl => sl.ParameterID == ParamId_UTS);
            Assert.That(utsLine.MinValue, Is.EqualTo(410m), "UTS min should be 410 MPa");
            Assert.That(utsLine.MaxValue, Is.EqualTo(540m), "UTS max should be 540 MPa");
            Assert.That(utsLine.Type, Is.EqualTo("mechanical"));
            Assert.That(utsLine.Parameter?.Name, Is.EqualTo("Ultimate Tensile Strength"));
            Assert.That(utsLine.ParameterUnit?.Name, Is.EqualTo("MPa"));

            var ysLine = specLines.First(sl => sl.ParameterID == ParamId_YS);
            Assert.That(ysLine.MinValue, Is.EqualTo(250m), "YS min should be 250 MPa");
            Assert.That(ysLine.MaxValue, Is.Null, "YS has no max value (min-only)");

            var elLine = specLines.First(sl => sl.ParameterID == ParamId_Elongation);
            Assert.That(elLine.MinValue, Is.EqualTo(23m), "Elongation min should be 23%");
            Assert.That(elLine.MaxValue, Is.Null, "Elongation has no max value");
            Assert.That(elLine.ParameterUnit?.Name, Is.EqualTo("%"));

            // Chemical parameters
            var carbonLine = specLines.First(sl => sl.ParameterID == ParamId_Carbon);
            Assert.That(carbonLine.MinValue, Is.Null, "Carbon has no min (max-only)");
            Assert.That(carbonLine.MaxValue, Is.EqualTo(0.220m), "Carbon max should be 0.220%");
            Assert.That(carbonLine.Type, Is.EqualTo("chemical"));

            var sulphurLine = specLines.First(sl => sl.ParameterID == ParamId_Sulphur);
            Assert.That(sulphurLine.MaxValue, Is.EqualTo(0.045m), "Sulphur max should be 0.045%");

            var phosphorusLine = specLines.First(sl => sl.ParameterID == ParamId_Phosphorus);
            Assert.That(phosphorusLine.MaxValue, Is.EqualTo(0.045m), "Phosphorus max should be 0.045%");
        }

        // ---------------------------------------------------------------
        // TEST 6: Verify Product Specification dropdown filtered by metal classification
        // ---------------------------------------------------------------
        [Test]
        public async Task Dropdown_FilteredByMetalClassification_ShouldReturnOnlyMatching()
        {
            // Arrange — create two product specs with different metal classifications
            var repo = CreateProductSpecRepo();

            var carbonSteelSpec = new ProductSpecification
            {
                SpecificationName = "CS Plate Tensile Spec",
                AliasName = "CS-TEN",
                SpecificationCode = "PS-CS-01",
                GradeID = SpecGradeId_E250BR,
                LaboratoryTestID = LabTestId_Tensile,
                MetalClassificationID = MetalClassId_CarbonSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = false
            };
            await repo.AddProductSpecification(carbonSteelSpec);

            var stainlessSteelSpec = new ProductSpecification
            {
                SpecificationName = "SS Plate Tensile Spec",
                AliasName = "SS-TEN",
                SpecificationCode = "PS-SS-01",
                GradeID = SpecGradeId_E250BR_SS,
                LaboratoryTestID = LabTestId_Tensile,
                MetalClassificationID = MetalClassId_StainlessSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = false
            };
            await repo.AddProductSpecification(stainlessSteelSpec);

            // Act — query product specs filtered by metal classification
            var carbonSteelSpecs = await _context.ProductSpecifications
                .Where(ps => ps.IsActive && ps.MetalClassificationID == MetalClassId_CarbonSteel)
                .ToListAsync();

            var stainlessSteelSpecs = await _context.ProductSpecifications
                .Where(ps => ps.IsActive && ps.MetalClassificationID == MetalClassId_StainlessSteel)
                .ToListAsync();

            // Assert
            Assert.That(carbonSteelSpecs, Has.Count.EqualTo(1));
            Assert.That(carbonSteelSpecs[0].SpecificationName, Is.EqualTo("CS Plate Tensile Spec"));

            Assert.That(stainlessSteelSpecs, Has.Count.EqualTo(1));
            Assert.That(stainlessSteelSpecs[0].SpecificationName, Is.EqualTo("SS Plate Tensile Spec"));
        }

        // ---------------------------------------------------------------
        // TEST 7: Verify IsCustom flag separates standard from custom
        // ---------------------------------------------------------------
        [Test]
        public async Task CustomList_ShouldReturnOnlyCustomSpecs()
        {
            // Arrange
            var repo = CreateProductSpecRepo();

            var standardSpec = new ProductSpecification
            {
                SpecificationName = "Standard IS 2062 Spec",
                AliasName = "STD-2062",
                SpecificationCode = "PS-STD-01",
                GradeID = SpecGradeId_E250BR,
                LaboratoryTestID = LabTestId_Tensile,
                MetalClassificationID = MetalClassId_CarbonSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = false
            };
            await repo.AddProductSpecification(standardSpec);

            var customSpec = new ProductSpecification
            {
                SpecificationName = "Custom Client XYZ Spec",
                AliasName = "CUST-XYZ",
                SpecificationCode = "PS-CUST-01",
                GradeID = SpecGradeId_E250BR,
                LaboratoryTestID = LabTestId_Tensile,
                MetalClassificationID = MetalClassId_CarbonSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = true
            };
            await repo.AddProductSpecification(customSpec);

            // Act
            var allStandard = await _context.ProductSpecifications
                .Where(ps => ps.IsActive && !ps.IsCustom)
                .ToListAsync();

            var allCustom = await _context.ProductSpecifications
                .Where(ps => ps.IsActive && ps.IsCustom)
                .ToListAsync();

            // Assert
            Assert.That(allStandard, Has.Count.EqualTo(1));
            Assert.That(allStandard[0].SpecificationName, Is.EqualTo("Standard IS 2062 Spec"));

            Assert.That(allCustom, Has.Count.EqualTo(1));
            Assert.That(allCustom[0].SpecificationName, Is.EqualTo("Custom Client XYZ Spec"));
            Assert.That(allCustom[0].IsCustom, Is.True);
        }

        // ---------------------------------------------------------------
        // TEST 8: Verify duplicate product spec name prevention
        // ---------------------------------------------------------------
        [Test]
        public async Task AddDuplicateTestGroup_ShouldFail()
        {
            // Arrange
            var service = CreateProductSpecService();
            var spec1 = new ProductSpecification
            {
                SpecificationName = "Duplicate Name Test",
                AliasName = "DUP-1",
                SpecificationCode = "PS-DUP-01",
                GradeID = SpecGradeId_E250BR,
                LaboratoryTestID = LabTestId_Tensile,
                MetalClassificationID = MetalClassId_CarbonSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = false
            };
            await service.CreateProductSpecification(spec1);

            // Act & Assert — creating a second spec with the same name should throw
            var spec2 = new ProductSpecification
            {
                SpecificationName = "Duplicate Name Test",
                AliasName = "DUP-2",
                SpecificationCode = "PS-DUP-02",
                GradeID = SpecGradeId_E250BR,
                LaboratoryTestID = LabTestId_Bend,
                MetalClassificationID = MetalClassId_CarbonSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = false
            };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await service.CreateProductSpecification(spec2));

            Assert.That(ex!.Message, Does.Contain("already exists"),
                "Should reject duplicate product specification name");
        }

        // ---------------------------------------------------------------
        // TEST 9: Verify soft-delete of product specification
        // ---------------------------------------------------------------
        [Test]
        public async Task DeleteProductSpec_ShouldSoftDelete()
        {
            // Arrange
            var service = CreateProductSpecService();
            var productSpec = new ProductSpecification
            {
                SpecificationName = "Soft Delete Test Spec",
                AliasName = "DEL-TEST",
                SpecificationCode = "PS-DEL-01",
                GradeID = SpecGradeId_E250BR,
                LaboratoryTestID = LabTestId_Tensile,
                MetalClassificationID = MetalClassId_CarbonSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = false
            };
            await service.CreateProductSpecification(productSpec);
            var saved = await _context.ProductSpecifications
                .FirstAsync(x => x.SpecificationName == "Soft Delete Test Spec");
            var savedId = saved.ID;

            // Act
            await service.RemoveProductSpecification(savedId);

            // Assert — record should still exist but IsActive = false
            var deleted = await _context.ProductSpecifications
                .FirstOrDefaultAsync(x => x.ID == savedId);
            Assert.That(deleted, Is.Not.Null, "Record should still exist in database (soft delete)");
            Assert.That(deleted!.IsActive, Is.False, "IsActive should be false after soft delete");

            // Verify it no longer appears in active queries
            var activeSpec = await _context.ProductSpecifications
                .FirstOrDefaultAsync(x => x.ID == savedId && x.IsActive);
            Assert.That(activeSpec, Is.Null, "Soft-deleted spec should not appear in active queries");

            // Verify re-creating with the same name is now allowed (original is inactive)
            // The ExistsByName check filters by IsActive, so this should succeed
            var newSpec = new ProductSpecification
            {
                SpecificationName = "Soft Delete Test Spec",
                AliasName = "DEL-TEST-2",
                SpecificationCode = "PS-DEL-02",
                GradeID = SpecGradeId_E250BR,
                LaboratoryTestID = LabTestId_Tensile,
                MetalClassificationID = MetalClassId_CarbonSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = false
            };
            Assert.DoesNotThrowAsync(async () => await service.CreateProductSpecification(newSpec),
                "Should allow creating spec with same name after soft delete");
        }

        // ---------------------------------------------------------------
        // TEST 10: Verify the complete chain
        //   Create spec -> Add test groups -> Add grade link -> Verify spec lines -> Validate limits
        // ---------------------------------------------------------------
        [Test]
        public async Task CompleteChain_CreateSpec_Suggest_LoadParams_VerifyLimits()
        {
            // STEP 1: Create Product Specification for IS 2062 E250 BR Tensile + Chemical
            var specService = CreateProductSpecService();
            var productSpec = new ProductSpecification
            {
                SpecificationName = "IS 2062:2011 E250 BR - Full Chain",
                AliasName = "2062-E250-CHAIN",
                SpecificationCode = "PS-CHAIN-01",
                GradeID = SpecGradeId_E250BR,
                LaboratoryTestID = LabTestId_Tensile,
                MetalClassificationID = MetalClassId_CarbonSteel,
                TestMethodSpecificationID = TestMethodSpecId_IS2062,
                IsCustom = false
            };
            await specService.CreateProductSpecification(productSpec);

            var savedSpec = await _context.ProductSpecifications
                .FirstAsync(x => x.SpecificationName == "IS 2062:2011 E250 BR - Full Chain");
            Assert.That(savedSpec.ID, Is.GreaterThan(0), "Step 1: Product Spec ID should be assigned");

            // STEP 2: Add Test Groups (Tensile + Bend + Chemical)
            var testGroupService = CreateTestGroupService();

            await testGroupService.CreateProductTestGroup(new ProductTestGroup
            {
                ProductSpecificationID = savedSpec.ID,
                LaboratoryTestID = LabTestId_Tensile,
                TestMethodStandardID = TestMethodStdId_IS1608,
                IsPerBatch = false,
                Year = 2022,
                Remark = "IS 1608:2022 - Tensile testing at ambient temperature"
            });
            await testGroupService.CreateProductTestGroup(new ProductTestGroup
            {
                ProductSpecificationID = savedSpec.ID,
                LaboratoryTestID = LabTestId_Bend,
                TestMethodStandardID = TestMethodStdId_IS1599,
                IsPerBatch = false,
                Year = 2019,
                Remark = "IS 1599:2019 - Bend test"
            });
            await testGroupService.CreateProductTestGroup(new ProductTestGroup
            {
                ProductSpecificationID = savedSpec.ID,
                LaboratoryTestID = LabTestId_Chemical,
                TestMethodStandardID = TestMethodStdId_IS228,
                IsPerBatch = true,
                Year = 2018,
                Remark = "IS 228:2018 - Chemical analysis of steels"
            });

            var testGroups = await testGroupService.GetByProductSpecificationId(savedSpec.ID);
            Assert.That(testGroups, Has.Count.EqualTo(3), "Step 2: Should have 3 test groups");

            // STEP 3: Link Product Specification Grade
            var gradeService = CreateSpecGradeService();
            await gradeService.CreateProductSpecificationGrade(new ProductSpecificationGrade
            {
                ProductSpecificationID = savedSpec.ID,
                SpecificationGradeID = SpecGradeId_E250BR,
                AliasName = "E250 BR (IS 2062:2011)"
            });

            var linkedGrades = await _context.ProductSpecificationGrades
                .Where(psg => psg.ProductSpecificationID == savedSpec.ID && psg.IsActive)
                .ToListAsync();
            Assert.That(linkedGrades, Has.Count.EqualTo(1), "Step 3: Should have 1 linked grade");

            // STEP 4: Load specification lines for the grade and verify limits
            var gradeId = linkedGrades[0].SpecificationGradeID;
            var specLines = await _context.SpecificationLines
                .Where(sl => sl.SpecificationGradeID == gradeId)
                .Include(sl => sl.Parameter)
                .ToListAsync();

            Assert.That(specLines, Has.Count.EqualTo(7), "Step 4: E250 BR should have 7 spec lines");

            // STEP 5: Simulate test result validation against specification limits
            var testResults = new Dictionary<string, decimal>
            {
                { "UTS", 475m },       // 410-540 MPa -> PASS
                { "YS", 280m },        // min 250 MPa -> PASS
                { "EL", 25m },         // min 23% -> PASS
                { "C", 0.180m },       // max 0.220% -> PASS
                { "Mn", 1.200m },      // max 1.500% -> PASS
                { "S", 0.030m },       // max 0.045% -> PASS
                { "P", 0.035m }        // max 0.045% -> PASS
            };

            foreach (var specLine in specLines)
            {
                var paramCode = specLine.Parameter?.Code;
                if (paramCode == null || !testResults.ContainsKey(paramCode))
                    continue;

                var result = testResults[paramCode];
                bool passesMin = !specLine.MinValue.HasValue || result >= specLine.MinValue.Value;
                bool passesMax = !specLine.MaxValue.HasValue || result <= specLine.MaxValue.Value;

                Assert.That(passesMin && passesMax, Is.True,
                    $"Step 5: {specLine.Parameter!.Name} result {result} should pass limits " +
                    $"[{specLine.MinValue?.ToString() ?? "N/A"} - {specLine.MaxValue?.ToString() ?? "N/A"}]");
            }

            // STEP 6: Verify failing results are caught
            var failingResults = new Dictionary<string, decimal>
            {
                { "UTS", 550m },       // max 540 -> FAIL
                { "YS", 200m },        // min 250 -> FAIL
                { "C", 0.250m }        // max 0.220 -> FAIL
            };

            int failCount = 0;
            foreach (var specLine in specLines)
            {
                var paramCode = specLine.Parameter?.Code;
                if (paramCode == null || !failingResults.ContainsKey(paramCode))
                    continue;

                var result = failingResults[paramCode];
                bool passesMin = !specLine.MinValue.HasValue || result >= specLine.MinValue.Value;
                bool passesMax = !specLine.MaxValue.HasValue || result <= specLine.MaxValue.Value;

                if (!(passesMin && passesMax))
                    failCount++;
            }

            Assert.That(failCount, Is.EqualTo(3),
                "Step 6: All 3 out-of-range results should fail validation (UTS>540, YS<250, C>0.220)");

            // STEP 7: Verify the specification header chain
            var specGrade = await _context.SpecificationGrades
                .Include(sg => sg.SpecificationLines)
                .FirstAsync(sg => sg.ID == SpecGradeId_E250BR);

            var specHeader = await _context.SpecificationHeaders
                .FirstAsync(sh => sh.ID == specGrade.SpecificationHeaderID);

            Assert.That(specHeader.AliasName, Is.EqualTo("IS 2062:2011"),
                "Step 7: Specification header should be IS 2062:2011");
            Assert.That(specGrade.Grade, Is.EqualTo("E250 BR"),
                "Step 7: Grade should be E250 BR");
            Assert.That(specGrade.MetalClassificationID, Is.EqualTo(MetalClassId_CarbonSteel),
                "Step 7: Grade should be linked to Carbon Steel classification");

            // STEP 8: Verify product spec can be retrieved with full details
            var details = await specService.GetProductSpecificationDetails(savedSpec.ID);
            Assert.That(details.SpecificationName, Is.EqualTo("IS 2062:2011 E250 BR - Full Chain"));
            Assert.That(details.IsActive, Is.True);
        }
    }
}
