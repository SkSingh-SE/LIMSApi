using System;
using System.Collections.Generic;
using LIMSApi.Dtos;

namespace LIMSApi.Dtos
{
    /// <summary>
    /// Phase 9 report envelope. All values are assembled from frozen sources only:
    /// ExecutionConfigSnapshot.ConfigJson + UniversalTestResult(revision) + Review/Approval + ReportDataJson freeze.
    /// No live master re-resolution, no recalculation.
    /// </summary>
    public class UniversalReportPreviewDto
    {
        public long TestExecutionID { get; set; }
        public long UniversalTestResultID { get; set; }
        public int ResultRevisionNo { get; set; }
        public string OverallDecision { get; set; } = string.Empty;
        public string? SnapshotHash { get; set; }
        public bool IsPreview { get; set; } = true;
        public string Watermark { get; set; } = "UNCONTROLLED PREVIEW";
        public UniversalReportDataDto Data { get; set; } = new();
        public string? ReportDataHash { get; set; }
        public string? ReportFormatCode { get; set; }
        public string? ReportFormatSource { get; set; }
    }

    /// <summary>
    /// Canonical frozen presentation payload. Stored as ReportDataJson on Generate.
    /// Display identity (lab/branch/customer/signatory/NABL-display) is frozen here at Generate time.
    /// </summary>
    public class UniversalReportDataDto
    {
        // Identity (frozen display values)
        public string? ReportNo { get; set; }
        public long TestExecutionID { get; set; }
        public int ExecutionNo { get; set; }
        public bool IsRetest { get; set; }
        public long? PreviousExecutionID { get; set; }
        public long UniversalTestGroupID { get; set; }
        public long UniversalTestResultID { get; set; }
        public int ResultRevisionNo { get; set; }
        public int ReportRevisionNo { get; set; }
        public string OverallDecision { get; set; } = string.Empty;
        public string? DecisionRule { get; set; }
        public string? AcceptanceCriteriaCode { get; set; }
        public string? SnapshotHash { get; set; }
        public long? ExecutionConfigSnapshotID { get; set; }

        // Laboratory / branch (FROZEN display copy at Generate)
        public string? LabName { get; set; }
        public string? LabAddress { get; set; }
        public string? LabPhone { get; set; }
        public string? LabEmail { get; set; }
        public string? LabWebsite { get; set; }
        public string? LabSubtitle { get; set; }
        public string? LabTagline { get; set; }
        public string? LabLogoPath { get; set; }
        public string? BranchName { get; set; }
        public string? BranchAddress { get; set; }
        public string? DepartmentName { get; set; }

        // Customer / sample (FROZEN display copy at Generate)
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? ContactPerson { get; set; }
        public string? CustomerReference { get; set; }
        public string? CaseNo { get; set; }
        public string? SampleNo { get; set; }
        public string? SampleDescription { get; set; }
        public string? GradeName { get; set; }
        public string? SampleCondition { get; set; }
        public string? SamplingSource { get; set; }
        public string? SamplingPlan { get; set; }
        public DateTime? SampleReceivedOn { get; set; }
        public DateTime? TestCommencedOn { get; set; }
        public DateTime? TestCompletedOn { get; set; }
        public DateTime? DateTested { get; set; }

        // Test & method & specification (FROZEN from snapshot)
        public string? LaboratoryTestName { get; set; }
        public string? LaboratoryTestCode { get; set; }
        public string? DisciplineName { get; set; }
        public string? TestMethodName { get; set; }
        public string? TestMethodStandard { get; set; }
        public string? TestMethodVersion { get; set; }
        public string? SpecificationTitle { get; set; }
        public string? ExecutionLayoutCode { get; set; }
        public string? ExecutionLayoutName { get; set; }
        public string? RendererType { get; set; }
        public string? TestEquipment { get; set; }
        public string? EnvironmentalConditions { get; set; }

        // Execution actuals + frozen config sections (presentation only, never recomputed)
        public List<SnapshotParameterDto> Parameters { get; set; } = new();
        public List<SnapshotConditionDto> Conditions { get; set; } = new();
        public List<SnapshotEquipmentDto> Equipment { get; set; } = new();
        public List<SnapshotFactorDto> Factors { get; set; } = new();
        public SnapshotMeasurementUncertaintyDto? MeasurementUncertainty { get; set; }
        public SnapshotAcceptanceCriteriaDto? AcceptanceCriteria { get; set; }
        public SnapshotScopeDto? Scope { get; set; }
        public string? ActualConditionsJson { get; set; }
        public string? ActualEquipmentJson { get; set; }

        // Final result / compliance (FINALIZED values only)
        public List<UniversalResultParameterDto> ResultParameters { get; set; } = new();
        public string? CalculationTraceJson { get; set; }
        public string? ComplianceSummaryJson { get; set; }

        // NABL / accreditation representation (derived from FROZEN scope status only)
        public string? ScopeStatus { get; set; }
        public bool ShowNablMark { get; set; }
        public string? NablCertNo { get; set; }
        public string? NablLogoPath { get; set; }
        public string? UlrNo { get; set; }
        public string? ScopeDisclaimer { get; set; }

        // Format metadata (Dual rendering mode: DEFAULT or CONFIGURED)
        public string? ReportFormatCode { get; set; } = "DEFAULT";
        public string? ReportFormatName { get; set; } = "Default Universal Report";
        public string ReportFormatSource { get; set; } = "DEFAULT"; // "DEFAULT" | "CONFIGURED"

        // NABL & ULR Governance (NABL 133 & June 2026 Policy)
        public bool IsAccreditedLaboratory { get; set; }
        public string? AccreditationStatus { get; set; }
        public string? AccreditationCertificateNo { get; set; }
        public DateTime? AccreditationValidFrom { get; set; }
        public DateTime? AccreditationValidTo { get; set; }
        public long? AccreditedBranchID { get; set; }
        public bool IsWithinAccreditedScope { get; set; }
        public string? ScopeReference { get; set; }
        public bool NablSymbolAllowed { get; set; }
        public bool ULRApplicable { get; set; }
        public bool ULRRequired { get; set; }
        public string? ULRExceptionReason { get; set; }

        // Review / verification / approval (FINAL states only)
        public long? AnalystID { get; set; }
        public string? AnalystName { get; set; }
        public string? AnalystDesignation { get; set; }
        public string? AnalystSignaturePath { get; set; }

        public long? ReviewerID { get; set; }
        public string? ReviewerName { get; set; }
        public string? ReviewerDesignation { get; set; }
        public string? ReviewerSignaturePath { get; set; }

        public long? VerifiedBy { get; set; }
        public string? VerifiedByName { get; set; }
        public DateTime? VerifiedOn { get; set; }

        public long? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public string? ApproverDesignation { get; set; }
        public string? ApproverSignaturePath { get; set; }
        public DateTime? ApprovedOn { get; set; }

        public string? ReviewRemarks { get; set; }
        public string? ApprovalRemarks { get; set; }
        public List<UniversalReviewFindingDto> Findings { get; set; } = new();

        // NOTE: audit trail is NOT part of the frozen document hash. Audits grow on every
        // preview/download (operational history) and stay queryable via UniversalResultAudit.
        // Freezing them would make ReportDataHash non-deterministic and break the golden test.
    }

    public class UniversalReportListItemDto
    {
        public long ID { get; set; }
        public long TestExecutionID { get; set; }
        public long UniversalTestResultID { get; set; }
        public int ResultRevisionNo { get; set; }
        public int ReportRevisionNo { get; set; }
        public string ReportNo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? OverallDecision { get; set; }
        public string? ReportDataHash { get; set; }
        public string? PdfHash { get; set; }
        public string? UlrNo { get; set; }
        public bool ShowNablMark { get; set; }
        public string? ReportFormatSource { get; set; }
        public DateTime? GeneratedOn { get; set; }
        public DateTime? ReleasedOn { get; set; }
        public string? GeneratedByName { get; set; }
        public string? ReleasedByName { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class UniversalReportDetailDto : UniversalReportListItemDto
    {
        public string? SnapshotHash { get; set; }
        public string? ResultRevisionHash { get; set; }
        public string? PdfPath { get; set; }
        public long BranchID { get; set; }
        public long OrganizationID { get; set; }
        public UniversalReportDataDto Data { get; set; } = new();
    }

    public class GenerateUniversalReportRequestDto
    {
        public string? Remarks { get; set; }
        public string? ReportFormatCode { get; set; }
    }

    public class ReleaseUniversalReportRequestDto
    {
        public string? Remarks { get; set; }
    }

    public class ReissueUniversalReportRequestDto
    {
        public string? Reason { get; set; }
    }

    public class ReportFormatOptionDto
    {
        public string FormatCode { get; set; } = string.Empty;
        public string FormatName { get; set; } = string.Empty;
        public string PageLayout { get; set; } = "Portrait";
        public bool IsDefault { get; set; }
    }
}
