using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class GeneratedReport : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long ReportFormatID { get; set; }

        [Required]
        public long SampleID { get; set; }

        [Required, MaxLength(100)]
        public string ReportNo { get; set; } = string.Empty;

        [MaxLength(128)]
        public string? CertificateNo { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Generated"; // Generated | Reviewed | Approved

        public string? PdfPath { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? SnapshotJson { get; set; }

        [MaxLength(200)]
        public string GeneratedBy { get; set; } = string.Empty;

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ReportFormatID))]
        public ReportFormat? ReportFormat { get; set; }

        [ForeignKey(nameof(SampleID))]
        public SampleDetail? Sample { get; set; }
    }
}
