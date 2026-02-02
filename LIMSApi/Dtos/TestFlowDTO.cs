namespace LIMSApi.Dtos
{
    public class TestFlowDTO
    {
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

}
