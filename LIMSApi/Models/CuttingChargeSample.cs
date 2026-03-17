using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class CuttingChargeSample
    {
        [Key]
        public long ID { get; set; }
        public long CuttingChargeHeaderID { get; set; }
        public long SampleID { get; set; }
        public string SampleNo { get; set; } = string.Empty;
        public long? MetalClassificationID { get; set; }

        public long? SpecimenTypeId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Length { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Width { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Thickness { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Diameter { get; set; }

        [MaxLength(50)]
        public string? Orientation { get; set; }

        [MaxLength(500)]
        public string? PreparationInstructions { get; set; }

        [MaxLength(50)]
        public string PreparationStatus { get; set; } = "Pending";

        [MaxLength(500)]
        public string? PhotoUrl { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SampleTotal { get; set; }

        [ForeignKey("CuttingChargeHeaderID"), JsonIgnore]
        public virtual CuttingChargeHeader? CuttingChargeHeader { get; set; }

        [ForeignKey("MetalClassificationID"), JsonIgnore]
        public virtual MetalClassificationMaster? MetalClassification { get; set; }

        [ForeignKey("SpecimenTypeId"), JsonIgnore]
        public virtual SpecimenTypeMaster? SpecimenType { get; set; }

        public ICollection<CuttingChargeDetail> CuttingChargeDetails { get; set; } = new List<CuttingChargeDetail>();
    }
}
