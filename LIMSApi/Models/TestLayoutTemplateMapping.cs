using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("TestLayoutTemplateMappings")]
    public class TestLayoutTemplateMapping : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long TestLayoutTemplateID { get; set; }

        [Required]
        public long LaboratoryTestID { get; set; }

        public long? LaboratoryTestSubGroupID { get; set; }

        public long? LaboratoryTestAnalysisTypeID { get; set; }

        [MaxLength(250)]
        public string? LeafNode { get; set; }

        public bool IsDefault { get; set; } = true;

        [System.Text.Json.Serialization.JsonIgnore]
        [ForeignKey(nameof(TestLayoutTemplateID))]
        public virtual TestLayoutTemplate? TestLayoutTemplate { get; set; }

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey(nameof(LaboratoryTestSubGroupID))]
        public virtual LaboratoryTestSubGroup? SubGroup { get; set; }

        [ForeignKey(nameof(LaboratoryTestAnalysisTypeID))]
        public virtual LaboratoryTestAnalysisType? AnalysisType { get; set; }
    }
}
