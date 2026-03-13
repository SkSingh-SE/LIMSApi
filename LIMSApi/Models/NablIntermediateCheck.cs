using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablIntermediateChecks")]
    public class NablIntermediateCheck : NablFormBase
    {
        public long? EquipmentId { get; set; }

        [ForeignKey("EquipmentId")]
        public virtual EquipmentMaster? Equipment { get; set; }

        [MaxLength(100)]
        public string? EquipmentCode { get; set; }

        public DateTime? CheckDate { get; set; }

        [MaxLength(200)]
        public string? CheckMethod { get; set; }

        [MaxLength(200)]
        public string? ReferenceStandard { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? ObservedValue { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? AcceptedValue { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? Tolerance { get; set; }

        [MaxLength(50)]
        public string? ResultStatus { get; set; } // Pass/Fail

        [MaxLength(200)]
        public string? CheckedBy { get; set; }

        [MaxLength(500)]
        public string? CorrectiveAction { get; set; }
    }
}
