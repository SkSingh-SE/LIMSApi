using System;
using System.Collections.Generic;

namespace LIMSApi.Dtos
{
    public class ProductMasterCreateDto
    {
        public long? ProductSizeMasterID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? GradePrefix { get; set; }
        public string? GradeValue { get; set; }
        public string? DisplayTitle { get; set; }
        public bool IsSizeApplicable { get; set; } = true;
        public List<long> MetalClassificationIDs { get; set; } = new List<long>();
        public List<ProductMasterVersionCreateDto> Versions { get; set; } = new List<ProductMasterVersionCreateDto>();
    }

    public class ProductMasterUpdateDto : ProductMasterCreateDto
    {
        public long ID { get; set; }
    }

    public class ProductMasterVersionCreateDto
    {
        public string VersionNumber { get; set; } = "1";
        public string? Year { get; set; }
        public string? SpecificationFilePath { get; set; }
        public long? StandardOrganizationID { get; set; }
        public string? SpecStdNo { get; set; }
        public string? PartSection { get; set; }
        public string? Title { get; set; }
        public string? ProductCaption { get; set; }
        public bool IsActiveVersion { get; set; } = true;
        public List<ProductMasterVersionGradeDto> Grades { get; set; } = new List<ProductMasterVersionGradeDto>();
    }

    public class ProductMasterVersionGradeDto
    {
        public long SpecificationGradeID { get; set; }
        public int SortOrder { get; set; } = 1;
        public List<ProductMasterVersionGradeConditionDto> Conditions { get; set; } = new List<ProductMasterVersionGradeConditionDto>();
    }

    public class ProductMasterVersionGradeConditionDto
    {
        public long? ProductConditionID1 { get; set; }
        public long? ProductConditionID2 { get; set; }
        public long? HeatTreatmentID { get; set; }
        public long? ProductSizeMasterID { get; set; }
        public int Priority { get; set; } = 1;
    }

    public class ProductMasterDetailsDto
    {
        public long ID { get; set; }
        public long? ProductSizeMasterID { get; set; }
        public string? ProductSizeName { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? GradePrefix { get; set; }
        public string? GradeValue { get; set; }
        public string? DisplayTitle { get; set; }
        public bool IsSizeApplicable { get; set; }
        public List<long> MetalClassificationIDs { get; set; } = new List<long>();
        public List<string> MetalClassificationNames { get; set; } = new List<string>();
        public List<ProductMasterVersionDetailsDto> Versions { get; set; } = new List<ProductMasterVersionDetailsDto>();
    }

    public class ProductMasterVersionDetailsDto
    {
        public long ID { get; set; }
        public string VersionNumber { get; set; } = "1";        public string? Year { get; set; }
        public string? SpecificationFilePath { get; set; }
        public long? StandardOrganizationID { get; set; }
        public string? StandardOrganizationName { get; set; }
        public string? SpecStdNo { get; set; }
        public string? PartSection { get; set; }
        public string? Title { get; set; }
        public string? ProductCaption { get; set; }
        public bool IsActiveVersion { get; set; }
        public List<ProductMasterVersionGradeDetailsDto> Grades { get; set; } = new List<ProductMasterVersionGradeDetailsDto>();
    }

    public class ProductMasterVersionGradeDetailsDto
    {
        public long ID { get; set; }
        public long SpecificationGradeID { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public long SpecificationHeaderID { get; set; }
        public string SpecificationHeaderName { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public List<ProductMasterVersionGradeConditionDetailsDto> Conditions { get; set; } = new List<ProductMasterVersionGradeConditionDetailsDto>();
        public GradeParametersDto Parameters { get; set; } = new GradeParametersDto();
    }

    public class ProductMasterVersionGradeConditionDetailsDto
    {
        public long ID { get; set; }
        public long? ProductConditionID1 { get; set; }
        public string? ProductConditionName1 { get; set; }
        public long? ProductConditionID2 { get; set; }
        public string? ProductConditionName2 { get; set; }
        public long? HeatTreatmentID { get; set; }
        public string? HeatTreatmentName { get; set; }
        public long? ProductSizeMasterID { get; set; }
        public string? ProductSizeName { get; set; }
        public int Priority { get; set; }
    }

    public class GradeParametersDto
    {
        public long SpecificationGradeID { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public long SpecificationHeaderID { get; set; }
        public string SpecificationHeaderName { get; set; } = string.Empty;

        // 4 Category Tab Data
        public List<SpecParameterLineDto> ChemicalParameters { get; set; } = new List<SpecParameterLineDto>();
        public List<SpecParameterLineDto> GeneralParameters { get; set; } = new List<SpecParameterLineDto>();
        public List<GradeLaboratoryTestDto> LaboratoryTests { get; set; } = new List<GradeLaboratoryTestDto>();
        public List<GradeTestMethodDto> TestMethods { get; set; } = new List<GradeTestMethodDto>();

        // Available condition options (extracted only from grade's spec lines)
        public List<DropdwonSelector> AvailablePC1 { get; set; } = new List<DropdwonSelector>();
        public List<DropdwonSelector> AvailablePC2 { get; set; } = new List<DropdwonSelector>();
        public List<DropdwonSelector> AvailableHeatTreatments { get; set; } = new List<DropdwonSelector>();
        public List<DropdwonSelector> AvailableProductSizes { get; set; } = new List<DropdwonSelector>();
    }

    public class GradeLaboratoryTestDto
    {
        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestName { get; set; } = string.Empty;
        public int ParameterCount { get; set; }
    }

    public class GradeTestMethodDto
    {
        public long TestMethodSpecificationID { get; set; }
        public string TestMethodName { get; set; } = string.Empty;
        public int? NumberOfTestSpecimen { get; set; }
    }

    public class SpecParameterLineDto
    {
        public long SpecificationLineID { get; set; }
        public long? ParameterID { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public string Type { get; set; } = "chemical";
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string? TextValue { get; set; }
        public string? ParameterUnitName { get; set; }
        public long? LaboratoryTestID { get; set; }
        public string? LaboratoryTestName { get; set; }
        public long? ProductConditionID1 { get; set; }
        public long? ProductConditionID2 { get; set; }
        public long? HeatTreatmentID { get; set; }
        public long? ProductSizeMasterID { get; set; }
        public List<SpecTestMethodDto> TestMethods { get; set; } = new List<SpecTestMethodDto>();
    }

    public class SpecTestMethodDto
    {
        public long? TestMethodSpecificationID { get; set; }
        public string TestMethodName { get; set; } = string.Empty;
        public int? NumberOfTestSpecimen { get; set; }
    }

    public class ProductMasterListDto
    {
        public long ID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? GradePrefix { get; set; }
        public string? DisplayTitle { get; set; }
        public bool IsSizeApplicable { get; set; }
        public string? ProductSizeName { get; set; }
        public string ActiveVersionNo { get; set; } = string.Empty;
        public string LinkedSpecsSummary { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
