using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class SamplePreparationMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long? TestMethodStandardID { get; set; }

        public long? LaboratoryTestID { get; set; }  // FK to LaboratoryTest (SubGroup)

        [Required, MaxLength(200)]
        public string SpecimenType { get; set; } = string.Empty;  // "Round Bar", "Flat Bar", "Pipe", etc.

        [MaxLength(500)]
        public string? Dimensions { get; set; }  // "ø5mm & R3.75mm", "W12.5mm & R20mm"

        [MaxLength(200)]
        public string? MaterialType { get; set; }  // "Ferrous", "Non-Ferrous", "All"

        [Column(TypeName = "decimal(18,2)")]
        public decimal Charges { get; set; }  // Preparation charge amount

        [MaxLength(500)]
        public string? Remark { get; set; }

        [ForeignKey("TestMethodStandardID")]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }

        [ForeignKey("LaboratoryTestID")]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }
    }
}
