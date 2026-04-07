using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using LIMSApi.Data;
using LIMSApi.Models;
using LIMSApi.Services;
using LIMSApi.Repositories;
using LIMSApi.Dtos;
using Microsoft.Extensions.Logging;

namespace LIMSApi.Tests
{
    /// <summary>
    /// Tests against REAL SQL Server database.
    /// Data persists after test — you can view it in SSMS/Azure Data Studio.
    ///
    /// Run: dotnet test --filter "Category=RealDB"
    /// </summary>
    [TestFixture]
    [Category("RealDB")]
    public class ProductSpecificationRealDbTests
    {
        private LIMSContext _context = null!;
        private string _connectionString = null!;

        // Track IDs of created test data for verification
        private long _createdSpecId;
        private long _createdTestGroupId1;
        private long _createdTestGroupId2;
        private long _createdTestGroupId3;
        private long _createdSpecGradeId;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Read connection string from appsettings.Test.json
            var config = new ConfigurationBuilder()
                .SetBasePath(TestContext.CurrentContext.TestDirectory)
                .AddJsonFile("appsettings.Test.json", optional: false)
                .Build();

            _connectionString = config.GetConnectionString("DefaultConnection")!;
            Assert.That(_connectionString, Is.Not.Null.And.Not.Empty, "Connection string must be configured");

            var options = new DbContextOptionsBuilder<LIMSContext>()
                .UseSqlServer(_connectionString)
                .Options;

            var mockHttp = new Mock<IHttpContextAccessor>();
            mockHttp.Setup(x => x.HttpContext).Returns((HttpContext?)null);

            _context = new LIMSContext(options, mockHttp.Object);

            // Verify DB is accessible
            Assert.That(_context.Database.CanConnect(), Is.True, "Cannot connect to database");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Print summary of created data
            TestContext.WriteLine("╔══════════════════════════════════════════════════════╗");
            TestContext.WriteLine("║  TEST DATA CREATED IN REAL DATABASE                  ║");
            TestContext.WriteLine("╠══════════════════════════════════════════════════════╣");
            TestContext.WriteLine($"║  Product Specification ID: {_createdSpecId,-25}║");
            TestContext.WriteLine($"║  Test Group IDs: {_createdTestGroupId1}, {_createdTestGroupId2}, {_createdTestGroupId3,-14}║");
            TestContext.WriteLine($"║  Spec Grade Mapping ID: {_createdSpecGradeId,-22}║");
            TestContext.WriteLine("╠══════════════════════════════════════════════════════╣");
            TestContext.WriteLine("║  Data persists in DB — view in SSMS:                ║");
            TestContext.WriteLine("║  SELECT * FROM ProductSpecifications                ║");
            TestContext.WriteLine("║  SELECT * FROM ProductTestGroups                    ║");
            TestContext.WriteLine("║  SELECT * FROM ProductSpecificationGrades           ║");
            TestContext.WriteLine("║  SELECT * FROM SpecificationGrades                  ║");
            TestContext.WriteLine("║  SELECT * FROM SpecificationLines                   ║");
            TestContext.WriteLine("╚══════════════════════════════════════════════════════╝");

            _context?.Dispose();
        }

        // ── Helpers ──
        private ProductSpecificationRepository CreateRepo() => new(_context);
        private ProductSpecificationService CreateService()
        {
            var logger = new Mock<ILogger<ProductSpecificationService>>();
            return new ProductSpecificationService(CreateRepo(), logger.Object);
        }
        private ProductTestGroupService CreateTestGroupService()
        {
            var repo = new ProductTestGroupRepository(_context);
            var logger = new Mock<ILogger<ProductTestGroupService>>();
            return new ProductTestGroupService(repo, logger.Object);
        }
        private ProductSpecificationGradeService CreateSpecGradeService()
        {
            var repo = new ProductSpecificationGradeRepository(_context);
            var logger = new Mock<ILogger<ProductSpecificationGradeService>>();
            return new ProductSpecificationGradeService(repo, logger.Object);
        }

        // ═══════════════════════════════════════════════
        // TEST 1: Verify prerequisites exist in real DB
        // ═══════════════════════════════════════════════
        [Test, Order(1)]
        public async Task T01_VerifyPrerequisitesExist()
        {
            // Metal Classification must exist
            var metalClass = await _context.MetalClassificationMasters
                .FirstOrDefaultAsync(m => m.IsActive);
            Assert.That(metalClass, Is.Not.Null, "At least one Metal Classification must exist in DB");
            TestContext.WriteLine($"  Metal Classification: {metalClass!.Name} (ID: {metalClass.ID})");

            // Specification Header must exist
            var specHeader = await _context.SpecificationHeaders
                .FirstOrDefaultAsync(s => s.IsActive);
            Assert.That(specHeader, Is.Not.Null, "At least one Specification Header (Material Spec) must exist");
            TestContext.WriteLine($"  Specification Header: {specHeader!.AliasName} (ID: {specHeader.ID})");

            // Specification Grade must exist
            var specGrade = await _context.SpecificationGrades
                .Include(g => g.SpecificationLines)
                .FirstOrDefaultAsync();
            Assert.That(specGrade, Is.Not.Null, "At least one Specification Grade must exist");
            TestContext.WriteLine($"  Grade: {specGrade!.Grade} (ID: {specGrade.ID}, Lines: {specGrade.SpecificationLines.Count})");

            // Laboratory Test must exist
            var labTest = await _context.LaboratoryTests
                .FirstOrDefaultAsync(t => t.IsActive);
            Assert.That(labTest, Is.Not.Null, "At least one Laboratory Test must exist");
            TestContext.WriteLine($"  Laboratory Test: {labTest!.Name} (ID: {labTest.ID})");

            // Test Method Specification must exist
            var testMethodSpec = await _context.TestMethodSpecifications
                .FirstOrDefaultAsync(t => t.IsActive);
            Assert.That(testMethodSpec, Is.Not.Null, "At least one Test Method Specification must exist");
            TestContext.WriteLine($"  Test Method Spec: {testMethodSpec!.Name} (ID: {testMethodSpec.ID})");
        }

        // ═══════════════════════════════════════════════
        // TEST 2: Create Product Specification with real data
        // ═══════════════════════════════════════════════
        [Test, Order(2)]
        public async Task T02_CreateProductSpecification()
        {
            // Get first available real data
            var metalClass = await _context.MetalClassificationMasters.FirstAsync(m => m.IsActive);
            var grade = await _context.SpecificationGrades.FirstAsync();
            var labTest = await _context.LaboratoryTests.FirstAsync(t => t.IsActive);
            var testMethodSpec = await _context.TestMethodSpecifications.FirstAsync(t => t.IsActive);

            var uniqueName = $"NUnit Test Spec - {DateTime.Now:yyyyMMdd-HHmmss}";

            var service = CreateService();
            var spec = new ProductSpecification
            {
                SpecificationName = uniqueName,
                AliasName = $"TEST-{DateTime.Now:HHmmss}",
                SpecificationCode = $"PS-TEST-{DateTime.Now:HHmmss}",
                GradeID = grade.ID,
                LaboratoryTestID = labTest.ID,
                MetalClassificationID = metalClass.ID,
                TestMethodSpecificationID = testMethodSpec.ID,
                IsCustom = false,
                Size = "12-25mm"
            };

            await service.CreateProductSpecification(spec);

            // Verify
            var saved = await _context.ProductSpecifications
                .FirstOrDefaultAsync(x => x.SpecificationName == uniqueName && x.IsActive);

            Assert.That(saved, Is.Not.Null, "Product Specification should be saved in real DB");
            _createdSpecId = saved!.ID;

            TestContext.WriteLine($"  Created Product Spec: '{uniqueName}'");
            TestContext.WriteLine($"  ID: {saved.ID}");
            TestContext.WriteLine($"  Grade: {grade.Grade} (ID: {grade.ID})");
            TestContext.WriteLine($"  Lab Test: {labTest.Name} (ID: {labTest.ID})");
            TestContext.WriteLine($"  Metal: {metalClass.Name} (ID: {metalClass.ID})");
        }

        // ═══════════════════════════════════════════════
        // TEST 3: Add Test Groups to the created spec
        // ═══════════════════════════════════════════════
        [Test, Order(3)]
        public async Task T03_AddTestGroups()
        {
            Assert.That(_createdSpecId, Is.GreaterThan(0), "Product Spec must be created first (T02)");

            var labTests = await _context.LaboratoryTests
                .Where(t => t.IsActive)
                .Take(3)
                .ToListAsync();

            Assert.That(labTests.Count, Is.GreaterThanOrEqualTo(1), "At least 1 lab test required");

            var testMethodStandards = await _context.TestMethodStandards
                .Where(t => t.IsActive)
                .Take(3)
                .ToListAsync();

            var service = CreateTestGroupService();

            // Add up to 3 test groups
            for (int i = 0; i < Math.Min(labTests.Count, 3); i++)
            {
                var group = new ProductTestGroup
                {
                    ProductSpecificationID = _createdSpecId,
                    LaboratoryTestID = labTests[i].ID,
                    TestMethodStandardID = testMethodStandards.ElementAtOrDefault(i)?.ID,
                    IsPerBatch = i == 2, // Third test is per-batch
                    Year = DateTime.Now.Year,
                    Remark = $"NUnit test group {i + 1}: {labTests[i].Name}"
                };

                await service.CreateProductTestGroup(group);

                if (i == 0) _createdTestGroupId1 = group.ID;
                if (i == 1) _createdTestGroupId2 = group.ID;
                if (i == 2) _createdTestGroupId3 = group.ID;

                TestContext.WriteLine($"  Added Test Group {i + 1}: {labTests[i].Name} (ID: {group.ID})");
            }

            // Verify
            var groups = await _context.ProductTestGroups
                .Where(g => g.ProductSpecificationID == _createdSpecId && g.IsActive)
                .ToListAsync();

            Assert.That(groups.Count, Is.GreaterThanOrEqualTo(1));
            TestContext.WriteLine($"  Total Test Groups linked: {groups.Count}");
        }

        // ═══════════════════════════════════════════════
        // TEST 4: Map Specification Grade
        // ═══════════════════════════════════════════════
        [Test, Order(4)]
        public async Task T04_MapSpecificationGrade()
        {
            Assert.That(_createdSpecId, Is.GreaterThan(0), "Product Spec must be created first (T02)");

            var grade = await _context.SpecificationGrades.FirstAsync();

            var service = CreateSpecGradeService();
            var mapping = new ProductSpecificationGrade
            {
                ProductSpecificationID = _createdSpecId,
                SpecificationGradeID = grade.ID,
                AliasName = $"NUnit Test - {grade.Grade}"
            };

            await service.CreateProductSpecificationGrade(mapping);
            _createdSpecGradeId = mapping.ID;

            // Verify
            var saved = await _context.ProductSpecificationGrades
                .FirstOrDefaultAsync(g => g.ProductSpecificationID == _createdSpecId && g.IsActive);

            Assert.That(saved, Is.Not.Null);
            TestContext.WriteLine($"  Mapped Grade: {grade.Grade} (GradeID: {grade.ID}) → Spec (ID: {_createdSpecId})");
        }

        // ═══════════════════════════════════════════════
        // TEST 5: Verify Spec Lines have min/max values
        // ═══════════════════════════════════════════════
        [Test, Order(5)]
        public async Task T05_VerifySpecLines()
        {
            var grade = await _context.SpecificationGrades.FirstAsync();

            var lines = await _context.SpecificationLines
                .Where(l => l.SpecificationGradeID == grade.ID)
                .Include(l => l.Parameter)
                .Include(l => l.ParameterUnit)
                .ToListAsync();

            TestContext.WriteLine($"  Grade: {grade.Grade} (ID: {grade.ID})");
            TestContext.WriteLine($"  Spec Lines: {lines.Count}");
            TestContext.WriteLine("  ┌────────────────────┬──────────┬──────────┬───────┐");
            TestContext.WriteLine("  │ Parameter          │ Min      │ Max      │ Unit  │");
            TestContext.WriteLine("  ├────────────────────┼──────────┼──────────┼───────┤");

            foreach (var line in lines)
            {
                var paramName = line.Parameter?.Name ?? $"ParamID:{line.ParameterID}";
                var unitName = line.ParameterUnit?.Name ?? "-";
                var min = line.MinValue?.ToString("F3") ?? "-";
                var max = line.MaxValue?.ToString("F3") ?? "-";
                TestContext.WriteLine($"  │ {paramName,-18} │ {min,-8} │ {max,-8} │ {unitName,-5} │");
            }
            TestContext.WriteLine("  └────────────────────┴──────────┴──────────┴───────┘");

            // At least some lines should exist (may be 0 if fresh DB)
            if (lines.Count > 0)
            {
                Assert.That(lines.Any(l => l.MinValue.HasValue || l.MaxValue.HasValue),
                    Is.True, "At least one spec line should have min or max value");
            }
            else
            {
                TestContext.WriteLine("  ⚠ No spec lines found — add via Material Specification form");
            }
        }

        // ═══════════════════════════════════════════════
        // TEST 6: Smart Suggest — query by grade ID
        // ═══════════════════════════════════════════════
        [Test, Order(6)]
        public async Task T06_SmartSuggestByGrade()
        {
            var grade = await _context.SpecificationGrades.FirstAsync();

            // Query ProductSpecifications linked to this grade
            var linkedSpecs = await _context.ProductSpecifications
                .Where(ps => ps.GradeID == grade.ID && ps.IsActive)
                .Include(ps => ps.LaboratoryTest)
                .ToListAsync();

            TestContext.WriteLine($"  Grade: {grade.Grade} (ID: {grade.ID})");
            TestContext.WriteLine($"  Product Specs linked to this grade: {linkedSpecs.Count}");

            foreach (var spec in linkedSpecs)
            {
                TestContext.WriteLine($"    → {spec.SpecificationName} | Test: {spec.LaboratoryTest?.Name}");

                // Get test groups
                var groups = await _context.ProductTestGroups
                    .Where(g => g.ProductSpecificationID == spec.ID && g.IsActive)
                    .Include(g => g.LaboratoryTest)
                    .Include(g => g.TestMethodStandard)
                    .ToListAsync();

                foreach (var g in groups)
                {
                    TestContext.WriteLine($"      └─ TestGroup: {g.LaboratoryTest?.Name} | Method: {g.TestMethodStandard?.Name ?? "-"} | PerBatch: {g.IsPerBatch}");
                }
            }

            // Our test spec should be in the list
            Assert.That(linkedSpecs.Any(s => s.ID == _createdSpecId),
                Is.True, "Created spec should appear in smart suggest results for this grade");
        }

        // ═══════════════════════════════════════════════
        // TEST 7: Verify complete chain — end to end
        // ═══════════════════════════════════════════════
        [Test, Order(7)]
        public async Task T07_CompleteChainVerification()
        {
            Assert.That(_createdSpecId, Is.GreaterThan(0));

            // Load the full chain
            var spec = await _context.ProductSpecifications
                .Include(s => s.MetalClassification)
                .Include(s => s.LaboratoryTest)
                .Include(s => s.ProductTestGroups).ThenInclude(g => g.LaboratoryTest)
                .Include(s => s.ProductTestGroups).ThenInclude(g => g.TestMethodStandard)
                .Include(s => s.ProductSpecificationGrades).ThenInclude(g => g.SpecificationGrade)
                .FirstOrDefaultAsync(s => s.ID == _createdSpecId);

            Assert.That(spec, Is.Not.Null);

            TestContext.WriteLine("═══════════════════════════════════════════════════════");
            TestContext.WriteLine("  COMPLETE CHAIN VERIFICATION");
            TestContext.WriteLine("═══════════════════════════════════════════════════════");
            TestContext.WriteLine($"  Product Specification: {spec!.SpecificationName}");
            TestContext.WriteLine($"  Code: {spec.SpecificationCode}");
            TestContext.WriteLine($"  Metal Classification: {spec.MetalClassification?.Name ?? "N/A"}");
            TestContext.WriteLine($"  Laboratory Test: {spec.LaboratoryTest?.Name ?? "N/A"}");
            TestContext.WriteLine($"  Size: {spec.Size ?? "-"}");
            TestContext.WriteLine($"  IsCustom: {spec.IsCustom}");
            TestContext.WriteLine();

            TestContext.WriteLine($"  Test Groups ({spec.ProductTestGroups.Count(g => g.IsActive)}):");
            foreach (var g in spec.ProductTestGroups.Where(g => g.IsActive))
            {
                TestContext.WriteLine($"    ├─ {g.LaboratoryTest?.Name ?? "?"} | {g.TestMethodStandard?.Name ?? "-"} | PerBatch: {g.IsPerBatch}");
            }

            TestContext.WriteLine($"  Spec Grades ({spec.ProductSpecificationGrades.Count(g => g.IsActive)}):");
            foreach (var g in spec.ProductSpecificationGrades.Where(g => g.IsActive))
            {
                TestContext.WriteLine($"    ├─ {g.SpecificationGrade?.Grade ?? "?"} ({g.AliasName ?? "-"})");
            }

            // Get spec lines for the grade
            var gradeId = spec.GradeID;
            var lines = await _context.SpecificationLines
                .Where(l => l.SpecificationGradeID == gradeId)
                .Include(l => l.Parameter)
                .ToListAsync();

            TestContext.WriteLine($"  Spec Lines for Grade (ID: {gradeId}): {lines.Count}");
            foreach (var l in lines)
            {
                TestContext.WriteLine($"    ├─ {l.Parameter?.Name ?? "?"}: Min={l.MinValue?.ToString("F3") ?? "-"} Max={l.MaxValue?.ToString("F3") ?? "-"}");
            }

            TestContext.WriteLine("═══════════════════════════════════════════════════════");

            // Final assertions
            Assert.That(spec.MetalClassification, Is.Not.Null, "Metal classification should be loaded");
            Assert.That(spec.LaboratoryTest, Is.Not.Null, "Laboratory test should be loaded");
            Assert.That(spec.ProductTestGroups.Count(g => g.IsActive), Is.GreaterThanOrEqualTo(1), "At least 1 test group");
        }
    }
}
