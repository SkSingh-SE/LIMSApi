using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class MeasurementUncertaintyListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortByColumn { get; set; }
        public string? SortOrder { get; set; }
        public long? LaboratoryTestID { get; set; }
        public long? ParameterID { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public string? UncertaintyType { get; set; }
        public string? Basis { get; set; }
        public string Status { get; set; } = "active";
    }

    public class MeasurementUncertaintyListItemDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long? LaboratoryTestID { get; set; }
        public string? LaboratoryTestName { get; set; }
        public long? ParameterID { get; set; }
        public string? ParameterName { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodName { get; set; }
        public string UncertaintyType { get; set; } = string.Empty;
        public string Basis { get; set; } = string.Empty;
        public decimal? CombinedUncertainty { get; set; }
        public decimal? ExpandedUncertainty { get; set; }
        public decimal CoverageFactor { get; set; }
        public decimal? ConfidenceLevel { get; set; }
        public string? UnitSymbol { get; set; }
        public string SummaryText { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string ModifiedByName { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
    }

    public class MeasurementUncertaintyComponentDto
    {
        public string Source { get; set; } = string.Empty;
        public string EvalType { get; set; } = "Type B";
        public string? Distribution { get; set; }
        public decimal StdUncertainty { get; set; }
        public decimal SensitivityCoefficient { get; set; } = 1.0m;
        public decimal Contribution { get; set; }
    }

    public class MeasurementUncertaintyDetailDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? LaboratoryTestID { get; set; }
        public string? LaboratoryTestName { get; set; }
        public long? ParameterID { get; set; }
        public string? ParameterName { get; set; }
        public string? ParameterUnit { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodName { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public string? TestMethodVersion { get; set; }
        public long? ParameterUnitID { get; set; }
        public string? UnitSymbol { get; set; }
        public string UncertaintyType { get; set; } = string.Empty;
        public string Basis { get; set; } = string.Empty;
        public decimal? CombinedUncertainty { get; set; }
        public decimal? ExpandedUncertainty { get; set; }
        public decimal CoverageFactor { get; set; }
        public decimal? ConfidenceLevel { get; set; }
        public List<MeasurementUncertaintyComponentDto> Components { get; set; } = new();
        public string SummaryText { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string ModifiedByName { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
    }

    public class MeasurementUncertaintyCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public long? LaboratoryTestID { get; set; }
        public long? ParameterID { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public long? ParameterUnitID { get; set; }

        [Required]
        [StringLength(20)]
        public string UncertaintyType { get; set; } = "EXPANDED";

        [Required]
        [StringLength(20)]
        public string Basis { get; set; } = "Type B";

        public decimal? CombinedUncertainty { get; set; }
        public decimal? ExpandedUncertainty { get; set; }
        public decimal CoverageFactor { get; set; } = 2.0m;
        public decimal? ConfidenceLevel { get; set; }

        public List<MeasurementUncertaintyComponentDto>? Components { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    public class MeasurementUncertaintyUpdateDto : MeasurementUncertaintyCreateDto
    {
        [Required]
        public long ID { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class MeasurementUncertaintyValidateRequest
    {
        public decimal? CombinedUncertainty { get; set; }
        public decimal CoverageFactor { get; set; } = 2.0m;
        public List<MeasurementUncertaintyComponentDto>? Components { get; set; }
    }

    public class MeasurementUncertaintyValidateResponse
    {
        public decimal CombinedUncertainty { get; set; }
        public decimal CoverageFactor { get; set; }
        public decimal ExpandedUncertainty { get; set; }
        public string AppliedOperation { get; set; } = string.Empty;
        public List<MeasurementUncertaintyComponentDto> Components { get; set; } = new();
    }
}
