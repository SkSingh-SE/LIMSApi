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

        // Auto-generated caption shown in dropdowns/lists: "{StdOrg} {TestMethodStandard} {Part} : {ActiveVersion}"
        [MaxLength(600)]
        public string? DisplayTitle { get; set; }

        public bool IsDisabled { get; set; } = false;

        // The version treated as default/active — auto-selected in inward/plan dropdowns.
        public long? DefaultVersionID { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("DefaultVersionID")]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual TestMethodSpecificationVersion? DefaultVersion { get; set; }

        // Phase 7.3: Test method specification enhancements
        [MaxLength(500)]
        public string? LinkedStandard { get; set; }

        [MaxLength(1000)]
        public string? FormulaExpression { get; set; }

        public string? DefaultParameters { get; set; }

        public ICollection<TestMethodSpecificationVersion> Versions { get; set; } = new List<TestMethodSpecificationVersion>();

        // Metal classifications this specification applies to (narrows test-method selection in inward).
        public ICollection<TestMethodSpecificationMetalClassification> MetalClassifications { get; set; } = new List<TestMethodSpecificationMetalClassification>();

    }
}
