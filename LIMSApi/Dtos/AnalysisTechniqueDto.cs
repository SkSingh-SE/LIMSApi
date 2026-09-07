using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class AnalysisTechniqueListItemDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? AliasNames { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
    }

    public class AnalysisTechniqueDetailDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? AliasNames { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class AnalysisTechniqueCreateDto
    {
        [Required(ErrorMessage = "Technique Code is required.")]
        [StringLength(50, ErrorMessage = "Technique Code cannot exceed 50 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Technique Name is required.")]
        [StringLength(100, ErrorMessage = "Technique Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Alias names cannot exceed 500 characters.")]
        public string? AliasNames { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class AnalysisTechniqueUpdateDto
    {
        [Required(ErrorMessage = "Technique ID is required.")]
        public long ID { get; set; }

        [Required(ErrorMessage = "Technique Code is required.")]
        [StringLength(50, ErrorMessage = "Technique Code cannot exceed 50 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Technique Name is required.")]
        [StringLength(100, ErrorMessage = "Technique Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Alias names cannot exceed 500 characters.")]
        public string? AliasNames { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
