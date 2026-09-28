using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class SpecificationMasterListDto
    {
        public long ID { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? StandardReference { get; set; }
        public long? StandardOrganizationID { get; set; }
        public string? StandardOrganizationName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int VersionCount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class SpecificationMasterDetailDto
    {
        public long ID { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? StandardReference { get; set; }
        public long? StandardOrganizationID { get; set; }
        public string? StandardOrganizationName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int VersionCount { get; set; }
        public string? ActiveVersionName { get; set; }
        public long? ActiveVersionID { get; set; }
        public int DraftVersionCount { get; set; }
        public int SupersededVersionCount { get; set; }
        public int GradeCount { get; set; }
        public int ActiveRequirementCount { get; set; }
        public long CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CompanyCode { get; set; } = "LIMS";
    }

    public class SpecificationMasterCreateDto
    {
        [Required(ErrorMessage = "Specification code is required.")]
        [StringLength(100, ErrorMessage = "Specification code cannot exceed 100 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Specification name is required.")]
        [StringLength(100, ErrorMessage = "Specification name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Standard / Reference cannot exceed 300 characters.")]
        public string? StandardReference { get; set; }

        public long? StandardOrganizationID { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class SpecificationMasterUpdateDto
    {
        [Required(ErrorMessage = "Specification ID is required.")]
        public long ID { get; set; }

        [Required(ErrorMessage = "Specification code is required.")]
        [StringLength(100, ErrorMessage = "Specification code cannot exceed 100 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Specification name is required.")]
        [StringLength(100, ErrorMessage = "Specification name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Standard / Reference cannot exceed 300 characters.")]
        public string? StandardReference { get; set; }

        public long? StandardOrganizationID { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class SpecificationFilterDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? StandardReference { get; set; }
        public long? StandardOrganizationID { get; set; }
        public string? Status { get; set; } // "all", "active", "inactive"
    }

    public class SpecificationGradeDto
    {
        public long ID { get; set; }
        public long SpecificationHeaderID { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public string? IdentifierValuesJson { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public int RequirementCount { get; set; }
    }

    public class SpecificationGradeCreateDto
    {
        [Required(ErrorMessage = "Grade name is required.")]
        [StringLength(100, ErrorMessage = "Grade name cannot exceed 100 characters.")]
        public string Grade { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string? Remarks { get; set; }

        public string? IdentifierValuesJson { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class SpecificationGradeUpdateDto
    {
        [Required(ErrorMessage = "Grade ID is required.")]
        public long ID { get; set; }

        [Required(ErrorMessage = "Grade name is required.")]
        [StringLength(100, ErrorMessage = "Grade name cannot exceed 100 characters.")]
        public string Grade { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string? Remarks { get; set; }

        public string? IdentifierValuesJson { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
