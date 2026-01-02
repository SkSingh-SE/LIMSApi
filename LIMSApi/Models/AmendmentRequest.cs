using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class AmendmentRequest : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long ReportHeaderID { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";
        // Pending | Approved | Rejected

        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long? UploadReferenceID { get; set; }

        [ForeignKey(nameof(ReportHeaderID))]
        public ReportHeader? ReportHeader { get; set; }
    }
}
