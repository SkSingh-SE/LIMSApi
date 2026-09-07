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
        public string? TestMethodVersion { get; set; }

        public long? SpecificationHeaderID { get; set; }
        public string? SpecificationTitle { get; set; }
        public long? SpecificationGradeID { get; set; }
        public string? GradeName { get; set; }

        public DateTime SnapshotDateUtc { get; set; } = DateTime.UtcNow;
        public bool IsFrozen { get; set; } = false;

        // Execution Layout / Data Capture Pattern
        public string RendererType { get; set; } = "ObservationMatrix"; // "ObservationMatrix", "MultiSpecimen", "MultiReading", "ParameterTable", "Qualitative", "Calculation", "Graph"
        public string SpecimenMode { get; set; } = "Single"; // "None", "Single", "Multiple", "ConfiguredCount"
        public string ObservationMode { get; set; } = "Multiple"; // "Single", "Multiple", "ConfiguredCount"
        public int? ConfiguredSpecimenCount { get; set; }
        public int? ConfiguredReadingCount { get; set; }
        public string? DefaultAggregateType { get; set; } = "Average"; // "None", "Average", "Mean", "Min", "Max", "Sum", "Count", "StDev"
        public string? AggregateScope { get; set; } = "Observation"; // "Observation", "Specimen", "Execution"

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

        // 7. Attachments
        public List<SnapshotAttachmentDto> Attachments { get; set; } = new();

        // 8. Remarks & Notes
        public string? GeneralRemarks { get; set; }
    }

    public class SnapshotParameterDto
    {
        public long ParameterMasterID { get; set; }
        public string Code { get; set; } = string.Empty; // Semantic code e.g. "SOIL_LL", "SOIL_PL", "SOIL_PI"
        public string Name { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public string InputType { get; set; } = "Decimal"; // Decimal, Integer, Text, Dropdown, Boolean, Formula
        public string ParameterType { get; set; } = "Input"; // "Input", "Calculated", "Derived"
        public int DecimalPrecision { get; set; } = 2;
        public bool IsCalculated { get; set; }
        public string? Formula { get; set; } // e.g. "{SOIL_LL} - {SOIL_PL}"
        public List<string> FormulaDependencies { get; set; } = new(); // ["SOIL_LL", "SOIL_PL"]
        public decimal? SpecMin { get; set; }
        public decimal? SpecMax { get; set; }
        public decimal? SpecTarget { get; set; }
        public string? ComparisonCriteria { get; set; } = "Range"; // "Range", ">=", "<=", "=", "Between", "TargetTolerance"
        public string? AcceptanceCriteria { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; } = true;
        public bool IsReportable { get; set; } = true;
        public string? AggregateType { get; set; } = "Average"; // "Average", "Min", "Max", "Sum", "None"
    }

    public class SnapshotConditionDto
    {
        public long? ConditionMasterID { get; set; }
        public long? ConditionDimensionID { get => ConditionMasterID; set => ConditionMasterID = value; }
        public string DimensionName { get; set; } = string.Empty; // e.g. "Temperature", "Voltage", "Age"
        public string? Unit { get; set; } // e.g. "°C", "V DC", "Days"
        public string? ConfiguredOperator { get; set; } = "="; // "=", ">=", "<=", "Between"
        public string? ConfiguredValue1 { get; set; }
        public string? ConfiguredValue2 { get; set; }
        public string? ActualExecutionValue { get; set; } // Actual execution reading e.g. "27"
        public string? SelectedExecutionValue { get => ActualExecutionValue; set => ActualExecutionValue = value; }
        public bool IsMandatory { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
    }

    public class SnapshotEquipmentDto
    {
        public long? EquipmentID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Model { get; set; }
        public string? CalibrationNo { get; set; }
        public DateTime? CalibratedOn { get; set; }
        public DateTime? ValidUpto { get; set; }
    }

    public class SnapshotFactorDto
    {
        public string FactorType { get; set; } = "Multiplication Factor"; // "Multiplication Factor", "Dilution Factor", "Unit Conversion"
        public string FactorName { get; set; } = string.Empty;
        public decimal Value { get; set; } = 1.0m;
        public string? AppliedOn { get; set; } = "All Results";
        public string? Description { get; set; }
    }

    public class SnapshotMeasurementUncertaintyDto
    {
        public string UncertaintyType { get; set; } = "Expanded Uncertainty (k=2)";
        public decimal? Value { get; set; }
        public string? Unit { get; set; } = "%";
        public decimal CoverageFactor { get; set; } = 2.0m;
        public string Basis { get; set; } = "Type B"; // "Type A", "Type B"
        public string? Remarks { get; set; } = "As per ISO 17025";
    }

    public class SnapshotAcceptanceCriteriaDto
    {
        public string DecisionRule { get; set; } = "All Parameters Must Pass"; // "All Parameters Must Pass", "Configured Rule"
        public string OverallDecision { get; set; } = "Pass if all parameters within spec range";
        public string RoundingRule { get; set; } = "Round to nearest";
        public decimal RoundingPrecision { get; set; } = 0.01m;
        public string? PassFailThreshold { get; set; }
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
}
