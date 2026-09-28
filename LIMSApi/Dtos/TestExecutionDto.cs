using System;
using System.Collections.Generic;

namespace LIMSApi.Dtos
{
    public class TestExecutionDto
    {
        public long ID { get; set; }
        public long UniversalTestGroupID { get; set; }
        public long BranchID { get; set; }
        public string? BranchName { get; set; }
        public long OrganizationID { get; set; }
        public long? ExecutionAnalystID { get; set; }
        public string? ExecutionAnalystName { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public long? VerifiedBy { get; set; }
        public string? VerifiedByName { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public long? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string? ReviewRemarks { get; set; }
        public int ExecutionNo { get; set; } = 1;
        public bool IsRetest { get; set; } = false;
        public long? PreviousExecutionID { get; set; }
        public string Status { get; set; } = "InProgress";
        public long? ExecutionConfigSnapshotID { get; set; }
        public TestExecutionConfigSnapshotDto? ConfigSnapshot { get; set; }
        public List<TestSpecimenDto> Specimens { get; set; } = new();

        // Sample Context Information
        public long? SampleID { get; set; }
        public string? SampleNo { get; set; }
        public string? CustomerName { get; set; }
        public string? SampleDescription { get; set; }
        public string? CaseNo { get; set; }
        public string? TestName { get; set; }
        public string? StandardName { get; set; }
        public string? TestMethodVersion { get; set; }
        public string? SpecificationTitle { get; set; }
        public string? GradeName { get; set; }
        public string? ActualConditionsJson { get; set; }
        public string? ActualEquipmentJson { get; set; }
        public List<ActualConditionEntryDto> ActualConditions { get; set; } = new();
        public List<ActualEquipmentEntryDto> ActualEquipment { get; set; } = new();
    }

    public class TestSpecimenDto
    {
        public long ID { get; set; }
        public long TestExecutionID { get; set; }
        public int SequenceNo { get; set; }
        public string? SpecimenIdentifier { get; set; }
        public bool IsDiscarded { get; set; }
        public List<TestObservationDto> Observations { get; set; } = new();
    }

    public class TestObservationDto
    {
        public long ID { get; set; }
        public long TestSpecimenID { get; set; }
        public int ReadingNo { get; set; }
        public List<ParameterObservationResultDto> ParameterResults { get; set; } = new();
    }

    public class ParameterObservationResultDto
    {
        public long ID { get; set; }
        public long TestObservationID { get; set; }
        public long ParameterMasterID { get; set; }
        public string ParameterCode { get; set; } = string.Empty;
        public string? ParameterName { get; set; }
        public string? RawValue { get; set; }
        public decimal? NumericValue { get; set; }
        public string? CalculatedValue { get; set; }
        public bool IsFormulaCalculated { get; set; }
        public decimal? SpecMin { get; set; }
        public decimal? SpecMax { get; set; }
        public string? ResultStatus { get; set; } // "Pass", "Fail", "Marginal", "NotApplicable"
    }

    public class TestExecutionSaveDto
    {
        public List<TestSpecimenSaveDto> Specimens { get; set; } = new();
        public List<SnapshotConditionSaveDto>? Conditions { get; set; }
        public List<ActualConditionEntryDto>? ActualConditions { get; set; }
        public List<ActualEquipmentEntryDto>? ActualEquipment { get; set; }
        public string? Remarks { get; set; }
        public string? ExecutionRemarks { get; set; }
    }

    public class ActualConditionEntryDto
    {
        public long? ConditionMasterID { get; set; }
        public long? ConditionDimensionID { get => ConditionMasterID; set => ConditionMasterID = value; }
        public string? DimensionName { get; set; }
        public string? DimensionCode { get; set; }
        public string? ConfiguredValue { get; set; }
        public string? ActualValue { get; set; }
        public string? SelectedExecutionValue { get => ActualValue; set => ActualValue = value; }
        public string? ActualExecutionValue { get => ActualValue; set => ActualValue = value; }
        public string? Unit { get; set; }
        public string? Remarks { get; set; }
        public DateTime ObservedOnUtc { get; set; } = DateTime.UtcNow;
        public long? AnalystID { get; set; }
    }

    public class ActualEquipmentEntryDto
    {
        public long EquipmentID { get => EquipmentMasterID ?? 0; set => EquipmentMasterID = value; }
        public long? EquipmentRequirementMasterID { get; set; }
        public long? EquipmentRequirementID { get => EquipmentRequirementMasterID; set => EquipmentRequirementMasterID = value; }
        public long? EquipmentMasterID { get; set; }
        public string? EquipmentCode { get; set; }
        public string? EquipmentName { get; set; }
        public string? Name { get => EquipmentName; set => EquipmentName = value; }
        public string? Model { get; set; }
        public string? EquipmentType { get; set; }
        public string? CalibrationCertificateNo { get; set; }
        public string? CalibrationNo { get => CalibrationCertificateNo; set => CalibrationCertificateNo = value; }
        public string? CalibrationDueDate { get; set; }
        public DateTime? CalibratedOn { get; set; }
        public DateTime? ValidUpto { get; set; }
        public string? CalibrationStatus { get; set; }
        public string? ReadinessStatus { get; set; }
        public string? Remarks { get; set; }
    }

    public class TestSpecimenSaveDto
    {
        public long? ID { get; set; } // 0 or null for new specimen
        public int SequenceNo { get; set; }
        public string? SpecimenIdentifier { get; set; }
        public bool IsDiscarded { get; set; }
        public List<TestObservationSaveDto> Observations { get; set; } = new();
    }

    public class TestObservationSaveDto
    {
        public long? ID { get; set; } // 0 or null for new observation
        public int ReadingNo { get; set; }
        public List<ParameterObservationResultSaveDto> ParameterResults { get; set; } = new();
        public List<ParameterObservationResultSaveDto> Results { get => ParameterResults; set => ParameterResults = value; }
    }

    public class ParameterObservationResultSaveDto
    {
        public long? ID { get; set; } // 0 or null for new result
        public long ParameterMasterID { get; set; }
        public string ParameterCode { get; set; } = string.Empty;
        public string? RawValue { get; set; }
        public decimal? NumericValue { get; set; }
        public string? CalculatedValue { get; set; }
        public bool IsFormulaCalculated { get; set; }
    }

    public class SnapshotConditionSaveDto
    {
        public long? ConditionDimensionID { get; set; }
        public string? DimensionName { get; set; }
        public string? DimensionCode { get; set; }
        public string? Unit { get; set; }
        public string? ConfiguredValue { get; set; }
        public string? SelectedExecutionValue { get; set; }
        public string? ActualExecutionValue { get => SelectedExecutionValue; set => SelectedExecutionValue = value; }
        public string? Remarks { get; set; }
    }

    public class ExecutionActionDto
    {
        public string? Remarks { get; set; }
    }

    public class ExecutionCalculationTraceDto
    {
        public string DependencyDAG { get; set; } = string.Empty;
        public List<ParameterCalculationStepDto> Steps { get; set; } = new();
    }

    public class ParameterCalculationStepDto
    {
        public string ParameterCode { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
        public string Formula { get; set; } = string.Empty;
        public List<string> Dependencies { get; set; } = new();
        public string SubstitutionTrace { get; set; } = string.Empty;
        public string FormattedResult { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ResultsOverviewDto
    {
        public int TotalReadings { get; set; }
        public int CalculatedResults { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public int NotApplicableCount { get; set; }
        public string OverallDecision { get; set; } = "Pending"; // "Pass", "Fail", "Pending"
        public List<ParameterOverviewItemDto> Parameters { get; set; } = new();
        public List<SpecimenOverviewItemDto> Specimens { get; set; } = new();
    }

    public class ParameterOverviewItemDto
    {
        public string ParameterCode { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public string InputType { get; set; } = "Decimal";
        public string? SpecRange { get; set; }
        public string? AverageValue { get; set; }
        public string? FinalValue { get; set; }
        public string Status { get; set; } = "Pending";
    }

    public class SpecimenOverviewItemDto
    {
        public int SequenceNo { get; set; }
        public string? SpecimenIdentifier { get; set; }
        public int ReadingCount { get; set; }
        public string ComplianceStatus { get; set; } = "Pending";
    }

    public class NablScopeSummaryDto
    {
        public int InScopeCount { get; set; }
        public int OutOfScopeCount { get; set; }
        public string ScopeStatus { get; set; } = "None"; // "Full", "Partial", "None"
        public List<string> InScopeParameters { get; set; } = new();
        public List<string> OutOfScopeParameters { get; set; } = new();
    }

    public class FormulaPreviewRequestDto
    {
        public string Formula { get; set; } = string.Empty;
        public Dictionary<string, double> Variables { get; set; } = new();
        public int Precision { get; set; } = 2;
    }

    public class FormulaPreviewResponseDto
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public double? Result { get; set; }
        public string? FormattedResult { get; set; }
        public string SubstitutionTrace { get; set; } = string.Empty;
        public List<string> Dependencies { get; set; } = new();
    }

    public class ExecutionAttachmentUploadDto
    {
        public string FileName { get; set; } = string.Empty;
        public string? FileType { get; set; } = "PDF";
        public string? FileUrl { get; set; }
        public long? FileSizeBytes { get; set; }
    }

    public class RetestRequestDto
    {
        public string ReasonCode { get; set; } = string.Empty;
        public string Justification { get; set; } = string.Empty;
    }
}
