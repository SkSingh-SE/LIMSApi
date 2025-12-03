using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class CuttingChargeDetail
    {
        [Key]
        public long ID { get; set; }
        public long CuttingChargeSampleID { get; set; }

        public long CuttingID { get; set; }
        public string CuttingType { get; set; }
        public string UnitType { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [ForeignKey("CuttingID")]
        public virtual CuttingPriceMaster? CuttingPriceMaster { get; set; }

        [ForeignKey("CuttingChargeSampleID"), JsonIgnore]
        public virtual CuttingChargeSample? CuttingChargeSample { get; set; }
    }
}
