using System;
using System.Collections.Generic;

namespace LIMSApi.Dtos
{
    public class ConfigurationAdjustmentDraftDto
    {
        public long UniversalTestGroupID { get; set; }
        public string OverallReason { get; set; } = string.Empty;
        public string? ConcurrencyToken { get; set; }
        public List<ConfigurationAdjustmentItemDto> Items { get; set; } = new();
    }

    public class ConfigurationAdjustmentItemDto
    {
        public long? ID { get; set; }
        public long? ConfigurationAdjustmentID { get; set; }
        public long UniversalTestGroupID { get; set; }
        public string Section { get; set; } = string.Empty; // Parameters, Conditions, Equipment, Factors, Uncertainty, AcceptanceCriteria, Layout
        public string EntityType { get; set; } = string.Empty; // Parameter, Requirement, Condition, Equipment, Factor, Uncertainty, AcceptanceCriteria, Layout
        public long? EntityID { get; set; }
        public string? EntityCode { get; set; }
        public string? EntityName { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string ChangeType { get; set; } = string.Empty; // ADD, REMOVE, CHANGE, REPLACE, OVERRIDE
        public string? PlannedValue { get; set; }
        public string? EffectiveValue { get; set; }
        public string? PreviousAdjustedValue { get; set; }
        public string? NewAdjustedValue { get; set; }
        public string? AdjustedValue
        {
            get => NewAdjustedValue;
            set { if (value != null && string.IsNullOrEmpty(NewAdjustedValue)) NewAdjustedValue = value; }
        }
        public string Reason { get; set; } = string.Empty;

        public string AuthorizationStatus { get; set; } = "Pending";
        public long? AuthorizedBy { get; set; }
        public string? AuthorizedByName { get; set; }
        public DateTime? AuthorizedOn { get; set; }
        public long CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class ConfigurationAdjustmentDetailDto
    {
        public long ID { get; set; }
        public long UniversalTestGroupID { get; set; }
        public int AdjustmentNumber { get; set; }
        public string Status { get; set; } = "Draft"; // Draft, Applied, Approved
        public string OverallReason { get; set; } = string.Empty;
        public long CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? AppliedBy { get; set; }
        public string? AppliedByName { get; set; }
        public DateTime? AppliedOn { get; set; }
        public long? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string? ApprovalRemarks { get; set; }
        public long? RejectedBy { get; set; }
        public string? RejectedByName { get; set; }
        public DateTime? RejectedOn { get; set; }
        public string? RejectionReason { get; set; }
        public long BranchID { get; set; }
        public string? BranchName { get; set; }
        public string ConcurrencyToken { get; set; } = string.Empty;
        public List<ConfigurationAdjustmentItemDto> Items { get; set; } = new();
        public AdjustedConfigurationDto? AdjustedConfiguration { get; set; }
    }

    public class AdjustedConfigurationDto
    {
        public long UniversalTestGroupID { get; set; }
        public long ConfigurationAdjustmentID { get; set; }
        public long AdjustmentID { get => ConfigurationAdjustmentID; set => ConfigurationAdjustmentID = value; }
        public int AdjustmentNumber { get; set; }
        public string AdjustmentStatus { get; set; } = string.Empty;

        // Authoritative Planning Headers (Deviation)
        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodCode { get; set; }
        public string? TestMethodName { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public string? TestMethodVersionNumber { get; set; }
        public long? SpecificationHeaderID { get; set; }
        public string? SpecificationCode { get; set; }
        public string? SpecificationName { get; set; }
        public long? SpecificationVersionID { get; set; }
        public string? SpecificationVersionNumber { get; set; }
        public long? SpecificationGradeID { get; set; }
        public string? SpecificationGradeName { get; set; }

        public List<EffectiveParameterRowDto> Parameters { get; set; } = new();
        public List<EffectiveRequirementRowDto> Requirements { get; set; } = new();
        public List<EffectiveConditionRowDto> Conditions { get; set; } = new();
        public List<EffectiveEquipmentRowDto> Equipment { get; set; } = new();
        public List<EffectiveFactorRowDto> Factors { get; set; } = new();
        public EffectiveUncertaintyDto? Uncertainty { get; set; }
        public EffectiveAcceptanceCriteriaDto? AcceptanceCriteria { get; set; }
        public EffectiveLayoutDto Layout { get; set; } = null!;
        public ValidationSummaryDto Validation { get; set; } = null!;
        public int AddedCount { get; set; }
        public int RemovedCount { get; set; }
        public int ChangedCount { get; set; }
        public int ReplacedCount { get; set; }
        public int OverriddenCount { get; set; }
        public DateTime GeneratedAtUtc { get; set; }
    }

    public class ExecutionDeviationRequestDto
    {
        public long UniversalTestGroupID { get; set; }
        public string DeviationCategory { get; set; } = string.Empty; // TestMethod, Specification, Equipment, ExecutionLayout
        public long? TargetEntityID { get; set; }
        public long SelectedAlternativeID { get; set; }
        public long? SelectedAlternativeSecondaryID { get; set; } // e.g. VersionID or GradeID
        public string Reason { get; set; } = string.Empty;
        public string? EvidenceReference { get; set; }
        public string? ConcurrencyToken { get; set; }
        public bool SubmitForApproval { get; set; } = true;
    }

    public class DeviationOptionDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SecondaryInfo { get; set; }
        public long? SecondaryID { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class DeviationLookupResultDto
    {
        public long UniversalTestGroupID { get; set; }
        public string DeviationCategory { get; set; } = string.Empty;
        public string CurrentValueDisplay { get; set; } = string.Empty;
        public long? CurrentEntityID { get; set; }
        public List<DeviationOptionDto> AvailableOptions { get; set; } = new();
    }

    public class AdjustmentValidationResultDto
    {
        public bool IsValid { get; set; }
        public bool CanApply { get; set; }
        public List<string> BlockingErrors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<AdjustmentItemValidationDto> ItemValidations { get; set; } = new();
    }

    public class AdjustmentItemValidationDto
    {
        public string Section { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string? EntityCode { get; set; }
        public bool IsValid { get; set; }
        public string? Message { get; set; }
        public bool IsBlocking { get; set; }
    }

    public class ApplyAdjustmentRequestDto
    {
        public long UniversalTestGroupID { get; set; }
        public long ConfigurationAdjustmentID { get; set; }
        public string ConcurrencyToken { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    public class ApproveAdjustmentRequestDto
    {
        public long UniversalTestGroupID { get; set; }
        public long ConfigurationAdjustmentID { get; set; }
        public string ConcurrencyToken { get; set; } = string.Empty;
        public string? ApprovalRemarks { get; set; }
    }

    public class RejectAdjustmentRequestDto
    {
        public long UniversalTestGroupID { get; set; }
        public long ConfigurationAdjustmentID { get; set; }
        public string ConcurrencyToken { get; set; } = string.Empty;
        public string RejectionReason { get; set; } = string.Empty;
    }

    public class DifferenceAuditComparisonItemDto
    {
        public string Section { get; set; } = string.Empty; // Parameters, Requirements, Conditions, Equipment, Factors, Uncertainty, AcceptanceCriteria, Layout
        public string EntityType { get; set; } = string.Empty; // Parameter, Requirement, Condition, Equipment, Factor, Uncertainty, AcceptanceCriteria, Layout
        public long? EntityID { get; set; }
        public string? EntityCode { get; set; }
        public string? EntityName { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string Classification { get; set; } = "UNCHANGED"; // ADDED, REMOVED, CHANGED, UNCHANGED
        public string? PlannedValue { get; set; }
        public string? EffectiveValue { get; set; }
        public string? AdjustedValue { get; set; }
        public string? Reason { get; set; }
        public string? AuthorizationStatus { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? AuthorizedByName { get; set; }
        public DateTime? AuthorizedOn { get; set; }
    }

    public class ComprehensiveDifferenceAuditDto
    {
        public long UniversalTestGroupID { get; set; }
        public int AdjustmentNumber { get; set; }
        public string AdjustmentStatus { get; set; } = "No Adjustment";
        public int TotalItems { get; set; }
        public int AddedCount { get; set; }
        public int RemovedCount { get; set; }
        public int ChangedCount { get; set; }
        public int UnchangedCount { get; set; }
        public List<DifferenceAuditComparisonItemDto> ComparisonItems { get; set; } = new();
        public List<ConfigurationAdjustmentItemDto> AdjustmentItems { get; set; } = new();
        public List<ConfigurationAdjustmentDetailDto> History { get; set; } = new();
    }
}
