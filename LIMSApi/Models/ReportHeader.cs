using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class ReportHeader : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long SampleID { get; set; }
        public string ReportNo { get; set; } = string.Empty;

        public string Status { get; set; } = "Draft";

        public string GeneratedBy { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public string? SnapshotJson { get; set; }
        public string? PdfPath { get; set; }

        public long? WorkflowInstanceId { get; set; }

        // NEW — Taken from TestResultHeader
        public string? CertificateNo { get; set; }

        [ForeignKey(nameof(SampleID))]
        public SampleDetail? Sample { get; set; }

        public ICollection<Report> Reports { get; set; }
        = new List<Report>();
    }

}
