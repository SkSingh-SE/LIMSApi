namespace LIMSApi.Dtos
{
    public class UniversalTestGroupListItemDto
    {
        public long ID { get; set; }
        public long SampleTestPlanID { get; set; }
        public long SampleID { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public string InwardCaseNo { get; set; } = string.Empty;
        public long InwardID { get; set; }
        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestCode { get; set; } = string.Empty;
        public string LaboratoryTestName { get; set; } = string.Empty;
        public string? DisciplineName { get; set; }
        public string Status { get; set; } = "Pending";
        public long BranchID { get; set; }
        public string? BranchName { get; set; }
        public string? DepartmentName { get; set; }
        public bool HasExecution { get; set; }
        public long? LatestExecutionID { get; set; }
        public string? ExecutionStatus { get; set; }
        public bool HasSnapshot { get; set; }
        public bool HasAdjustment { get; set; }
        public string? AdjustmentStatus { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class UniversalTestGroupDetailDto
    {
        public long ID { get; set; }
        public long SampleTestPlanID { get; set; }
        public long SampleID { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public string InwardCaseNo { get; set; } = string.Empty;
        public long InwardID { get; set; }
        public DateTime InwardDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public long? CustomerID { get; set; }

        public string? SampleDetails { get; set; }
        public long? ProductMasterID { get; set; }
        public string? ProductName { get; set; }
        public long? SpecificationGradeID { get; set; }
        public string? GradeName { get; set; }
        public long? SpecificationHeaderID { get; set; }
        public string? SpecificationTitle { get; set; }
        public string? StandardReference { get; set; }
        public long? SpecificationVersionID { get; set; }
        public string? SpecificationVersionName { get; set; }
        public string? SpecificationVersionStatus { get; set; }
        public bool IsSupersededSpecVersion { get; set; }
        public bool IsStandardless { get; set; }

        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestCode { get; set; } = string.Empty;
        public string LaboratoryTestName { get; set; } = string.Empty;
        public string? LaboratoryTestDescription { get; set; }
        public long? DisciplineID { get; set; }
        public string? DisciplineName { get; set; }
        public long? LabDepartmentID { get; set; }

        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodCode { get; set; }
        public string? TestMethodName { get; set; }
        public string? TestMethodStandard { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public string? TestMethodVersion { get; set; }
        public string? TestMethodVersionStatus { get; set; }
        public bool IsSupersededMethodVersion { get; set; }
        public DateTime? MethodEffectiveDate { get; set; }

        public long BranchID { get; set; }
        public string? BranchName { get; set; }
        public long? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public string DepartmentRoutingSource { get; set; } = string.Empty;

        public long? ExecutionLayoutID { get; set; }
        public string? ExecutionLayoutCode { get; set; }
        public string? ExecutionLayoutName { get; set; }
        public string? RendererType { get; set; }
        public string? PlannedConfigurationJson { get; set; }

        public string Status { get; set; } = "Pending";
        public string CompanyCode { get; set; } = string.Empty;
        public long OrganizationID { get; set; }

        public DateTime CreatedOn { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedByName { get; set; }

        public bool HasExecution { get; set; }
        public long? LatestExecutionID { get; set; }
        public string? LatestExecutionStatus { get; set; }
        public bool HasSnapshot { get; set; }

        public bool CanOpenExecution { get; set; }
        public string CanOpenExecutionReason { get; set; } = string.Empty;

        public bool HasAdjustment { get; set; }
        public string? AdjustmentStatus { get; set; }
        public string? AdjustmentNumber { get; set; }
        public long? AdjustmentID { get; set; }

        public List<long> SiblingTestGroupIDs { get; set; } = new();
    }

    public class EffectiveConfigurationDto
    {
        public long UniversalTestGroupID { get; set; }
        public UniversalTestGroupDetailDto TestGroup { get; set; } = null!;
        public PlannedConfigurationSnapshotDto? PlannedBaseline { get; set; }
        public UniversalPlanPreviewResponseDto EffectiveConfiguration { get; set; } = null!;
        public TestExecutionConfigSnapshotDto SnapshotPreview { get; set; } = null!;
        public List<EffectiveParameterRowDto> Parameters { get; set; } = new();
        public List<EffectiveRequirementRowDto> Requirements { get; set; } = new();
        public List<EffectiveConditionRowDto> Conditions { get; set; } = new();
        public EffectiveMethodDto Method { get; set; } = null!;
        public List<EffectiveEquipmentRowDto> Equipment { get; set; } = new();
        public List<EffectiveFactorRowDto> Factors { get; set; } = new();
        public EffectiveUncertaintyDto? Uncertainty { get; set; }
        public EffectiveAcceptanceCriteriaDto? AcceptanceCriteria { get; set; }
        public EffectiveLayoutDto Layout { get; set; } = null!;
        public ValidationSummaryDto Validation { get; set; } = null!;
    }

    public class EffectiveParameterRowDto
    {
        public long ParameterID { get; set; }
        public string ParameterCode { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
        public string? Symbol { get; set; }
        public string? Unit { get; set; }
        public string UnitSource { get; set; } = string.Empty;
        public string InputType { get; set; } = string.Empty;
        public int? DecimalPrecision { get; set; }
        public string? CalculationRole { get; set; }
        public bool IsCalculated { get; set; }
        public string? Formula { get; set; }
        public List<string> FormulaDependencies { get; set; } = new();
        public bool IsMandatory { get; set; }
        public bool IsReportable { get; set; }
        public string RequirementText { get; set; } = string.Empty;
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? MinTolerance { get; set; }
        public decimal? MaxTolerance { get; set; }
        public string? AcceptanceCriteria { get; set; }
        public string ResolutionStatus { get; set; } = string.Empty;
        public string? ResolutionReason { get; set; }
        public string RequirementSource { get; set; } = string.Empty;
        public long? SpecificationLineID { get; set; }
        public string Status { get; set; } = string.Empty;

        // Current Master Comparison & Drift
        public string? MasterUnit { get; set; }
        public string? MasterFormula { get; set; }
        public decimal? MasterMinValue { get; set; }
        public decimal? MasterMaxValue { get; set; }
        public int? MasterDecimalPrecision { get; set; }
        public string MasterDriftStatus { get; set; } = "UNCHANGED"; // UNCHANGED, CHANGED, MISSING, INVALID
        public string? DriftCategory { get; set; } // Presentation, Operational, Scientific, Compliance
    }

    public class EffectiveRequirementRowDto
    {
        public long ParameterID { get; set; }
        public string ParameterCode { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
        public string RequirementType { get; set; } = string.Empty;
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public decimal? MinTolerance { get; set; }
        public decimal? MaxTolerance { get; set; }
        public string? AcceptanceCriteria { get; set; }
        public string? Equation { get; set; }
        public string? MinEquation { get; set; }
        public string? MaxEquation { get; set; }
        public string? ConditionContext { get; set; }
        public string Source { get; set; } = string.Empty;
        public long? SpecificationLineID { get; set; }
        public string ResolutionStatus { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class EffectiveConditionRowDto
    {
        public long ConditionMasterID { get; set; }
        public string ConditionCode { get; set; } = string.Empty;
        public string ConditionName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public string Operator { get; set; } = string.Empty;
        public string ConfiguredValue { get; set; } = string.Empty;
        public string RequirementContext { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public bool HasConfiguration { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class EffectiveMethodDto
    {
        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodCode { get; set; }
        public string? TestMethodName { get; set; }
        public string? StandardReference { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public string? Version { get; set; }
        public string? Year { get; set; }
        public string? Status { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? SupersededDate { get; set; }
        public bool IsSuperseded { get; set; }
        public bool IsDefault { get; set; }
        public string Source { get; set; } = string.Empty;
    }

    public class EffectiveEquipmentRowDto
    {
        public long? EquipmentID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? EquipmentType { get; set; }
        public long? EquipmentTypeID { get; set; }
        public string? EquipmentTypeName { get; set; }
        public string? Model { get; set; }
        public bool IsRequired { get; set; }
        public bool IsMandatory { get; set; }
        public long? EquipmentRequirementID { get; set; }
        public string? RequirementCode { get; set; }
        public string RequirementName { get; set; } = string.Empty;
        public int MatchingEquipmentCount { get; set; }
        public int AvailableEquipmentCount { get; set; }
        public string ReadinessStatus { get; set; } = "READY"; // READY, WARNING, BLOCKED
        public string? BlockingReason { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CalibrationStatus { get; set; } = string.Empty;
        public DateTime? CalibratedOn { get; set; }
        public DateTime? ValidUpto { get; set; }
        public string? CalibrationNo { get; set; }
    }

    public class EffectiveFactorRowDto
    {
        public long FactorConversionID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FactorType { get; set; } = string.Empty;
        public decimal FactorValue { get; set; }
        public long InputParameterID { get; set; }
        public string? InputParameterCode { get; set; }
        public string? InputParameterName { get; set; }
        public long? OutputParameterID { get; set; }
        public string? OutputParameterCode { get; set; }
        public string? OutputParameterName { get; set; }
        public string? AppliedOn { get; set; }
        public bool IsMandatory { get; set; }
        public string ScopeLevel { get; set; } = "Global"; // Version, Method, Test, Global
        public string Status { get; set; } = "CONFIGURED";
    }

    public class EffectiveUncertaintyDto
    {
        public bool IsConfigured { get; set; }
        public long? MeasurementUncertaintyMasterID { get; set; }
        public string? MasterCode { get; set; }
        public string? MasterName { get; set; }
        public string? UncertaintyType { get; set; }
        public decimal? CombinedUncertainty { get; set; }
        public decimal? ExpandedUncertainty { get; set; }
        public decimal? Value { get; set; }
        public string? Unit { get; set; }
        public decimal? CoverageFactor { get; set; }
        public decimal? ConfidenceLevel { get; set; }
        public string? Basis { get; set; }
        public string? ComponentsJson { get; set; }
        public string? Remarks { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class EffectiveAcceptanceCriteriaDto
    {
        public bool IsConfigured { get; set; }
        public long? AcceptanceCriteriaID { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? EvaluationType { get; set; }
        public string? ComparisonType { get; set; }
        public string? DecisionRule { get; set; }
        public string? RoundingRule { get; set; }
        public decimal? RoundingPrecision { get; set; }
        public string ResolutionSource { get; set; } = string.Empty;
        public string Status { get; set; } = "RESOLVED";
    }

    public class EffectiveLayoutDto
    {
        public long? PlannedExecutionLayoutID { get; set; }
        public string? PlannedLayoutCode { get; set; }
        public string? PlannedLayoutName { get; set; }
        public string? PlannedRendererType { get; set; }

        public long? EffectiveExecutionLayoutID { get; set; }
        public string? EffectiveLayoutCode { get; set; }
        public string? EffectiveLayoutName { get; set; }
        public string? EffectiveRendererType { get; set; }
        public string LayoutResolutionLevel { get; set; } = "Unassigned";
        public string ResolutionStatus { get; set; } = "UNCHANGED"; // UNCHANGED, LAYOUT_DRIFT, UNASSIGNED
        public string? Reason { get; set; }
        public ExecutionLayoutDto? ExecutionLayout { get; set; }
        public List<long> UnmappedParameterIDs { get; set; } = new();
    }

    public class ValidationSummaryDto
    {
        public string OverallStatus { get; set; } = "READY"; // READY, WARNING, BLOCKED
        public int BlockingCount { get; set; }
        public int WarningCount { get; set; }
        public bool AllPassed { get; set; }
        public List<ValidationItemDto> Items { get; set; } = new();
        public List<string> BlockingErrors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    public class ValidationItemDto
    {
        public string Check { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // PASS, WARNING, BLOCKED, N/A
        public string Source { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
}
