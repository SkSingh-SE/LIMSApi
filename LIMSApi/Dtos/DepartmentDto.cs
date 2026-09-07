using System;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class DepartmentListItemDto
    {
        public long ID { get; set; }
        public long BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string? BranchCode { get; set; }
        public long? DisciplineID { get; set; }
        public string? DisciplineName { get; set; }
        public string? DisciplineCode { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsChemical { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class DepartmentDetailDto
    {
        public long ID { get; set; }
        public long BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public long? DisciplineID { get; set; }
        public string? DisciplineName { get; set; }
        public string? DisciplineCode { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsChemical { get; set; }
        public bool IsActive { get; set; }
    }

    public class DepartmentCreateDto
    {
        [Required(ErrorMessage = "Branch is required.")]
        public long BranchID { get; set; }

        [Required(ErrorMessage = "Discipline is required.")]
        public long DisciplineID { get; set; }

        [Required(ErrorMessage = "Department Code is required.")]
        [StringLength(20, ErrorMessage = "Department Code cannot exceed 20 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department Name is required.")]
        [StringLength(100, ErrorMessage = "Department Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(3000, ErrorMessage = "Description cannot exceed 3000 characters.")]
        public string? Description { get; set; }

        public bool IsChemical { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }

    public class DepartmentUpdateDto
    {
        [Required(ErrorMessage = "Department ID is required.")]
        public long ID { get; set; }

        [Required(ErrorMessage = "Branch is required.")]
        public long BranchID { get; set; }

        [Required(ErrorMessage = "Discipline is required.")]
        public long DisciplineID { get; set; }

        [Required(ErrorMessage = "Department Code is required.")]
        [StringLength(20, ErrorMessage = "Department Code cannot exceed 20 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department Name is required.")]
        [StringLength(100, ErrorMessage = "Department Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(3000, ErrorMessage = "Description cannot exceed 3000 characters.")]
        public string? Description { get; set; }

        public bool IsChemical { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }
}
