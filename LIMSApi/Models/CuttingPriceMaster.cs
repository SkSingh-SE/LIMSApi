using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class CuttingPriceMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long? SpecimenTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CuttingType { get; set; } = string.Empty;
        [MaxLength(100)]
        [Required]
        public string UnitType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(7,2)")]
        public decimal? SizeRangeMin { get; set; }  // null for Water Jet / Gas / EDM

        [Column(TypeName = "decimal(7,2)")]
        public decimal? SizeRangeMax { get; set; }  // null for Water Jet / Gas / EDM

        public string? Remark { get; set; } = string.Empty;

        [ForeignKey("SpecimenTypeId"), JsonIgnore]
        public virtual SpecimenTypeMaster? SpecimenType { get; set; }

        // Year-wise rates (Normal/Hard metal). One row per financial year that has its own rate.
        public virtual ICollection<CuttingPriceVersion> Versions { get; set; } = new List<CuttingPriceVersion>();
    }
}
