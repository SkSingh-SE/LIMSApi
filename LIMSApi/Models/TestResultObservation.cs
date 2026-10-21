using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("TestResultObservations")]
    public class TestResultObservation : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long TestResultHeaderID { get; set; }

        [Required]
        [MaxLength(50)]
        public string ObservationCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = "Remarks";

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string ObservationText { get; set; } = string.Empty;

        public bool IsRuntimeAdded { get; set; } = false;

        [MaxLength(500)]
        public string? AddedReason { get; set; }

        public bool IsReportable { get; set; } = true;

        public bool IsVoided { get; set; } = false;

        [MaxLength(500)]
        public string? VoidReason { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [ForeignKey(nameof(TestResultHeaderID))]
        public virtual TestResultHeader? TestResultHeader { get; set; }
    }
}
