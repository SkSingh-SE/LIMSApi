using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    /// <summary>
    /// Per-parameter specification resolution status (Screen 14 Part C).
    /// Drives UI badge color and message; never hidden behind a flat "No Spec Requirement" string.
    /// </summary>
    public enum SpecificationResolutionStatus
    {
        /// <summary>Spec line found for (SpecVersion, SpecGrade, ParameterID). Limits/fomula applied.</summary>
        RESOLVED = 0,

        /// <summary>Standardless test (Header/Version both NULL). Spec gate is N/A.</summary>
        SPECIFICATION_NOT_APPLICABLE = 1,

        /// <summary>Spec line absent for this parameter; parameter is optional.</summary>
        NOT_CONFIGURED = 2,

        /// <summary>Spec line absent for this parameter; parameter is mandatory and blocks planning.</summary>
        MANDATORY_MISSING = 3,

        /// <summary>Spec line exists for the parameter but a different version was expected.</summary>
        VERSION_MISMATCH = 4,

        /// <summary>Spec line exists for the parameter but a different grade was expected.</summary>
        GRADE_MISMATCH = 5,

        /// <summary>Spec line exists for the parameter but for a different test definition.</summary>
        PARAMETER_MISMATCH = 6,

        /// <summary>Multiple spec lines conflict for the same parameter — ambiguous.</summary>
        AMBIGUOUS_CONFIGURATION = 7,

        /// <summary>Configuration is structurally invalid (e.g. spec header/grade/version parity broken).</summary>
        INVALID_CONFIGURATION = 8
    }

    // Workspace Header & Overview
    public class UniversalPlanWorkspaceDto
    {
        public long InwardID { get; set; }
        public string CaseNo { get; set; } = string.Empty;
        public long SampleID { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public string? SampleDetails { get; set; }
        public DateTime InwardDate { get; set; }

        public long? CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public long? ProductMasterID { get; set; }
        public string? ProductMasterName { get; set; }

        public long? SpecificationGradeID { get; set; }
        public string? GradeName { get; set; }

        public long? SpecificationHeaderID { get; set; }
        public string? SpecificationName { get; set; }
        public string? StandardReference { get; set; }

        public long? SpecificationVersionID { get; set; }
        public string? SpecificationVersionName { get; set; }
        public bool IsSupersededSpecVersion { get; set; }

        public List<SpecificationVersionOptionDto> AvailableSpecVersions { get; set; } = new();

        public long BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public List<BranchOptionDto> AvailableBranches { get; set; } = new();

        public long SampleTestPlanID { get; set; }
        public string PlanStatus { get; set; } = "Draft"; // Draft, Submitted, Approved
        public int PlanVersion { get; set; } = 1;
        public bool IsPlanLocked { get; set; } = false; // true if PlanStatus is Submitted or Approved

        public List<UniversalPlanSampleSummaryDto> AvailableSamples { get; set; } = new();
        public List<UniversalTestCardDto> AvailableTests { get; set; } = new();
        public List<PlannedUniversalTestDto> PlannedTests { get; set; } = new();
    }

    public class SpecificationVersionOptionDto
    {
        public long ID { get; set; }
        public string Version { get; set; } = string.Empty;
        public string? Year { get; set; }
        public string Status { get; set; } = string.Empty; // Active, Superseded
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuperseded { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? SupersededDate { get; set; }
    }

    public class BranchOptionDto
    {
        public long ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class UniversalTestCardDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long? DisciplineID { get; set; }
        public string? DisciplineName { get; set; }
        public long? LabDepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int ParameterCount { get; set; }
        public int MethodCount { get; set; }
        public int ConditionCount { get; set; }
        public bool IsReady { get; set; }
        public string ReadinessMessage { get; set; } = string.Empty;
    }

    public class PlannedUniversalTestDto
    {
        public long UniversalTestGroupID { get; set; }
        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestCode { get; set; } = string.Empty;
        public string LaboratoryTestName { get; set; } = string.Empty;
        public string? DisciplineName { get; set; }

        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodName { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public string? TestMethodVersion { get; set; }
        public bool IsSupersededMethodVersion { get; set; }

        public long? SpecificationHeaderID { get; set; }
        public string? SpecificationName { get; set; }
        public long? SpecificationVersionID { get; set; }
        public string? SpecificationVersionName { get; set; }
        public long? SpecificationGradeID { get; set; }
        public string? GradeName { get; set; }

        public long BranchID { get; set; }
        public string? BranchName { get; set; }
        public long? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed
        public long? TestExecutionID { get; set; }
        public string? ExecutionStatus { get; set; }
    }

    // Effective Configuration Preview Request & Response
    public class UniversalPlanPreviewRequestDto
    {
        public long SampleID { get; set; }
        public long LaboratoryTestID { get; set; }
        public long? SpecificationHeaderID { get; set; }
        public long? SpecificationVersionID { get; set; }
        public long? SpecificationGradeID { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public long BranchID { get; set; }
        public DateTime? ReferenceDate { get; set; }
    }

    public class UniversalPlanPreviewResponseDto
    {
        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestCode { get; set; } = string.Empty;
        public string LaboratoryTestName { get; set; } = string.Empty;
        public long? DisciplineID { get; set; }
        public string? DisciplineName { get; set; }

        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodName { get; set; }
        public string? TestMethodStandard { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public string? TestMethodVersion { get; set; }
        public bool IsSupersededMethodVersion { get; set; }
        public List<TestMethodVersionOptionDto> AvailableMethodVersions { get; set; } = new();

        public long? SpecificationHeaderID { get; set; }
        public string? SpecificationTitle { get; set; }
        /// <summary>0 means no spec version resolved (standardless). Use alongside SpecificationHeaderID to determine parity.</summary>
        public long SpecificationVersionID { get; set; }
        public string? SpecificationVersionNumber { get; set; }
        public bool IsSupersededSpecVersion { get; set; }
        public long? SpecificationGradeID { get; set; }
        public string? GradeName { get; set; }

        /// <summary>True when Header/Version parity invariant holds: either both NULL or both non-NULL.</summary>
        public bool IsStandardlessTest { get; set; }

        public long BranchID { get; set; }
        public string? BranchName { get; set; }
        public long? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public string DepartmentRoutingSource { get; set; } = "Resolved from Branch + Discipline";

        public List<PreviewParameterDto> Parameters { get; set; } = new();
        public List<PreviewConditionDto> Conditions { get; set; } = new();

        public UniversalPlanValidationSummaryDto ValidationSummary { get; set; } = new();
        public bool IsConfigurationReady { get; set; }

        /// <summary>Authoritative tenant context (organization, branch, company code) — never a hardcoded fallback.</summary>
        public TenantContextDto? Tenant { get; set; }
    }

    public class TenantContextDto
    {
        public long OrganizationID { get; set; }
        public long BranchID { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
    }

    public class TestMethodVersionOptionDto
    {
        public long ID { get; set; }
        public string Version { get; set; } = string.Empty;
        public string? Year { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuperseded { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? SupersededDate { get; set; }
    }

    public class PreviewParameterDto
    {
        public long ParameterID { get; set; }
        public string ParameterCode { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
        public string? ParameterUnit { get; set; }
        public string? InputType { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsReportable { get; set; }
        public string RequirementText { get; set; } = string.Empty; // e.g. "≥ 205 MPa"
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? MinTolerance { get; set; }
        public decimal? MaxTolerance { get; set; }
        public string? AcceptanceCriteria { get; set; }
        public string? Equation { get; set; }
        public string? Note { get; set; }
        public bool HasRequirement { get; set; }

        /// <summary>Authoritative resolution status (Screen 14 Part C).</summary>
        public SpecificationResolutionStatus ResolutionStatus { get; set; } = SpecificationResolutionStatus.NOT_CONFIGURED;

        /// <summary>Human-readable reason (e.g. "No requirement configured for UTS + Fe500D + IS1786 Rev 2008").</summary>
        public string? ResolutionReason { get; set; }

        /// <summary>ID of the resolved SpecificationLine (if any).</summary>
        public long? SpecificationLineID { get; set; }

        /// <summary>Backing SpecificationVersionID (for traceability).</summary>
        public long? SourceSpecificationVersionID { get; set; }

        public string Status { get; set; } = "Required"; // Required / Optional / Missing
    }

    public class PreviewConditionDto
    {
        public long ConditionMasterID { get; set; }
        public string ConditionCode { get; set; } = string.Empty;
        public string ConditionName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? ParameterUnit { get; set; }
        public bool IsMandatory { get; set; }
        public string ConfiguredValue { get; set; } = string.Empty; // e.g. "20–30 °C"
        public bool HasConfiguration { get; set; }
        public string Status { get; set; } = "Required";
    }

    public class UniversalPlanValidationSummaryDto
    {
        public bool SamplePass { get; set; } = true;
        public string? SampleMessage { get; set; }

        public bool ProductGradePass { get; set; } = true;
        public string? ProductGradeMessage { get; set; }

        public bool SpecificationPass { get; set; } = true;
        public string? SpecificationMessage { get; set; }

        public bool SpecificationVersionPass { get; set; } = true;
        public string? SpecificationVersionMessage { get; set; }

        public bool TestDefinitionPass { get; set; } = true;
        public string? TestDefinitionMessage { get; set; }

        public bool MandatoryParametersPass { get; set; } = true;
        public string? MandatoryParametersMessage { get; set; }

        public bool TestMethodPass { get; set; } = true;
        public string? TestMethodMessage { get; set; }

        public bool MethodVersionPass { get; set; } = true;
        public string? MethodVersionMessage { get; set; }

        public bool BranchPass { get; set; } = true;
        public string? BranchMessage { get; set; }

        public bool DepartmentRoutingPass { get; set; } = true;
        public string? DepartmentRoutingMessage { get; set; }

        public bool RequiredConditionsPass { get; set; } = true;
        public string? RequiredConditionsMessage { get; set; }

        public bool AllPassed => SamplePass && ProductGradePass && SpecificationPass &&
                                SpecificationVersionPass && TestDefinitionPass && MandatoryParametersPass &&
                                TestMethodPass && MethodVersionPass && BranchPass &&
                                DepartmentRoutingPass && RequiredConditionsPass &&
                                (!BlockingErrors?.Any() ?? true);

        public List<string> BlockingErrors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    // Save Draft & Confirm Requests
    public class UniversalPlanSaveDto
    {
        public long SampleTestPlanID { get; set; }
        public long SampleID { get; set; }
        public long BranchID { get; set; }
        public long? SpecificationHeaderID { get; set; }
        public long? SpecificationGradeID { get; set; }
        public long? SpecificationVersionID { get; set; }
        public List<UniversalPlanTestItemDto> Tests { get; set; } = new();
    }

    public class UniversalPlanTestItemDto
    {
        public long? UniversalTestGroupID { get; set; }
        public long LaboratoryTestID { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public long? SpecificationHeaderID { get; set; }
        public long? SpecificationGradeID { get; set; }
        public long? SpecificationVersionID { get; set; }
        public long? BranchID { get; set; }
        public bool IsRetest { get; set; } = false;
    }

    public class UniversalPlanConfirmDto
    {
        public long SampleTestPlanID { get; set; }
        public long SampleID { get; set; }
        public long BranchID { get; set; }
        public long? SpecificationHeaderID { get; set; }
        public long? SpecificationGradeID { get; set; }
        public long? SpecificationVersionID { get; set; }
        public List<UniversalPlanTestItemDto> Tests { get; set; } = new();
    }

    public class UniversalPlanConfirmResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public long SampleTestPlanID { get; set; }
        public string PlanStatus { get; set; } = string.Empty;
        public List<long> CreatedUniversalTestGroupIDs { get; set; } = new();
    }

    public class UniversalPlanSampleSummaryDto
    {
        public long SampleID { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public string? SampleDetails { get; set; }
        public long? ProductMasterID { get; set; }
        public string? ProductName { get; set; }
        public long? SpecificationGradeID { get; set; }
        public string? GradeName { get; set; }
        public int Quantity { get; set; }
        public string PlanStatus { get; set; } = "Draft"; // Draft, Submitted, Approved, Cancelled
        public int PlannedTestCount { get; set; }
        public bool IsSelected { get; set; }
    }

    public class UniversalPlanCopyRequestDto
    {
        [Required]
        public long SourceSampleID { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one target sample must be selected.")]
        public List<long> TargetSampleIDs { get; set; } = new();

        public long? ExecutionBranchID { get; set; }
    }

    public class UniversalPlanCopyResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalTargets { get; set; }
        public int SuccessfulCopies { get; set; }
        public int FailedCopies { get; set; }
        public List<UniversalPlanTargetCopyDetailDto> Details { get; set; } = new();
    }

    public class UniversalPlanTargetCopyDetailDto
    {
        public long SampleID { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public bool Success { get; set; }
        public int CopiedTestCount { get; set; }
        public string? Message { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
    }
}
