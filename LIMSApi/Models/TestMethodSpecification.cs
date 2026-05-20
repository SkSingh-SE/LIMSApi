using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class TestMethodSpecification : AuditProperty // Test Method Standard
    {
        [Key]
        public long ID { get; set; }
        public long StandardOrganizationID { get; set; }
        [MaxLength(255)]
        [Required]
        public string TestMethodStandard { get; set; } = string.Empty;
        [MaxLength(500)]
        [Required]
        public required string Name { get; set; }
        public string? Part { get; set; }
        public bool IsDisabled { get; set; } = false;

        // Phase 7.3: Test method specification enhancements
        [MaxLength(500)]
        public string? LinkedStandard { get; set; }

        [MaxLength(1000)]
        public string? FormulaExpression { get; set; }

        public string? DefaultParameters { get; set; }

        public ICollection<TestMethodSpecificationVersion> Versions { get; set; } = new List<TestMethodSpecificationVersion>();

    }
}
