using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class ReportTemplateBlock : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long ReportTemplateID { get; set; }

        [Required, MaxLength(100)]
        public string BlockType { get; set; } = string.Empty;
        // Header, KeyValueTable, ResultTable, Observation, ImageGallery, Statement, EndFooter

        public int SortOrder { get; set; }

        [Required]
        public string ConfigJson { get; set; } = string.Empty;
        // mapping + layout rules


        [MaxLength(200)]
        public string? ConditionExpression { get; set; }
        // example: "TestCode == 'TENSILE'"

        [ForeignKey(nameof(ReportTemplateID))]
        public ReportTemplate? Template { get; set; }
    }

}
