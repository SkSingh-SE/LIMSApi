using System;
using System.Collections.Generic;

namespace LIMSApi.Dtos
{
    public class TestExecutionConfigSnapshotDto
    {
        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestName { get; set; } = string.Empty;
        public string? LaboratoryTestCode { get; set; }
        public string? DisciplineName { get; set; }

        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodName { get; set; }
        public string? TestMethodStandard { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public string? TestMethodVersion { get; set; }

        public long? SpecificationHeaderID { get; set; }
        public string? SpecificationCode { get; set; }
        public string? SpecificationTitle { get; set; }
        public long? SpecificationVersionID { get; set; }
        public string? SpecificationVersion { get; set; }
        public long? SpecificationGradeID { get; set; }
        public long? GradeID { get => SpecificationGradeID; set => SpecificationGradeID = value; }
        public string? GradeName { get; set; }

        public DateTime SnapshotDateUtc { get; set; } = DateTime.UtcNow;
        public bool IsFrozen { get; set; } = false;

        // Execution Layout / Data Capture Pattern
        public long? ExecutionLayoutID { get; set; }
        public string? ExecutionLayoutCode { get; set; }
        public string? ExecutionLayoutName { get; set; }
        public string RendererType { get; set; } = "ObservationMatrix"; // "ObservationMatrix", "MultiSpecimen", "MultiReading", "ParameterTable", "Qualitative", "Calculation", "Graph"
        public string SpecimenMode { get; set; } = "Single"; // "None", "Single", "Multiple", "ConfiguredCount"
        public string ObservationMode { get; set; } = "Multiple"; // "Single", "Multiple", "ConfiguredCount"
        public int? ConfiguredSpecimenCount { get; set; }
        public int? ConfiguredReadingCount { get; set; }
        public string? DefaultAggregateType { get; set; } = "Average"; // "None", "Average", "Mean", "Min", "Max", "Sum", "Count", "StDev"
        public string? AggregateScope { get; set; } = "Observation"; // "Observation", "Specimen", "Execution"
        public string LayoutResolutionLevel { get; set; } = "Unassigned";
        public ExecutionLayoutDto? ExecutionLayout { get; set; }
        public List<long> UnmappedParameterIDs { get; set; } = new();

        // 1. Parameters
        public List<SnapshotParameterDto> Parameters { get; set; } = new();

        // 2. Conditions / Environment
        public List<SnapshotConditionDto> Conditions { get; set; } = new();

        // 3. Equipment / Instruments
        public List<SnapshotEquipmentDto> Equipment { get; set; } = new();

        // 4. Factors & Conversions
        public List<SnapshotFactorDto> Factors { get; set; } = new();

        // 5. Measurement Uncertainty (MU)
        public SnapshotMeasurementUncertaintyDto? MeasurementUncertainty { get; set; }

        // 6. Acceptance Criteria
        public SnapshotAcceptanceCriteriaDto? AcceptanceCriteria { get; set; }

        // 6b. Lab Scope freeze reference (Phase 1C: which scope applied at planning time;
        // execution-time re-verification and verdict freeze belong to Phase 5/6).
        public SnapshotScopeDto? Scope { get; set; }
        
        // 6c. Accreditation freeze reference (Phase 1C: which accreditation certificate covered the execution).
        public SnapshotAccreditationDto? Accreditation { get; set; }

        // 7. Attachments
        public List<SnapshotAttachmentDto> Attachments { get; set; } = new();

        // 8. Remarks & Notes
        public string? GeneralRemarks { get; set; }

        // 9. Provenance Metadata (Phase 6 Lineage Anchor)
        public SnapshotProvenanceDto? Provenance { get; set; }
    }

    public class SnapshotParameterDto
    {
        public long ParameterMasterID { get; set; }
        public string Code { get; set; } = string.Empty; // Semantic code e.g. "SOIL_LL", "SOIL_PL", "SOIL_PI"
        public string Name { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public string InputType { get; set; } = "Decimal"; // Decimal, Integer, Text, Dropdown, Boolean, Formula, Date, DateTime
        public string ParameterType { get; set; } = "Input"; // "Input", "Calculated", "Derived"
        public string? CalculationRole { get; set; } // e.g. "CurvePeak"
        public int DecimalPrecision { get; set; } = 2;
        public bool IsCalculated { get; set; }
        public string? Formula { get; set; } // e.g. "{SOIL_LL} - {SOIL_PL}"
        public List<string> FormulaDependencies { get; set; } = new(); // ["SOIL_LL", "SOIL_PL"]
        public decimal? SpecMin { get; set; }
        public decimal? SpecMax { get; set; }
        public decimal? SpecTarget { get; set; }
        public decimal? MinTolerance { get; set; }
        public decimal? MaxTolerance { get; set; }
        public string? ComparisonCriteria { get; set; } = "Range"; // "Range", ">=", "<=", "=", "Between", "TargetTolerance"
        public string? AcceptanceCriteria { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; } = true;
        public bool IsReportable { get; set; } = true;
        public string? AggregateType { get; set; } = "Average"; // "Average", "Min", "Max", "Sum", "None"
        public List<SnapshotDropdownOptionDto> DropdownOptions { get; set; } = new();
        public string? Symbol { get; set; }

        // Configuration-Driven Effective Tolerance
        public decimal? EffectiveMin { get; set; }
        public decimal? EffectiveMax { get; set; }
        public string? ToleranceSource { get; set; } // "SPECIFICATION_LINE", "TOLERANCE_MASTER", "NONE"
        public string? ToleranceType { get; set; }   // "Absolute", "Percentage"
        public long? ToleranceMasterID { get; set; }
        public decimal? AppliedTolerance { get; set; }

        // Parameter-Level Measurement Uncertainty (ISO 17025)
        public decimal? ParameterCombinedUncertainty { get; set; }
        public decimal? ParameterExpandedUncertainty { get; set; }
        public decimal? ParameterCoverageFactor { get; set; }
        public long? MeasurementUncertaintyMasterID { get; set; }
        public string? MUSource { get; set; }        // "PARAMETER_MASTER", "NONE"
        
        // Parameter-Level NABL Scope (Phase 1C)
        public decimal? NablScopeLowerLimit { get; set; }
        public decimal? NablScopeUpperLimit { get; set; }
        public bool? IsUnderISO { get; set; }
        public long? LabScopeSpecParamId { get; set; }
    }

    public class SnapshotDropdownOptionDto
    {
        public long ID { get; set; }
        public string DisplayText { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class SnapshotConditionDto
    {
        public long? ConditionMasterID { get; set; }
        public long? ConditionDimensionID { get => ConditionMasterID; set => ConditionMasterID = value; }
        public string DimensionName { get; set; } = string.Empty; // e.g. "Temperature", "Voltage", "Age"
        public string? DimensionCode { get; set; }
        public string? Category { get; set; }
        public string? Unit { get; set; } // e.g. "°C", "V DC", "Days"
        public string? ConfiguredOperator { get; set; } = "="; // "=", ">=", "<=", "Between"
        public string? Operator { get => ConfiguredOperator; set => ConfiguredOperator = value; }
        public string? ConfiguredValue1 { get; set; }
        public string? ConfiguredValue2 { get; set; }
        public string? ConfiguredValue { get => ConfiguredValue1; set => ConfiguredValue1 = value; }
        public string? ActualExecutionValue { get; set; } // Actual execution reading e.g. "27"
        public string? SelectedExecutionValue { get => ActualExecutionValue; set => ActualExecutionValue = value; }
        public string? RequirementContext { get; set; }
        public bool IsMandatory { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
    }

    public class SnapshotEquipmentDto
    {
        public long? EquipmentID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Model { get; set; }
        public string? EquipmentType { get; set; }
        public string? CalibrationNo { get; set; }
        public DateTime? CalibratedOn { get; set; }
        public DateTime? ValidUpto { get; set; }
        public DateTime? CalibrationValidUpto { get => ValidUpto; set => ValidUpto = value; }
        public string? CalibrationStatus { get; set; }

        // Phase 1D: frozen planned-requirement reference (null = no configured requirement; dept fallback).
        // Always read these frozen values for historical executions — never re-resolve the live master.
        public long? EquipmentRequirementID { get; set; }
        public string? RequirementCode { get; set; }
        public bool IsMandatory { get; set; } = true;
    }

    public class SnapshotFactorDto
    {
        public string FactorType { get; set; } = "Multiplication Factor"; // "Multiplication Factor", "Division Factor", "Offset", "Dilution Factor"
        public string FactorName { get; set; } = string.Empty;
        public string? Name { get => FactorName; set => FactorName = value; }
        public decimal Value { get; set; } = 1.0m;
        public decimal FactorValue { get => Value; set => Value = value; }
        public string? AppliedOn { get; set; } = "All Results";
        public string? Description { get; set; }

        // Phase 1E: frozen master reference for historical reproducibility.
        // Populated from FactorConversionMaster at execution start; unit conversions and
        // formulas are owned elsewhere and never frozen here.
        // Always read these frozen values for historical executions — never re-resolve the live master.
        public long? FactorConversionID { get; set; }
        public string? FactorCode { get; set; }
        public string? Code { get => FactorCode; set => FactorCode = value; }
        public long? InputParameterID { get; set; }
        public long? OutputParameterID { get; set; }
        public bool IsMandatory { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
    }

    public class SnapshotMeasurementUncertaintyDto
    {
        public string UncertaintyType { get; set; } = "Expanded Uncertainty (k=2)";
        public decimal? Value { get; set; }
        public string? Unit { get; set; } = "%";
        public decimal CoverageFactor { get; set; } = 2.0m;
        public decimal? ConfidenceLevel { get; set; }
        public decimal? CombinedUncertainty { get; set; }
        public decimal? ExpandedUncertainty { get; set; }
        public string Basis { get; set; } = "Type B"; // "Type A", "Type B"
        public string? Remarks { get; set; } = "As per ISO 17025";

        // Phase 1F: frozen master reference for historical reproducibility.
        // Populated from MeasurementUncertaintyMaster when a test/method-level configuration
        // matches; null when the legacy NABL string-match fallback (or nothing) applied.
        // Always read these frozen values for historical executions — never re-resolve the live master.
        public long? MeasurementUncertaintyMasterID { get; set; }
        public string? MasterCode { get; set; }
        public string? MasterName { get; set; }
    }

    public class SnapshotAcceptanceCriteriaDto
    {
        public string DecisionRule { get; set; } = "All Parameters Must Pass"; // "All Parameters Must Pass", "Configured Rule"
        public string OverallDecision { get; set; } = "Pass if all parameters within spec range";
        public string RoundingRule { get; set; } = "Round to nearest";
        public decimal RoundingPrecision { get; set; } = 0.01m;
        public string? PassFailThreshold { get; set; }

        // Phase 1B: frozen master reference for historical reproducibility.
        // Populated from AcceptanceCriteriaMaster once Phase 2 links test configuration to the master.
        // Always read these frozen values for historical executions — never re-resolve the live master.
        public long? AcceptanceCriteriaID { get; set; }
        public string? AcceptanceCriteriaCode { get; set; }
        public string? AcceptanceCriteriaName { get; set; }
    }

    public class SnapshotScopeDto
    {
        // LabScopeMaster ID whose validity/branch window covered planning time (null = no effective scope).
        public long? LabScopeID { get; set; }
        // Planning-time reference only: "Referenced" | "NoEffectiveScope".
        // Execution verdicts (WithinScope/OutsideScope/NotAccredited) are stamped at execution (Phase 5/6).
        public string ScopeStatus { get; set; } = "NoEffectiveScope";
        public DateTime CheckedOnUtc { get; set; } = DateTime.UtcNow;
        public string? Note { get; set; }
    }

    public class SnapshotAccreditationDto
    {
        public long? AccreditationID { get; set; }
        public string? CertificateNumber { get; set; }
        public string? AccreditationStatus { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public long? BranchID { get; set; }
        public string? LogoPath { get; set; }
    }

    public class SnapshotAttachmentDto
    {
        public long? AttachmentID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? FileType { get; set; } = "PDF";
        public long? FileSizeBytes { get; set; }
        public string? FileUrl { get; set; }
        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
        public string? UploadedByName { get; set; }
    }

    public class SnapshotProvenanceDto
    {
        public string SourceType { get; set; } = "Phase4EffectiveConfiguration"; // "Phase5ApprovedAdjustment", "Phase4EffectiveConfiguration"
        public long? SourceAdjustmentID { get; set; }
        public long? SourceReferenceID { get => SourceAdjustmentID; set => SourceAdjustmentID = value; }
        public int? SourceAdjustmentNumber { get; set; }
        public int? SourceRevision { get => SourceAdjustmentNumber ?? 1; set => SourceAdjustmentNumber = value; }
        public DateTime? SourceApprovedAtUtc { get; set; }
        public long? SourceApprovedBy { get; set; }
        public string? PlanBaselineHash { get; set; }
        public DateTime SnapshotGeneratedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime FrozenAtUtc { get => SnapshotGeneratedAtUtc; set => SnapshotGeneratedAtUtc = value; }
        public long? GeneratedByUserID { get; set; }
        public long? ResolvedByUserID { get => GeneratedByUserID; set => GeneratedByUserID = value; }
    }
}
