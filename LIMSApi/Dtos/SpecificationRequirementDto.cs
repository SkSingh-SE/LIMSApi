using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LIMSApi.Helpers.Enums;

namespace LIMSApi.Dtos
{
    public class SpecificationRequirementContextDto
    {
        public long SpecificationHeaderID { get; set; }
        public string SpecificationCode { get; set; } = string.Empty;
        public string SpecificationName { get; set; } = string.Empty;
        public string? StandardReference { get; set; }

        public long SpecificationVersionID { get; set; }
        public string VersionNumber { get; set; } = string.Empty;
        public string? Year { get; set; }
        public VersionStatus VersionStatus { get; set; }
        public bool IsDefaultVersion { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool IsEditable => VersionStatus == VersionStatus.Draft;

        public long SpecificationGradeID { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public string? MetalClassificationName { get; set; }

        public int TotalRequirementsCount { get; set; }
    }

    public class SpecificationRequirementItemDto
    {
        public long ID { get; set; }
        public long SpecificationVersionID { get; set; }
        public long SpecificationGradeID { get; set; }

        // Section 1: Definition
        public long ParameterID { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public string? ParameterCode { get; set; }
        public string? ParameterSymbol { get; set; }
        public long? ParameterUnitID { get; set; }
        public string? ParameterUnitName { get; set; }
        public string? ParameterUnitSymbol { get; set; }
        public string InputType { get; set; } = string.Empty;
        public string LegacyType { get; set; } = "universal";

        // Section 2: Limits & Tolerances
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string? LowerLimitValue { get; set; }
        public string? UpperLimitValue { get; set; }
        public decimal? LowerLimitDecimalValue { get; set; }
        public decimal? UpperLimitDecimalValue { get; set; }
        public decimal? MinTolerance { get; set; }
        public decimal? MaxTolerance { get; set; }
        public string FormattedLimits { get; set; } = string.Empty;

        // Section 3: Equations
        public string? Equation { get; set; }
        public string? MinEquation { get; set; }
        public string? MaxEquation { get; set; }
        public string? ParameterFormula { get; set; }
        public string? ParameterFormulaDisplay { get; set; }
        public bool ParameterIsCalculated { get; set; }
        public bool HasEquations => !string.IsNullOrWhiteSpace(Equation) || !string.IsNullOrWhiteSpace(MinEquation) || !string.IsNullOrWhiteSpace(MaxEquation);

        // Section 4: Applicability & Conditions
        public long? ProductSizeMasterID { get; set; }
        public string? ProductSizeName { get; set; }
        public long? HeatTreatmentID { get; set; }
        public string? HeatTreatmentName { get; set; }
        public long? SpecimenOrientationID { get; set; }
        public string? SpecimenOrientationName { get; set; }
        public long? DimensionalFactorID { get; set; }
        public long? ProductConditionID1 { get; set; }
        public long? ProductConditionID2 { get; set; }
        public List<RequirementDimensionConditionDto> Conditions { get; set; } = new();
        public string ConditionsSummary { get; set; } = string.Empty;

        // Section 5: Test Method Applicability
        public long? LaboratoryTestID { get; set; }
        public string? LaboratoryTestName { get; set; }
        public List<RequirementTestMethodMappingDto> TestMethods { get; set; } = new();
        public string TestMethodsSummary { get; set; } = string.Empty;

        // Section 6: Instructions & Reporting
        public string? TestCondition { get; set; }
        public string? TestNote { get; set; }

        // Section 7: Status & Lock
        public bool IsLocked { get; set; }
    }

    public class SaveSpecificationRequirementDto
    {
        public long ID { get; set; }

        [Required]
        public long SpecificationVersionID { get; set; }

        [Required]
        public long SpecificationGradeID { get; set; }

        // Section 1: Definition
        [Required]
        public long ParameterID { get; set; }
        public long? ParameterUnitID { get; set; }
        public long? ParameterUnitEquivalentID { get; set; }
        public string InputType { get; set; } = "Decimal";
        public string? TextValue { get; set; }
        public string LegacyType { get; set; } = "chemical";

        // Section 2: Limits & Tolerances
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string? LowerLimitValue { get; set; }
        public string? UpperLimitValue { get; set; }
        public decimal? LowerLimitDecimalValue { get; set; }
        public decimal? UpperLimitDecimalValue { get; set; }
        public decimal? MinTolerance { get; set; }
        public decimal? MaxTolerance { get; set; }

        // Section 3: Equations
        public string? Equation { get; set; }
        public string? MinEquation { get; set; }
        public string? MaxEquation { get; set; }

        // Section 4: Applicability & Conditions
        public long? ProductSizeMasterID { get; set; }
        public long? HeatTreatmentID { get; set; }
        public long? SpecimenOrientationID { get; set; }
        public long? DimensionalFactorID { get; set; }
        public long? ProductConditionID1 { get; set; }
        public long? ProductConditionID2 { get; set; }
        public List<RequirementDimensionConditionDto> Conditions { get; set; } = new();

        // Section 5: Test Method Applicability
        public long? LaboratoryTestID { get; set; }
        public List<RequirementTestMethodMappingDto> TestMethods { get; set; } = new();

        // Section 6: Instructions & Reporting
        public string? TestCondition { get; set; }
        public string? TestNote { get; set; }
    }

    public class RequirementDimensionConditionDto
    {
        public long ID { get; set; }
        public long ConditionMasterID { get; set; }
        public long TestConditionDimensionID { get => ConditionMasterID; set => ConditionMasterID = value; }
        public string? ConditionCode { get; set; }
        public string? ConditionName { get; set; }
        public string? DimensionName { get => ConditionName; set => ConditionName = value; }
        public string? Unit { get; set; }
        public string? ValueType { get; set; }
        public string Operator { get; set; } = "=";
        public string Value1 { get; set; } = string.Empty;
        public string? Value2 { get; set; }
    }

    public class RequirementTestMethodMappingDto
    {
        public long ID { get; set; }
        public long? LaboratoryTestID { get; set; }
        public string? LaboratoryTestName { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodSpecificationName { get; set; }
        public int? NumberOfTestSpecimen { get; set; }
        public int? DisplayOrder { get; set; }
    }

    public class CopyVersionRequirementsDto
    {
        [Required]
        public long SourceVersionID { get; set; }

        [Required]
        public long TargetVersionID { get; set; }

        public long? SpecificationGradeID { get; set; }
    }
}
