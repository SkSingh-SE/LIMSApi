using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class MachiningChargeMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long LaboratoryTestID { get; set; }

        public long TestMethodStandardID { get; set; }

        [MaxLength(200)]
        public string? SpecimenRawMaterialSize { get; set; }  // e.g. "Ø8mm × 120mm" — nullable

        [Required, MaxLength(300)]
        public string SpecimenSize { get; set; } = string.Empty;  // e.g. "A370-6.25mm-R5-M8"

        // NABL: dimensional drawing — uploaded via the central FileUpload flow (IFileUploadService)
        public long? UploadReferenceID { get; set; }

        [MaxLength(500)]
        public string? DrawingFilePath { get; set; }

        [MaxLength(255)]
        public string? FileName { get; set; }

        [MaxLength(500)]
        public string? Remark { get; set; }

        // Year-wise prices (General/Hard metal). One row per financial year that has its own price.
        public virtual ICollection<MachiningChargeVersion> Versions { get; set; } = new List<MachiningChargeVersion>();

        [NotMapped]
        public IFormFile? file { get; set; }
    }
}
