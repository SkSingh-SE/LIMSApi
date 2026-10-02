using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class EquipmentCalibration
    {
        [Key]
        public long ID { get; set; }
        public long EquipmentID { get; set; }
        public DateTime CalibrationDate { get; set; }
        public DateTime CalibrationDueDate { get; set; }
        public string? Certificate { get; set; } = string.Empty;
        public string? CertificatePath { get; set; } = string.Empty;
        public long? UploadReferenceID { get; set; }
        public long? CalibrationAgencyID { get; set; }
        public string? Agency { get; set; }
        public bool IsReviewed { get; set; } = false;
        public string? ReviewReason { get; set; }
        public string? Desrciption { get; set; }
        public string? CalibrationCreateBy { get; set; }
        public DateTime? CalibrationCreateDate { get; set; }
        public string? ReviewerBy { get; set; }
        public DateTime? ReviewerDate { get; set; }


        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
