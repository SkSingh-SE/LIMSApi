using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTest : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        public long? LabDepartmentID { get; set; }

        public string? Equation { get; set; }

        // True = chemical analysis test (drives Sub-Group -> AnalysisType hierarchy).
        public bool IsChemicalTest { get; set; } = false;

        // True = mechanical testing (tensile, impact, hardness, etc.).
        public bool IsMechanical { get; set; } = false;

        [Range(1, 365)]
        public int? TestDuration { get; set; }

        public int GlobalUsageCount { get; set; }
        public int RecentUsageCount { get; set; }
        public DateTime? LastPerformedDate { get; set; }

        [ForeignKey("LabDepartmentID")]
        public virtual DepartmentMaster? LabDepartment { get; set; }

        public virtual ICollection<LaboratoryTestSubGroup> SubGroups { get; set; } = new List<LaboratoryTestSubGroup>();
    }
}
