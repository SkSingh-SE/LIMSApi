using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class CuttingChargeHeader : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required]
        public long InwardID { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotal { get; set; }

        [ForeignKey("InwardID"), JsonIgnore]
        public virtual SampleInward? SampleInward { get; set; }
        public virtual ICollection<CuttingChargeSample> Samples { get; set; } = new List<CuttingChargeSample>();
    }
}
