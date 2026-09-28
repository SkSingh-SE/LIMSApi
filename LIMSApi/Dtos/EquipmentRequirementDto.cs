using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class EquipmentRequirementListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortByColumn { get; set; }
        public string? SortOrder { get; set; }
        public long? LaboratoryTestID { get; set; }
        public long? TestMethodSpecificationID { get; set; }
        public long? EquipmentTypeID { get; set; }
        public string Status { get; set; } = "active";
    }

    public class EquipmentRequirementListItemDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestName { get; set; } = string.Empty;
        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodName { get; set; }
        public long? ParameterID { get; set; }
        public string? ParameterName { get; set; }
        public long EquipmentTypeID { get; set; }
        public string EquipmentTypeName { get; set; } = string.Empty;
        public long? EquipmentID { get; set; }
        public string? EquipmentName { get; set; }
        public string? RequiredCapability { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string ModifiedByName { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
    }

    public class EquipmentRequirementDetailDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long LaboratoryTestID { get; set; }
        public string LaboratoryTestName { get; set; } = string.Empty;
        public long? TestMethodSpecificationID { get; set; }
        public string? TestMethodName { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }
        public string? TestMethodVersion { get; set; }
        public long? ParameterID { get; set; }
        public string? ParameterName { get; set; }
        public string? ParameterUnit { get; set; }
        public long EquipmentTypeID { get; set; }
        public string EquipmentTypeName { get; set; } = string.Empty;
        public long? EquipmentID { get; set; }
        public string? EquipmentName { get; set; }
        public string? RequiredCapability { get; set; }
        public decimal? MinimumRange { get; set; }
        public decimal? MaximumRange { get; set; }
        public long? RangeUnitID { get; set; }
        public string? RangeUnitName { get; set; }
        public string? AccuracyRequirement { get; set; }
        public string? ResolutionRequirement { get; set; }
        public bool IsMandatory { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string ModifiedByName { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
    }

    public class EquipmentRequirementCreateDto
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
        public long LaboratoryTestID { get; set; }

        public long? TestMethodSpecificationID { get; set; }

        public long? TestMethodSpecificationVersionID { get; set; }

        public long? ParameterID { get; set; }

        [Required]
        public long EquipmentTypeID { get; set; }

        public long? EquipmentID { get; set; }

        [StringLength(500)]
        public string? RequiredCapability { get; set; }

        public decimal? MinimumRange { get; set; }

        public decimal? MaximumRange { get; set; }

        public long? RangeUnitID { get; set; }

        [StringLength(200)]
        public string? AccuracyRequirement { get; set; }

        [StringLength(200)]
        public string? ResolutionRequirement { get; set; }

        public bool IsMandatory { get; set; } = true;

        public int DisplayOrder { get; set; } = 0;
    }

    public class EquipmentRequirementUpdateDto : EquipmentRequirementCreateDto
    {
        [Required]
        public long ID { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class EligibleEquipmentDto
    {
        public long EquipmentID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EquipmentNo { get; set; } = string.Empty;
        public string? ModelNo { get; set; }
        public long BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public long EquipmentTypeID { get; set; }
        public string EquipmentTypeName { get; set; } = string.Empty;
        public bool CalibrationRequired { get; set; }
        public string CalibrationStatus { get; set; } = string.Empty;
        public DateTime? LastCalibrationDate { get; set; }
        public DateTime? NextCalibrationDueDate { get; set; }
        public string? CalibrationCertificate { get; set; }
        public bool IsSelectable { get; set; }
        public string? BlockReason { get; set; }
    }
}
