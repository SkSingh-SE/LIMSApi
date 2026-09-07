using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class ProductMasterService : IProductMasterService
    {
        private readonly IProductMasterRepository _repository;
        private readonly LIMSContext _context;
        private readonly ILogger<ProductMasterService> _logger;
        private readonly LoggedInUserDTO _loggedInUser;

        public ProductMasterService(IProductMasterRepository repository, LIMSContext context, ILogger<ProductMasterService> logger)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<object> CreateProductMaster(ProductMasterCreateDto dto)
        {
            if (await _repository.ExistsByName(dto.ProductName))
            {
                throw new ArgumentException($"Product Master with name '{dto.ProductName}' already exists.");
            }

            var model = new ProductMaster
            {
                ProductSizeMasterID = dto.IsSizeApplicable ? dto.ProductSizeMasterID : null,
                ProductName = dto.ProductName.Trim(),
                GradePrefix = dto.GradePrefix?.Trim(),
                GradeValue = dto.GradeValue?.Trim(),
                DisplayTitle = dto.DisplayTitle?.Trim(),
                IsSizeApplicable = dto.IsSizeApplicable,
                CreatedBy = _loggedInUser?.EmployeeID ?? 0,
                CreatedOn = DateTime.UtcNow,
                CompanyCode = _loggedInUser?.CompanyCode,
                IsActive = true
            };

            if (dto.MetalClassificationIDs != null)
            {
                foreach (var mcId in dto.MetalClassificationIDs.Distinct())
                {
                    model.MetalClassifications.Add(new ProductMasterMetalClassification
                    {
                        MetalClassificationID = mcId
                    });
                }
            }

            if (dto.Versions != null)
            {
                foreach (var vDto in dto.Versions)
                {
                    var versionEntity = new ProductMasterVersion
                    {
                        VersionNumber = vDto.VersionNumber,
                        Year = vDto.Year,
                        SpecificationFilePath = vDto.SpecificationFilePath,
                        StandardOrganizationID = vDto.StandardOrganizationID,
                        SpecStdNo = vDto.SpecStdNo,
                        PartSection = vDto.PartSection,
                        Title = vDto.Title,
                        ProductCaption = vDto.ProductCaption,
                        IsActiveVersion = vDto.IsActiveVersion,
                        CreatedBy = _loggedInUser?.EmployeeID ?? 0,
                        CreatedOn = DateTime.UtcNow,
                        CompanyCode = _loggedInUser?.CompanyCode,
                        IsActive = true
                    };

                    if (vDto.Grades != null)
                    {
                        var dupGrades = vDto.Grades.GroupBy(g => g.SpecificationGradeID).Where(grp => grp.Count() > 1).Select(grp => grp.Key).ToList();
                        if (dupGrades.Any())
                        {
                            throw new ArgumentException($"Duplicate applicability/grade ID(s) [{string.Join(", ", dupGrades)}] found in version '{vDto.VersionNumber}'. Each applicability must be unique per version.");
                        }

                        int sortOrder = 1;
                        foreach (var gDto in vDto.Grades)
                        {
                            var versionGrade = new ProductMasterVersionGrade
                            {
                                SpecificationGradeID = gDto.SpecificationGradeID,
                                SortOrder = gDto.SortOrder > 0 ? gDto.SortOrder : sortOrder++,
                                CreatedBy = _loggedInUser?.EmployeeID ?? 0,
                                CreatedOn = DateTime.UtcNow,
                                CompanyCode = _loggedInUser?.CompanyCode,
                                IsActive = true
                            };

                            if (gDto.Conditions != null)
                            {
                                int priority = 1;
                                foreach (var cDto in gDto.Conditions)
                                {
                                    versionGrade.Conditions.Add(new ProductMasterVersionGradeCondition
                                    {
                                        ProductConditionID1 = cDto.ProductConditionID1,
                                        ProductConditionID2 = cDto.ProductConditionID2,
                                        HeatTreatmentID = cDto.HeatTreatmentID,
                                        ProductSizeMasterID = cDto.ProductSizeMasterID,
                                        Priority = cDto.Priority > 0 ? cDto.Priority : priority++,
                                        CreatedBy = _loggedInUser?.EmployeeID ?? 0,
                                        CreatedOn = DateTime.UtcNow,
                                        CompanyCode = _loggedInUser?.CompanyCode,
                                        IsActive = true
                                    });
                                }
                            }

                            versionEntity.Grades.Add(versionGrade);
                        }
                    }

                    model.Versions.Add(versionEntity);
                }
            }

            await _repository.Add(model);

            if (!string.IsNullOrWhiteSpace(dto.GradePrefix))
            {
                await AddPrefixOption(dto.GradePrefix);
            }

            return new { message = "Product Master created successfully.", id = model.ID };
        }

        public async Task<object> UpdateProductMaster(ProductMasterUpdateDto dto)
        {
            var existing = await _repository.GetById(dto.ID);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Product Master with ID {dto.ID} not found.");
            }

            if (await _repository.ExistsByNameAndNotId(dto.ProductName, dto.ID))
            {
                throw new ArgumentException($"Product Master with name '{dto.ProductName}' already exists.");
            }

            existing.ProductSizeMasterID = dto.IsSizeApplicable ? dto.ProductSizeMasterID : null;
            existing.ProductName = dto.ProductName.Trim();
            existing.GradePrefix = dto.GradePrefix?.Trim();
            existing.GradeValue = dto.GradeValue?.Trim();
            existing.DisplayTitle = dto.DisplayTitle?.Trim();
            existing.IsSizeApplicable = dto.IsSizeApplicable;
            existing.ModifiedBy = _loggedInUser?.EmployeeID ?? 0;
            existing.ModifiedOn = DateTime.UtcNow;

            existing.MetalClassifications.Clear();
            if (dto.MetalClassificationIDs != null)
            {
                foreach (var mcId in dto.MetalClassificationIDs.Distinct())
                {
                    existing.MetalClassifications.Add(new ProductMasterMetalClassification
                    {
                        ProductMasterID = existing.ID,
                        MetalClassificationID = mcId
                    });
                }
            }

            // Non-destructive reconciliation of ProductMasterVersions and child applicability
            if (dto.Versions != null && dto.Versions.Any())
            {
                var incomingVersionIds = dto.Versions.Where(v => v.ID > 0).Select(v => v.ID).ToHashSet();
                var versionsToRemove = existing.Versions.Where(v => !incomingVersionIds.Contains(v.ID)).ToList();
                foreach (var vToRemove in versionsToRemove)
                {
                    _context.ProductMasterVersions.Remove(vToRemove);
                }

                foreach (var vDto in dto.Versions)
                {
                    var dupGrades = vDto.Grades?.GroupBy(g => g.SpecificationGradeID).Where(grp => grp.Count() > 1).Select(grp => grp.Key).ToList();
                    if (dupGrades != null && dupGrades.Any())
                    {
                        throw new ArgumentException($"Duplicate applicability/grade ID(s) [{string.Join(", ", dupGrades)}] found in version '{vDto.VersionNumber}'. Each applicability must be unique per version.");
                    }

                    var existingVersion = vDto.ID > 0 ? existing.Versions.FirstOrDefault(v => v.ID == vDto.ID) : null;
                    if (existingVersion == null)
                    {
                        // New Version Insert
                        var versionEntity = new ProductMasterVersion
                        {
                            ProductMasterID = existing.ID,
                            VersionNumber = vDto.VersionNumber,
                            Year = vDto.Year,
                            SpecificationFilePath = vDto.SpecificationFilePath,
                            StandardOrganizationID = vDto.StandardOrganizationID,
                            SpecStdNo = vDto.SpecStdNo,
                            PartSection = vDto.PartSection,
                            Title = vDto.Title,
                            ProductCaption = vDto.ProductCaption,
                            IsActiveVersion = vDto.IsActiveVersion,
                            CreatedBy = _loggedInUser?.EmployeeID ?? 0,
                            CreatedOn = DateTime.UtcNow,
                            CompanyCode = _loggedInUser?.CompanyCode,
                            IsActive = true
                        };

                        if (vDto.Grades != null)
                        {
                            int sortOrder = 1;
                            foreach (var gDto in vDto.Grades)
                            {
                                var versionGrade = new ProductMasterVersionGrade
                                {
                                    SpecificationGradeID = gDto.SpecificationGradeID,
                                    SortOrder = gDto.SortOrder > 0 ? gDto.SortOrder : sortOrder++,
                                    CreatedBy = _loggedInUser?.EmployeeID ?? 0,
                                    CreatedOn = DateTime.UtcNow,
                                    CompanyCode = _loggedInUser?.CompanyCode,
                                    IsActive = true
                                };

                                if (gDto.Conditions != null)
                                {
                                    int priority = 1;
                                    foreach (var cDto in gDto.Conditions)
                                    {
                                        versionGrade.Conditions.Add(new ProductMasterVersionGradeCondition
                                        {
                                            ProductConditionID1 = cDto.ProductConditionID1,
                                            ProductConditionID2 = cDto.ProductConditionID2,
                                            HeatTreatmentID = cDto.HeatTreatmentID,
                                            ProductSizeMasterID = cDto.ProductSizeMasterID,
                                            Priority = cDto.Priority > 0 ? cDto.Priority : priority++,
                                            CreatedBy = _loggedInUser?.EmployeeID ?? 0,
                                            CreatedOn = DateTime.UtcNow,
                                            CompanyCode = _loggedInUser?.CompanyCode,
                                            IsActive = true
                                        });
                                    }
                                }

                                versionEntity.Grades.Add(versionGrade);
                            }
                        }

                        existing.Versions.Add(versionEntity);
                    }
                    else
                    {
                        // Update Existing Version Metadata (Non-Destructive)
                        existingVersion.VersionNumber = vDto.VersionNumber;
                        existingVersion.Year = vDto.Year;
                        existingVersion.Title = vDto.Title;
                        existingVersion.ProductCaption = vDto.ProductCaption;
                        existingVersion.IsActiveVersion = vDto.IsActiveVersion;
                        existingVersion.StandardOrganizationID = vDto.StandardOrganizationID;
                        existingVersion.SpecStdNo = vDto.SpecStdNo;
                        existingVersion.PartSection = vDto.PartSection;
                        existingVersion.SpecificationFilePath = vDto.SpecificationFilePath;
                        existingVersion.ModifiedBy = _loggedInUser?.EmployeeID ?? 0;
                        existingVersion.ModifiedOn = DateTime.UtcNow;

                        // Reconcile Grades (Applicabilities)
                        if (vDto.Grades != null)
                        {
                            var incomingGradeIds = vDto.Grades.Where(g => g.ID > 0).Select(g => g.ID).ToHashSet();
                            var gradesToRemove = existingVersion.Grades
                                .Where(g => !incomingGradeIds.Contains(g.ID) && !vDto.Grades.Any(x => x.SpecificationGradeID == g.SpecificationGradeID))
                                .ToList();
                            foreach (var gToRemove in gradesToRemove)
                            {
                                _context.ProductMasterVersionGrades.Remove(gToRemove);
                            }

                            int sortOrder = 1;
                            foreach (var gDto in vDto.Grades)
                            {
                                var existingGrade = gDto.ID > 0
                                    ? existingVersion.Grades.FirstOrDefault(g => g.ID == gDto.ID)
                                    : existingVersion.Grades.FirstOrDefault(g => g.SpecificationGradeID == gDto.SpecificationGradeID);

                                if (existingGrade == null)
                                {
                                    var newGrade = new ProductMasterVersionGrade
                                    {
                                        ProductMasterVersionID = existingVersion.ID,
                                        SpecificationGradeID = gDto.SpecificationGradeID,
                                        SortOrder = gDto.SortOrder > 0 ? gDto.SortOrder : sortOrder++,
                                        CreatedBy = _loggedInUser?.EmployeeID ?? 0,
                                        CreatedOn = DateTime.UtcNow,
                                        CompanyCode = _loggedInUser?.CompanyCode,
                                        IsActive = true
                                    };

                                    if (gDto.Conditions != null)
                                    {
                                        int priority = 1;
                                        foreach (var cDto in gDto.Conditions)
                                        {
                                            newGrade.Conditions.Add(new ProductMasterVersionGradeCondition
                                            {
                                                ProductConditionID1 = cDto.ProductConditionID1,
                                                ProductConditionID2 = cDto.ProductConditionID2,
                                                HeatTreatmentID = cDto.HeatTreatmentID,
                                                ProductSizeMasterID = cDto.ProductSizeMasterID,
                                                Priority = cDto.Priority > 0 ? cDto.Priority : priority++,
                                                CreatedBy = _loggedInUser?.EmployeeID ?? 0,
                                                CreatedOn = DateTime.UtcNow,
                                                CompanyCode = _loggedInUser?.CompanyCode,
                                                IsActive = true
                                            });
                                        }
                                    }

                                    existingVersion.Grades.Add(newGrade);
                                }
                                else
                                {
                                    existingGrade.SpecificationGradeID = gDto.SpecificationGradeID;
                                    existingGrade.SortOrder = gDto.SortOrder > 0 ? gDto.SortOrder : sortOrder++;
                                    existingGrade.ModifiedBy = _loggedInUser?.EmployeeID ?? 0;
                                    existingGrade.ModifiedOn = DateTime.UtcNow;

                                    // Reconcile Conditions
                                    if (gDto.Conditions != null)
                                    {
                                        var incomingCondIds = gDto.Conditions.Where(c => c.ID > 0).Select(c => c.ID).ToHashSet();
                                        var condsToRemove = existingGrade.Conditions.Where(c => !incomingCondIds.Contains(c.ID)).ToList();
                                        foreach (var cToRemove in condsToRemove)
                                        {
                                            _context.ProductMasterVersionGradeConditions.Remove(cToRemove);
                                        }

                                        int priority = 1;
                                        foreach (var cDto in gDto.Conditions)
                                        {
                                            var existingCond = cDto.ID > 0 ? existingGrade.Conditions.FirstOrDefault(c => c.ID == cDto.ID) : null;
                                            if (existingCond == null)
                                            {
                                                existingGrade.Conditions.Add(new ProductMasterVersionGradeCondition
                                                {
                                                    ProductMasterVersionGradeID = existingGrade.ID,
                                                    ProductConditionID1 = cDto.ProductConditionID1,
                                                    ProductConditionID2 = cDto.ProductConditionID2,
                                                    HeatTreatmentID = cDto.HeatTreatmentID,
                                                    ProductSizeMasterID = cDto.ProductSizeMasterID,
                                                    Priority = cDto.Priority > 0 ? cDto.Priority : priority++,
                                                    CreatedBy = _loggedInUser?.EmployeeID ?? 0,
                                                    CreatedOn = DateTime.UtcNow,
                                                    CompanyCode = _loggedInUser?.CompanyCode,
                                                    IsActive = true
                                                });
                                            }
                                            else
                                            {
                                                existingCond.ProductConditionID1 = cDto.ProductConditionID1;
                                                existingCond.ProductConditionID2 = cDto.ProductConditionID2;
                                                existingCond.HeatTreatmentID = cDto.HeatTreatmentID;
                                                existingCond.ProductSizeMasterID = cDto.ProductSizeMasterID;
                                                existingCond.Priority = cDto.Priority > 0 ? cDto.Priority : priority++;
                                                existingCond.ModifiedBy = _loggedInUser?.EmployeeID ?? 0;
                                                existingCond.ModifiedOn = DateTime.UtcNow;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            await _repository.Update(existing);

            if (!string.IsNullOrWhiteSpace(dto.GradePrefix))
            {
                await AddPrefixOption(dto.GradePrefix);
            }

            return new { message = "Product Master updated successfully.", id = existing.ID };
        }

        public async Task DeleteProductMaster(long id)
        {
            var existing = await _repository.GetById(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Product Master with ID {id} not found.");
            }
            await _repository.Delete(existing);
        }

        public async Task<ProductMasterDetailsDto?> GetProductMasterById(long id)
        {
            var entity = await _repository.GetDetailsById(id);
            if (entity == null) return null;

            var dto = new ProductMasterDetailsDto
            {
                ID = entity.ID,
                ProductSizeMasterID = entity.ProductSizeMasterID,
                ProductSizeName = entity.ProductSizeMaster != null ? entity.ProductSizeMaster.DisplayName : null,
                ProductName = entity.ProductName,
                GradePrefix = entity.GradePrefix,
                GradeValue = entity.GradeValue,
                DisplayTitle = entity.DisplayTitle,
                IsSizeApplicable = entity.IsSizeApplicable,
                MetalClassificationIDs = entity.MetalClassifications.Select(x => x.MetalClassificationID).ToList(),
                MetalClassificationNames = entity.MetalClassifications.Select(x => x.MetalClassification != null ? x.MetalClassification.Name : "").Where(n => !string.IsNullOrEmpty(n)).ToList(),
                Versions = new List<ProductMasterVersionDetailsDto>()
            };

            foreach (var v in entity.Versions)
            {
                var vDto = new ProductMasterVersionDetailsDto
                {
                    ID = v.ID,
                    VersionNumber = v.VersionNumber,
                    Year = v.Year,
                    SpecificationFilePath = v.SpecificationFilePath,
                    StandardOrganizationID = v.StandardOrganizationID,
                    StandardOrganizationName = v.StandardOrganization != null ? v.StandardOrganization.Name : null,
                    SpecStdNo = v.SpecStdNo,
                    PartSection = v.PartSection,
                    Title = v.Title,
                    ProductCaption = v.ProductCaption,
                    IsActiveVersion = v.IsActiveVersion,
                    Grades = new List<ProductMasterVersionGradeDetailsDto>()
                };

                foreach (var vg in v.Grades.OrderBy(g => g.SortOrder))
                {
                    var gradeEntity = vg.SpecificationGrade ?? await _context.SpecificationGrades.FindAsync(vg.SpecificationGradeID);
                    var headerEntity = gradeEntity != null ? await _context.SpecificationHeaders.FindAsync(gradeEntity.SpecificationHeaderID) : null;

                    var activeSpecVer = headerEntity != null
                        ? await _context.SpecificationVersions.AsNoTracking().FirstOrDefaultAsync(sv => sv.SpecificationHeaderID == headerEntity.ID && sv.Status == Helpers.Enums.VersionStatus.Active)
                        : null;

                    int reqCount = 0;
                    if (activeSpecVer != null && gradeEntity != null)
                    {
                        reqCount = await _context.SpecificationLines.AsNoTracking()
                            .CountAsync(sl => sl.SpecificationVersionID == activeSpecVer.ID && sl.SpecificationGradeID == gradeEntity.ID);
                    }

                    var vgDto = new ProductMasterVersionGradeDetailsDto
                    {
                        ID = vg.ID,
                        SpecificationGradeID = vg.SpecificationGradeID,
                        GradeName = gradeEntity != null ? gradeEntity.Grade : $"Grade {vg.SpecificationGradeID}",
                        SpecificationHeaderID = gradeEntity != null ? gradeEntity.SpecificationHeaderID : 0,
                        SpecificationHeaderName = headerEntity != null ? (headerEntity.AliasName ?? headerEntity.SpecificationNo ?? "") : "",
                        SpecificationCode = headerEntity != null ? (headerEntity.SpecificationNo ?? headerEntity.AliasName ?? "") : "",
                        ActiveVersionID = activeSpecVer?.ID,
                        ActiveVersionName = activeSpecVer != null ? (activeSpecVer.Version ?? activeSpecVer.EffectiveDate?.ToString("yyyy") ?? "Active") : "No Active Version",
                        RequirementCount = reqCount,
                        SortOrder = vg.SortOrder,
                        Conditions = vg.Conditions.OrderBy(c => c.Priority).Select(c => new ProductMasterVersionGradeConditionDetailsDto
                        {
                            ID = c.ID,
                            ProductConditionID1 = c.ProductConditionID1,
                            ProductConditionName1 = c.ProductCondition1 != null ? c.ProductCondition1.Name : null,
                            ProductConditionID2 = c.ProductConditionID2,
                            ProductConditionName2 = c.ProductCondition2 != null ? c.ProductCondition2.Name : null,
                            HeatTreatmentID = c.HeatTreatmentID,
                            HeatTreatmentName = c.HeatTreatment != null ? c.HeatTreatment.Name : null,
                            ProductSizeMasterID = c.ProductSizeMasterID,
                            ProductSizeName = c.ProductSizeMaster != null ? c.ProductSizeMaster.DisplayName : null,
                            Priority = c.Priority
                        }).ToList()
                    };

                    vDto.Grades.Add(vgDto);
                }

                vDto.ApplicabilityCount = vDto.Grades.Count;
                dto.Versions.Add(vDto);
            }

            return dto;
        }

        public async Task<PagedResponse<object>> GetAllProductMasters(PageFilter filter)
        {
            return await _repository.GetAll(filter);
        }

        public async Task<List<DropdwonSelector>> GetProductMasterDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20, long metalId = 0)
        {
            return await _repository.GetDropdown(searchTerm, pageNo, pageSize, metalId);
        }

        public async Task<GradeApplicabilityInfoDto?> GetGradeApplicabilityInfo(long gradeId)
        {
            var gradeEntity = await _context.SpecificationGrades.FindAsync(gradeId);
            if (gradeEntity == null) return null;

            var headerEntity = await _context.SpecificationHeaders.FindAsync(gradeEntity.SpecificationHeaderID);
            var activeSpecVer = headerEntity != null
                ? await _context.SpecificationVersions.AsNoTracking().FirstOrDefaultAsync(sv => sv.SpecificationHeaderID == headerEntity.ID && sv.Status == Helpers.Enums.VersionStatus.Active)
                : null;

            int reqCount = 0;
            if (activeSpecVer != null)
            {
                reqCount = await _context.SpecificationLines.AsNoTracking()
                    .CountAsync(sl => sl.SpecificationVersionID == activeSpecVer.ID && sl.SpecificationGradeID == gradeEntity.ID);
            }

            return new GradeApplicabilityInfoDto
            {
                SpecificationGradeID = gradeEntity.ID,
                GradeName = gradeEntity.Grade,
                SpecificationHeaderID = gradeEntity.SpecificationHeaderID,
                SpecificationHeaderName = headerEntity != null ? (headerEntity.AliasName ?? headerEntity.SpecificationNo ?? "") : "",
                SpecificationCode = headerEntity != null ? (headerEntity.SpecificationNo ?? headerEntity.AliasName ?? "") : "",
                ActiveVersionID = activeSpecVer?.ID,
                ActiveVersionName = activeSpecVer != null ? (activeSpecVer.Version ?? activeSpecVer.EffectiveDate?.ToString("yyyy") ?? "Active") : "No Active Version",
                RequirementCount = reqCount
            };
        }

        public async Task<GradeParametersDto?> GetGradeParametersByGradeId(long gradeId)
        {
            var gradeEntity = await _context.SpecificationGrades.FindAsync(gradeId);
            if (gradeEntity == null) return null;

            var headerEntity = await _context.SpecificationHeaders.FindAsync(gradeEntity.SpecificationHeaderID);

            var specLines = await _context.SpecificationLines
                .AsSplitQuery()
                .Include(x => x.Parameter)
                .Include(x => x.ParameterUnit)
                .Include(x => x.LaboratoryTest)
                .Include(x => x.HeatTreatment)
                .Include(x => x.ProductSizeMaster)
                .Include(x => x.TestMethodMappings)
                    .ThenInclude(tm => tm.TestMethodSpecification)
                .Where(x => x.SpecificationGradeID == gradeId)
                .ToListAsync();

            var result = new GradeParametersDto
            {
                SpecificationGradeID = gradeId,
                GradeName = gradeEntity.Grade,
                SpecificationHeaderID = gradeEntity.SpecificationHeaderID,
                SpecificationHeaderName = headerEntity != null ? (headerEntity.AliasName ?? headerEntity.SpecificationNo ?? "") : "",
                ChemicalParameters = new List<SpecParameterLineDto>(),
                GeneralParameters = new List<SpecParameterLineDto>(),
                LaboratoryTests = new List<GradeLaboratoryTestDto>(),
                TestMethods = new List<GradeTestMethodDto>(),
                AvailablePC1 = new List<DropdwonSelector>(),
                AvailablePC2 = new List<DropdwonSelector>(),
                AvailableHeatTreatments = new List<DropdwonSelector>(),
                AvailableProductSizes = new List<DropdwonSelector>()
            };

            foreach (var line in specLines)
            {
                var paramDto = new SpecParameterLineDto
                {
                    SpecificationLineID = line.ID,
                    ParameterID = line.ParameterID,
                    ParameterName = line.Parameter != null ? line.Parameter.Name : "",
                    Type = (line.Type ?? "chemical").ToLower() == "chemical" ? "chemical" : "mechanical",
                    MinValue = line.MinValue,
                    MaxValue = line.MaxValue,
                    TextValue = line.TextValue,
                    ParameterUnitName = line.ParameterUnit != null ? line.ParameterUnit.Name : "",
                    LaboratoryTestID = line.LaboratoryTestID,
                    LaboratoryTestName = line.LaboratoryTest != null ? line.LaboratoryTest.Name : "",
                    ProductConditionID1 = line.ProductConditionID1,
                    ProductConditionID2 = line.ProductConditionID2,
                    HeatTreatmentID = line.HeatTreatmentID,
                    ProductSizeMasterID = line.ProductSizeMasterID,
                    TestMethods = line.TestMethodMappings.Select(tm => new SpecTestMethodDto
                    {
                        TestMethodSpecificationID = tm.TestMethodSpecificationID,
                        TestMethodName = tm.TestMethodSpecification != null ? (tm.TestMethodSpecification.DisplayTitle ?? tm.TestMethodSpecification.Name) : "",
                        NumberOfTestSpecimen = tm.NumberOfTestSpecimen
                    }).ToList()
                };

                if (paramDto.Type == "chemical")
                {
                    result.ChemicalParameters.Add(paramDto);
                }
                else
                {
                    result.GeneralParameters.Add(paramDto);
                }
            }

            // Extract distinct Laboratory Tests across grade lines
            var labTestGroups = specLines
                .Where(x => x.LaboratoryTestID.HasValue && x.LaboratoryTest != null)
                .GroupBy(x => x.LaboratoryTestID!.Value);

            foreach (var g in labTestGroups)
            {
                var first = g.First();
                result.LaboratoryTests.Add(new GradeLaboratoryTestDto
                {
                    LaboratoryTestID = g.Key,
                    LaboratoryTestName = first.LaboratoryTest!.Name,
                    ParameterCount = g.Count()
                });
            }

            // Extract distinct Test Methods across grade lines
            var testMethodMappings = specLines
                .SelectMany(x => x.TestMethodMappings)
                .Where(tm => tm.TestMethodSpecificationID.HasValue && tm.TestMethodSpecification != null)
                .GroupBy(tm => tm.TestMethodSpecificationID!.Value);

            foreach (var g in testMethodMappings)
            {
                var first = g.First();
                result.TestMethods.Add(new GradeTestMethodDto
                {
                    TestMethodSpecificationID = g.Key,
                    TestMethodName = first.TestMethodSpecification != null ? (first.TestMethodSpecification.DisplayTitle ?? first.TestMethodSpecification.Name) : "",
                    NumberOfTestSpecimen = first.NumberOfTestSpecimen
                });
            }

            // Extract Available Condition Options (only distinct non-null values present in grade's spec lines)
            var pc1Ids = specLines.Where(x => x.ProductConditionID1.HasValue).Select(x => x.ProductConditionID1!.Value).Distinct().ToList();
            if (pc1Ids.Any())
            {
                var pc1Entities = await _context.ProductConditionMasters.Where(x => pc1Ids.Contains(x.ID)).ToListAsync();
                result.AvailablePC1 = pc1Entities.Select(x => new DropdwonSelector { Id = x.ID, Name = x.Name }).ToList();
            }

            var pc2Ids = specLines.Where(x => x.ProductConditionID2.HasValue).Select(x => x.ProductConditionID2!.Value).Distinct().ToList();
            if (pc2Ids.Any())
            {
                var pc2Entities = await _context.ProductConditionMasters.Where(x => pc2Ids.Contains(x.ID)).ToListAsync();
                result.AvailablePC2 = pc2Entities.Select(x => new DropdwonSelector { Id = x.ID, Name = x.Name }).ToList();
            }

            var htIds = specLines.Where(x => x.HeatTreatmentID.HasValue).Select(x => x.HeatTreatmentID!.Value).Distinct().ToList();
            if (htIds.Any())
            {
                var htEntities = await _context.HeatTreatmentMasters.Where(x => htIds.Contains(x.ID)).ToListAsync();
                result.AvailableHeatTreatments = htEntities.Select(x => new DropdwonSelector { Id = x.ID, Name = x.Name }).ToList();
            }

            var sizeIds = specLines.Where(x => x.ProductSizeMasterID.HasValue).Select(x => x.ProductSizeMasterID!.Value).Distinct().ToList();
            if (sizeIds.Any())
            {
                var sizeEntities = await _context.ProductSizeMasters.Where(x => sizeIds.Contains(x.ID)).ToListAsync();
                result.AvailableProductSizes = sizeEntities.Select(x => new DropdwonSelector { Id = x.ID, Name = x.DisplayName }).ToList();
            }

            return result;
        }

        public async Task<List<string>> GetPrefixOptions()
        {
            var list = new List<string>();

            // 1. Fetch from standard dropdown Configuration (KeyName = "ProductPrefix", GroupName = "dropdown")
            var dropdownConfig = await _context.Configurations
                .FirstOrDefaultAsync(c => c.KeyName == "ProductPrefix" && c.IsActive);

            if (dropdownConfig != null && !string.IsNullOrWhiteSpace(dropdownConfig.Value))
            {
                var items = dropdownConfig.Value.Split('|', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s));
                
                foreach (var item in items)
                {
                    if (!list.Contains(item, StringComparer.OrdinalIgnoreCase))
                    {
                        list.Add(item);
                    }
                }
            }

            // 2. Also incorporate individual entries with GroupName = "ProductPrefix" if any exist
            var groupConfigs = await _context.Configurations
                .Where(c => c.GroupName == "ProductPrefix" && c.IsActive)
                .Select(c => c.Value)
                .Distinct()
                .ToListAsync();

            foreach (var gc in groupConfigs)
            {
                if (!string.IsNullOrWhiteSpace(gc) && !list.Contains(gc.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    list.Add(gc.Trim());
                }
            }

            // 3. Defaults fallback
            if (!list.Any())
            {
                list = new List<string> { "Grade", "Class", "Designation", "Type", "Series" };
            }

            return list;
        }

        public async Task<bool> AddPrefixOption(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix)) return false;

            var trimmed = prefix.Trim();

            // Find existing dropdown configuration
            var dropdownConfig = await _context.Configurations
                .FirstOrDefaultAsync(c => c.KeyName == "ProductPrefix" && c.IsActive);

            if (dropdownConfig != null)
            {
                var currentItems = (dropdownConfig.Value ?? "")
                    .Split('|', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                if (!currentItems.Contains(trimmed, StringComparer.OrdinalIgnoreCase))
                {
                    currentItems.Add(trimmed);
                    dropdownConfig.Value = string.Join("|", currentItems);
                    dropdownConfig.ModifiedBy = _loggedInUser?.EmployeeID ?? 0;
                    dropdownConfig.ModifiedOn = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }

            // Check if individual record exists
            var existsInGroup = await _context.Configurations
                .AnyAsync(c => c.GroupName == "ProductPrefix" && c.Value.ToLower() == trimmed.ToLower() && c.IsActive);

            if (!existsInGroup)
            {
                var defaultItems = new List<string> { "Grade", "Class", "Designation", "Type", "Series" };
                if (!defaultItems.Contains(trimmed, StringComparer.OrdinalIgnoreCase))
                {
                    defaultItems.Add(trimmed);
                }

                _context.Configurations.Add(new Configuration
                {
                    KeyName = "ProductPrefix",
                    GroupName = "dropdown",
                    Value = string.Join("|", defaultItems),
                    ValueType = "string",
                    Description = "Product Master Grade Prefix Options",
                    CreatedBy = _loggedInUser?.EmployeeID ?? 0,
                    CreatedOn = DateTime.UtcNow,
                    CompanyCode = _loggedInUser?.CompanyCode ?? "LIMS",
                    IsActive = true
                });
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ToggleProductMasterStatus(long id)
        {
            var existing = await _repository.GetById(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Product / Material Master with ID {id} not found.");
            }

            if (!existing.IsActive)
            {
                if (await _repository.ExistsByNameAndNotId(existing.ProductName, existing.ID))
                {
                    throw new InvalidOperationException($"Cannot reactivate: An active Product / Material Master with name '{existing.ProductName}' already exists.");
                }
            }

            existing.IsActive = !existing.IsActive;
            existing.ModifiedBy = _loggedInUser?.EmployeeID ?? 0;
            existing.ModifiedOn = DateTime.UtcNow;

            await _repository.Update(existing);
            _logger.LogInformation("Product Master '{Name}' (ID {ID}) status toggled to {Status}.", existing.ProductName, existing.ID, existing.IsActive ? "Active" : "Inactive");
            return existing.IsActive;
        }
    }
}
