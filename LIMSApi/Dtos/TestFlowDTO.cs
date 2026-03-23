namespace LIMSApi.Dtos
{
    public class TestFlowDTO
    {
    }

    // ----- Orientation Mismatch Warning -----
    public class OrientationMismatchWarningDto
    {
        public long ParameterId { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public string RequiredOrientation { get; set; } = string.Empty;
        public string ActualOrientation { get; set; } = string.Empty;
        public long? SpecificationLineId { get; set; }
    }

    public class OrientationCheckResultDto
    {
        public bool HasMismatches { get; set; }
        public List<OrientationMismatchWarningDto> Warnings { get; set; } = new();
    }
    public class TestResultSaveDto
    {
        public long InwardId { get; set; }
        public long SampleId { get; set; }
        public long PlanId { get; set; }

        public List<TestResultGroupDto> GeneralTests { get; set; } = new();
        public List<TestResultGroupDto> ChemicalTests { get; set; } = new();
    }
    public class TestResultGroupDto
    {
        public long HeaderId { get; set; }          // Existing TestResultHeader ID
        public long GeneralTestId { get; set; }     // GeneralTest ID
        public long TestMethodId { get; set; }      // General/Chemical Method ID
        public long LaboratoryTestId { get; set; }  // LaboratoryTests table ID

        public List<TestResultParameterDto> Parameters { get; set; } = new();
    }
    public class TestResultParameterDto
    {
        public long Id { get; set; }                 // TestResultParameter.ID
        public long ParameterID { get; set; }        // Master Parameter ID

        public string ParameterName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        // Value entered or calculated
        public decimal? Value { get; set; }

        public string? Remarks { get; set; }

        // Specification limits
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }

        // Evaluation flags
        public bool? IsWithinLimit { get; set; } = false;     // true / false based on min-max
        public bool Altered { get; set; } = false;   // edited after initial entry

        // Formula / calculation metadata
        public string? Formula { get; set; }
        public bool IsCalculated { get; set; }

        // Extra / optional parameters
        public bool IsAdditional { get; set; }
        public long? SpecificationLineID { get; set; }

        // Billable flag (observed parameters excluded from pricing)
        public bool IsBillable { get; set; } = true;
    }

    public class MoveToLongTermDto
    {
        public long HeaderId { get; set; }
        public int DurationHours { get; set; }
    }
    public class LongTermRecordDto
    {
        public long LongTermTestId { get; set; }
        public long ParameterId { get; set; }
        public decimal Value { get; set; }
        public string? Remarks { get; set; }
    }

    public class TestResultImageDto
    {
        public long Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? Caption { get; set; }
    }
    public class LongTermParsedValue
    {
        public int ParameterId { get; set; }
        public string ParameterName { get; set; }
        public decimal? Value { get; set; }
        public string Remarks { get; set; }
        public DateTime RecordedAt { get; set; }
    }

    public class AddStandaloneParameterDto
    {
        public string ParameterName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string? FormulaExpression { get; set; }
        public decimal? SpecMinValue { get; set; }
        public decimal? SpecMaxValue { get; set; }
        public string? AcceptanceCriteria { get; set; }
    }

    public class AddParameterFromMethodDto
    {
        public long SourceTestMethodId { get; set; }
        public long ParameterID { get; set; }
    }

    public class EnvironmentAtTimeDto
    {
        public long HeaderId { get; set; }
        public long? LabRoomId { get; set; }
        public decimal? RoomTemperature { get; set; }
        public decimal? RoomHumidity { get; set; }
        public string? EquipmentIdsJson { get; set; }
        public DateTime? TestStartTime { get; set; }
        public DateTime? TestEndTime { get; set; }
        public string? PerformedByName { get; set; }
        public long? PerformedById { get; set; }
    }

    public class CalculateParametersResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ParametersRecalculated { get; set; }
        public bool? IsOverallPass { get; set; }
        public List<ParameterCalculationResultDto> Parameters { get; set; } = new();
    }

    public class ParameterCalculationResultDto
    {
        public long Id { get; set; }
        public long ParameterID { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public decimal? Value { get; set; }
        public bool IsCalculated { get; set; }
        public string? ResultStatus { get; set; }
        public bool? IsWithinLimit { get; set; }
    }

    // StartTest response with optional preparation warning
    public class StartTestResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Test started";
        public bool PreparationWarning { get; set; }
        public string? PreparationStatus { get; set; }
        public string? WarningMessage { get; set; }
        public DateTime? TestStartTime { get; set; }
        public string? PerformedByName { get; set; }
    }

    public class CompleteTestResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Test completed";
        public DateTime? TestEndTime { get; set; }
        public string? PerformedByName { get; set; }
    }

    // Auto-create headers response with warnings
    public class AutoCreateHeadersResponse
    {
        public bool Success { get; set; } = true;
        public int HeadersCreated { get; set; }
        public List<string> Warnings { get; set; } = new();
    }

    // Phase 5: Verification DTOs
    public class VerificationActionDto
    {
        public string? Comments { get; set; }
    }

    public class PreparationStatusDto
    {
        public bool PreparationRequired { get; set; }
        public bool PreparationRecorded { get; set; }
        public string? PreparationStatus { get; set; }
        public decimal CuttingCharges { get; set; }
        public decimal MachiningCharges { get; set; }
        public List<MachiningChargeLineDto> MachiningBreakdown { get; set; } = new();
        public List<object> Specimens { get; set; } = new();
        public string? CuttingEditUrl { get; set; }
        public string? MachiningEditUrl { get; set; }
    }

    // Phase 6: Unified Price Summary DTOs
    public class UnifiedPriceSummaryDto
    {
        public long SampleId { get; set; }
        public long InwardId { get; set; }
        public decimal CuttingCharges { get; set; }
        public List<CuttingChargeLineDto> CuttingBreakdown { get; set; } = new();
        public decimal MachiningCharges { get; set; }
        public List<MachiningChargeLineDto> MachiningBreakdown { get; set; } = new();
        public decimal OtherPreparationCharges { get; set; }
        public decimal TotalPreparationCharges { get; set; }
        public List<TestChargeLineDto> TestCharges { get; set; } = new();
        public decimal TotalTestCharges { get; set; }
        public decimal GrandTotal { get; set; }
        public string? BillingStatus { get; set; }
    }

    public class CuttingChargeLineDto
    {
        public string CuttingType { get; set; } = string.Empty;
        public string UnitType { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }

    public class TestChargeLineDto
    {
        public long HeaderId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public decimal CalculatedPrice { get; set; }
        public decimal? OverridePrice { get; set; }
        public decimal FinalPrice { get; set; }
        public bool IsOverridden { get; set; }
    }

    // Machining Charge DTOs
    public class MachiningChargeItemDto
    {
        public long Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Remark { get; set; }
    }

    public class MachiningChargeLineDto
    {
        public long Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Remark { get; set; }
    }

}
