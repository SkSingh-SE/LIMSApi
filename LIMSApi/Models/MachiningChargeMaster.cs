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

        public int SpecimenQuantity { get; set; } = 1;

        public bool PreparationRequired { get; set; } = true;

        public bool CuttingRequired { get; set; } = true;

        public bool MachiningRequired { get; set; } = true;

        public bool OtherPreparation { get; set; } = false;

        public long? MetalClassificationID { get; set; }

        public long? ProductMasterID { get; set; }

        [MaxLength(500)]
        public string? CuttingInstructions { get; set; }

        [MaxLength(500)]
        public string? MachiningInstructions { get; set; }

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

        [ForeignKey("MetalClassificationID")]
        public virtual MetalClassificationMaster? MetalClassification { get; set; }

        [ForeignKey("ProductMasterID")]
        public virtual ProductMaster? ProductMaster { get; set; }

        [NotMapped]
        public IFormFile? file { get; set; }
    }
}
