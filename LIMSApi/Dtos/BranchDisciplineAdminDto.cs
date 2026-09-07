using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Dtos
{
    public class BranchDisciplineItemDto
    {
        public long Id { get; set; }
        public long BranchID { get; set; }
        public long DisciplineID { get; set; }
        public string DisciplineName { get; set; } = string.Empty;
        public string DisciplineCode { get; set; } = string.Empty;
        public string? DisciplineDescription { get; set; }
        public bool IsAccredited { get; set; }
        public bool IsActive { get; set; }
    }

    public class AvailableDisciplineItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class BranchDisciplinesResponseDto
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public List<BranchDisciplineItemDto> AssignedDisciplines { get; set; } = new();
        public List<AvailableDisciplineItemDto> AvailableDisciplines { get; set; } = new();
    }

    public class BranchDisciplineAssignDto
    {
        [Required]
        public long DisciplineID { get; set; }

        public bool IsAccredited { get; set; } = false;
    }

    public class BranchDisciplineAccreditationDto
    {
        public bool IsAccredited { get; set; }
    }
}
