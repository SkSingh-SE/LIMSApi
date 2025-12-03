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
        [Required]
        public long MetalClassificationID { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SampleTotal { get; set; }

        [ForeignKey("CuttingChargeHeaderID"), JsonIgnore]
        public virtual CuttingChargeHeader? CuttingChargeHeader { get; set; }


        [ForeignKey("MetalClassificationID"), JsonIgnore]
        public virtual MetalClassificationMaster? MetalClassification { get; set; }
        public ICollection<CuttingChargeDetail> CuttingChargeDetails { get; set; } = new List<CuttingChargeDetail>();
    }
}
