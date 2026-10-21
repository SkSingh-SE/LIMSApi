using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("TestResultReadings")]
    public class TestResultReading : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long TestResultHeaderID { get; set; }

        public long? TestResultParameterID { get; set; }

        [MaxLength(50)]
        public string ParameterCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string SpecimenCode { get; set; } = "S1";

        [MaxLength(50)]
        public string? LocationCode { get; set; }

        [Required]
        public int SequenceNo { get; set; } = 1;

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Value { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        public bool IsVoided { get; set; } = false;

        [MaxLength(500)]
        public string? VoidReason { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [ForeignKey(nameof(TestResultHeaderID))]
        public virtual TestResultHeader? TestResultHeader { get; set; }

        [ForeignKey(nameof(TestResultParameterID))]
        public virtual TestResultParameter? TestResultParameter { get; set; }
    }
}
