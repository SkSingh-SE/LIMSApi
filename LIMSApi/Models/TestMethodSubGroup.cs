using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class TestMethodSubGroup
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        [StringLength(500)]
        public string? InvoiceCase { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? TestCharge { get; set; }
        public string? SampleSize { get; set; }// consider in mm
        public long? TestMethodID {  get; set; }

        [ForeignKey("TestMethodID")]
        [JsonIgnore]
        public virtual LaboratoryTest? TestMethod {  get; set; } 

    }
}
