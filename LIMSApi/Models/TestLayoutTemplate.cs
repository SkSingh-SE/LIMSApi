using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("TestLayoutTemplates")]
    public class TestLayoutTemplate : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, MaxLength(100)]
        public string LayoutCode { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string LayoutName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Revision { get; set; } = "1.0";

        public int Version { get; set; } = 1;

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Published"; // "Draft", "Published", "Archived"

        public bool IsDefault { get; set; } = true;

        public bool IsGlobalDefault { get; set; } = false;

        // ── 5-Tier Target Scoping & Leaf-Node Hierarchy ──
        public long? LaboratoryTestID { get; set; }

        public long? LaboratoryTestSubGroupID { get; set; }

        public long? LaboratoryTestAnalysisTypeID { get; set; }

        public long? TestMethodSpecificationID { get; set; }

        [MaxLength(250)]
        public string? TestMethodName { get; set; }

        [MaxLength(250)]
        public string? LeafNode { get; set; }

        public bool ApplyToLeaf { get; set; } = true;

        // ── Summary Metadata ──
        public int SectionCount { get; set; } = 0;

        [MaxLength(500)]
        public string? ArchetypeSummary { get; set; }

        // ── Dynamic Structural Payload ──
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string LayoutConfigJson { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? OutputPayloadSchemaJson { get; set; }

        // ── Navigation Properties ──
        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey(nameof(LaboratoryTestSubGroupID))]
        public virtual LaboratoryTestSubGroup? SubGroup { get; set; }

        [ForeignKey(nameof(LaboratoryTestAnalysisTypeID))]
        public virtual LaboratoryTestAnalysisType? AnalysisType { get; set; }

        public virtual ICollection<TestLayoutTemplateMapping> Mappings { get; set; } = new List<TestLayoutTemplateMapping>();

        [NotMapped]
        public List<long>? MappedTestIDs { get; set; }

        [NotMapped]
        public List<TestLayoutTemplateMappingItemDto>? MappedLeafNodes { get; set; }
    }

    public class TestLayoutTemplateMappingItemDto
    {
        public long LaboratoryTestID { get; set; }
        public long? LaboratoryTestSubGroupID { get; set; }
        public long? LaboratoryTestAnalysisTypeID { get; set; }
        public string? LeafNode { get; set; }
        public string? UniqueKey { get; set; }
    }
}
