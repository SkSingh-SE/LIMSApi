using System;
using System.Collections.Generic;

namespace LIMSApi.Dtos
{
    public class UniversalResultParameterDto
    {
        public long ID { get; set; }
        public long ParameterMasterID { get; set; }
        public string ParameterCode { get; set; } = string.Empty;
        public string? ParameterName { get; set; }
        public string InputType { get; set; } = "Decimal";
        public string? Unit { get; set; }
        public int DecimalPrecision { get; set; } = 2;
        public bool IsCalculated { get; set; }
        public bool IsMandatory { get; set; } = true;
        public bool IsReportable { get; set; } = true;
        public int DisplayOrder { get; set; }
        public string? RawValue { get; set; }
        public decimal? RawNumericValue { get; set; }
        public string? AppliedFactorCode { get; set; }
        public string? AppliedFactorOperation { get; set; }
        public decimal? FactoredValue { get; set; }
        public string? Formula { get; set; }
        public string? SubstitutionTrace { get; set; }
        public decimal? CalculatedValue { get; set; }
        public decimal? ComplianceValue { get; set; }
        public string? DisplayValue { get; set; }
        public string? ReportedValue { get; set; }
        public decimal? SpecMin { get; set; }
        public decimal? SpecMax { get; set; }
        public decimal? SpecTarget { get; set; }
        public decimal? MinTolerance { get; set; }
        public decimal? MaxTolerance { get; set; }
        public decimal? EffectiveMin { get; set; }
        public decimal? EffectiveMax { get; set; }
        public string? ToleranceSource { get; set; }
        public string? ToleranceType { get; set; }
        public decimal? AppliedTolerance { get; set; }
        public string? ComplianceValueSource { get; set; }
        public string RequirementStatus { get; set; } = "RESOLVED";
        public string Verdict { get; set; } = "NOT_EVALUATED";
        public decimal? CombinedUncertainty { get; set; }
        public decimal? ExpandedUncertainty { get; set; }
        public decimal? CoverageFactor { get; set; }
        public string? MUSource { get; set; }
        public bool GuardBandApplied { get; set; }
        public string? EvaluationNote { get; set; }
        public decimal? ComplianceMargin { get; set; }
        public string? MarginText { get; set; }
    }

    public class UniversalResultDetailDto
    {
        public long ID { get; set; }
        public long TestExecutionID { get; set; }
        public long UniversalTestGroupID { get; set; }
        public long BranchID { get; set; }
        public string? BranchName { get; set; }
        public long OrganizationID { get; set; }
        public int RevisionNo { get; set; }
        public string ResultStatus { get; set; } = "Draft";
        public string OverallDecision { get; set; } = "NOT_EVALUATED";
        public string? SnapshotHash { get; set; }
        public long? ExecutionConfigSnapshotID { get; set; }
        public string? DecisionRule { get; set; }
        public string? AcceptanceCriteriaCode { get; set; }
        public string ConcurrencyToken { get; set; } = string.Empty;
        public long? FinalizedBy { get; set; }
        public DateTime? FinalizedOn { get; set; }
        public long? ReviewerID { get; set; }
        public long? VerifiedBy { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string? ReviewRemarks { get; set; }
        public string? ApprovalRemarks { get; set; }
        public string? SampleNo { get; set; }
        public string? CaseNo { get; set; }
        public string? TestName { get; set; }
        public string? TestMethodName { get; set; }
        public string? SpecificationTitle { get; set; }
        public string? GradeName { get; set; }
        public int ExecutionNo { get; set; }
        public string ExecutionStatus { get; set; } = string.Empty;
        public List<UniversalResultParameterDto> Parameters { get; set; } = new();
        public List<UniversalReviewFindingDto> Findings { get; set; } = new();
        public List<UniversalResultAuditDto> Audits { get; set; } = new();
        public string? CalculationTraceJson { get; set; }
        public string? ComplianceSummaryJson { get; set; }
    }

    public class EvaluateResultRequestDto
    {
        public string? Remarks { get; set; }
    }

    public class FinalizeResultRequestDto
    {
        public string ConcurrencyToken { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    public class UniversalReviewFindingDto
    {
        public long ID { get; set; }
        public long UniversalTestResultID { get; set; }
        public long TestExecutionID { get; set; }
        public string FindingType { get; set; } = "Observation";
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = "Major";
        public string Status { get; set; } = "Open";
        public bool IsBlocking { get; set; } = true;
        public string? Resolution { get; set; }
        public long? ResolvedBy { get; set; }
        public DateTime? ResolvedOn { get; set; }
    }

    public class CreateFindingRequestDto
    {
        public string FindingType { get; set; } = "Observation";
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = "Major";
        public bool IsBlocking { get; set; } = true;
    }

    public class ResolveFindingRequestDto
    {
        public string Resolution { get; set; } = string.Empty;
    }

    public class AssignReviewerRequestDto
    {
        public long ReviewerID { get; set; }
        public string? Remarks { get; set; }
    }

    public class ReviewActionRequestDto
    {
        public string ConcurrencyToken { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    public class UniversalResultAuditDto
    {
        public long ID { get; set; }
        public string EventType { get; set; } = string.Empty;
        public long ActorID { get; set; }
        public string? ActorName { get; set; }
        public DateTime EventOn { get; set; }
        public string? SnapshotHash { get; set; }
        public int RevisionNo { get; set; }
        public string? DetailsJson { get; set; }
    }

    public class Phase9HandoffDto
    {
        public long TestExecutionID { get; set; }
        public long UniversalTestResultID { get; set; }
        public int ApprovedRevisionNo { get; set; }
        public string OverallDecision { get; set; } = string.Empty;
        public string? SnapshotHash { get; set; }
        public long? ExecutionConfigSnapshotID { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public List<UniversalResultParameterDto> ReportableParameters { get; set; } = new();
    }
}
