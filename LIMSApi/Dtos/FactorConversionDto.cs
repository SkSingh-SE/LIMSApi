using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class FactorConversionListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortByColumn { get; set; }
        public string? SortOrder { get; set; }
        public string? FactorType { get; set; }
        public long? InputParameterID { get; set; }
        public long? OutputParameterID { get; set; }
        public string Status { get; set; } = "active";
    }

    public class FactorConversionListItemDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FactorType { get; set; } = string.Empty;
        public decimal FactorValue { get; set; }
        public long InputParameterID { get; set; }
        public string InputParameterName { get; set; } = string.Empty;
        public long? OutputParameterID { get; set; }
        public string? OutputParameterName { get; set; }
        public string TransformationText { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string ModifiedByName { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
    }

    public class FactorConversionDetailDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FactorType { get; set; } = string.Empty;
        public decimal FactorValue { get; set; }
        public long InputParameterID { get; set; }
        public string InputParameterName { get; set; } = string.Empty;
        public string? InputParameterUnit { get; set; }
        public long? OutputParameterID { get; set; }
        public string? OutputParameterName { get; set; }
        public string? OutputParameterUnit { get; set; }
        public long? LaboratoryTestID { get; set; }
        public string? LaboratoryTestName { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodName { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public string? TestMethodVersion { get; set; }
        public string AppliedOn { get; set; } = string.Empty;
        public string TransformationText { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string ModifiedByName { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
    }

    public class FactorConversionCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(20)]
        public string FactorType { get; set; } = "MULTIPLICATION";

        public decimal FactorValue { get; set; } = 1.0m;

        [Required]
        public long InputParameterID { get; set; }

        public long? OutputParameterID { get; set; }

        public long? LaboratoryTestID { get; set; }

        public long? TestMethodSpecificationID { get; set; }

        public long? TestMethodSpecificationVersionID { get; set; }

        [StringLength(100)]
        public string AppliedOn { get; set; } = "Measured Value";

        public bool IsMandatory { get; set; } = true;

        public int DisplayOrder { get; set; } = 0;
    }

    public class FactorConversionUpdateDto : FactorConversionCreateDto
    {
        [Required]
        public long ID { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class FactorValidateRequest
    {
        public long? FactorID { get; set; }

        [StringLength(20)]
        public string? FactorType { get; set; }

        public decimal? FactorValue { get; set; }

        public decimal InputValue { get; set; }
    }

    public class FactorValidateResponse
    {
        public string FactorType { get; set; } = string.Empty;
        public decimal FactorValue { get; set; }
        public decimal InputValue { get; set; }
        public decimal OutputValue { get; set; }
        public string AppliedOperation { get; set; } = string.Empty;
    }
}
