using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class Report : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long ReportHeaderID { get; set; }

        public string ReportNo { get; set; } = string.Empty;
        public int Version { get; set; } = 1;

        // Draft / Final / Superseded
        public string Status { get; set; } = "Draft";

        public string GeneratedBy { get; set; } = "";
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public string? SnapshotJson { get; set; }
        public string? PdfPath { get; set; }

        public string? CertificateNo { get; set; }

        [ForeignKey(nameof(ReportHeaderID))]
        public ReportHeader? ReportHeader { get; set; }

        public ICollection<ReportBlock> Blocks { get; set; }
            = new List<ReportBlock>();
    }

}