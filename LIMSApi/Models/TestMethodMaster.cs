using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class TestMethodMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        [StringLength(100)]
        public string? Caption { get; set; }
        [StringLength(100)]
        public string? TestMethodSubGroup { get; set; }
        [StringLength(500)]
        public string? InvoiceCase { get; set; }
        public long? LabDepartmentID { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? TestCharge { get; set; }
        public int FixedTimeDuration {  get; set; }
        public string? SampleSize { get; set; }

    }
}
