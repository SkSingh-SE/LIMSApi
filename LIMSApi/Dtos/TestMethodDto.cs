using System;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class TestMethodListItemDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? StandardReference { get; set; }
        public long? AnalysisTechniqueID { get; set; }
        public string? AnalysisTechniqueName { get; set; }
        public string? AnalysisTechniqueCode { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class TestMethodDetailDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? StandardReference { get; set; }
        public long? AnalysisTechniqueID { get; set; }
        public string? AnalysisTechniqueName { get; set; }
        public string? AnalysisTechniqueCode { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
    }

    public class TestMethodCreateDto
    {
        [Required(ErrorMessage = "Test Method Code is required.")]
        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Test Method Name is required.")]
        [StringLength(500, ErrorMessage = "Name cannot exceed 500 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Standard / Reference cannot exceed 255 characters.")]
        public string? StandardReference { get; set; }

        public long? AnalysisTechniqueID { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }
    }

    public class TestMethodUpdateDto
    {
        [Required(ErrorMessage = "ID is required.")]
        public long ID { get; set; }

        [Required(ErrorMessage = "Test Method Code is required.")]
        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Test Method Name is required.")]
        [StringLength(500, ErrorMessage = "Name cannot exceed 500 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Standard / Reference cannot exceed 255 characters.")]
        public string? StandardReference { get; set; }

        public long? AnalysisTechniqueID { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }
    }

    public class TestMethodDropdownDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? StandardReference { get; set; }
        public long? AnalysisTechniqueID { get; set; }
        public string? AnalysisTechniqueCode { get; set; }
    }
}
