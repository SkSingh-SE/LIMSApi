using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class ReportBlock : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long ReportID { get; set; }

        [Required]
        [MaxLength(100)]
        public string BlockType { get; set; } = string.Empty;
        // Examples:
        // "Header", "TestInfo", "Table", "Image", "Statement", "Footer"

        public int SortOrder { get; set; }

        [Required]
        public string PayloadJson { get; set; } = string.Empty;

        [ForeignKey(nameof(ReportID))]
        public Report? Report { get; set; }
    }

}
