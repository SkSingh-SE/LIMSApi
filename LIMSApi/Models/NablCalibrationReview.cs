using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablCalibrationReviews")]
    public class NablCalibrationReview : NablFormBase
    {
        public long? EquipmentId { get; set; }

        [ForeignKey("EquipmentId")]
        public virtual EquipmentMaster? Equipment { get; set; }

        [MaxLength(100)]
        public string? EquipmentCode { get; set; }

        [MaxLength(200)]
        public string? EquipmentName { get; set; }

        public DateTime? CalibrationDate { get; set; }

        [MaxLength(200)]
        public string? CalibrationAgencyName { get; set; }

        [MaxLength(100)]
        public string? CertificateNo { get; set; }

        public DateTime? CalibrationDueDate { get; set; }

        [MaxLength(50)]
        public string? CalibrationResult { get; set; } // Pass/Fail/ConditionalPass

        public string? CalibrationDataJson { get; set; } // JSON

        [MaxLength(200)]
        public string? ReviewedByName { get; set; }

        [MaxLength(50)]
        public string? ReviewConclusion { get; set; } // AcceptForUse/NotAcceptable

        [MaxLength(500)]
        public string? CorrectiveAction { get; set; }
    }
}
