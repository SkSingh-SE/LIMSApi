using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LIMSApi.Helpers.Enums;

namespace LIMSApi.Models
{
    public class ReportFormatSection : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long ReportFormatID { get; set; }

        public ReportSectionType SectionType { get; set; }

        public int SortOrder { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? ConfigJson { get; set; }

        public bool IsVisible { get; set; } = true;

        [ForeignKey(nameof(ReportFormatID))]
        public ReportFormat? ReportFormat { get; set; }
    }
}
