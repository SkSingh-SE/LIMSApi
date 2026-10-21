using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("TestResultTraversePoints")]
    public class TestResultTraversePoint : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long TestResultHeaderID { get; set; }

        [Required]
        [MaxLength(20)]
        public string SpecimenCode { get; set; } = "S1";

        public int SequenceNo { get; set; } = 1;

        [Column(TypeName = "decimal(10,4)")]
        public decimal Distance { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? HardnessValue { get; set; }

        [MaxLength(50)]
        public string HardnessUnit { get; set; } = "HV 0.5";

        public bool IsBelowCutoff { get; set; } = false;

        public bool IsVoided { get; set; } = false;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [ForeignKey(nameof(TestResultHeaderID))]
        public virtual TestResultHeader? TestResultHeader { get; set; }
    }
}
