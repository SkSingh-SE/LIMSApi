using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class ReportFormat : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, MaxLength(50)]
        public string FormatCode { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string FormatName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(20)]
        public string PageLayout { get; set; } = "Portrait"; // Portrait | Landscape

        [MaxLength(20)]
        public string PageSize { get; set; } = "A4"; // A4 | Letter

        [Column(TypeName = "nvarchar(max)")]
        public string? HeaderConfigJson { get; set; }

        public bool IsDefault { get; set; } = false;
        public bool IsLocked { get; set; } = false;

        public ICollection<ReportFormatSection> Sections { get; set; } = new List<ReportFormatSection>();
        public ICollection<ReportFormatMapping> Mappings { get; set; } = new List<ReportFormatMapping>();
    }
}
