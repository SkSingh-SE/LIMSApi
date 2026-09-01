using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class ProductMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long? ProductSizeMasterID { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? GradePrefix { get; set; }

        [StringLength(100)]
        public string? GradeValue { get; set; }

        [StringLength(300)]
        public string? DisplayTitle { get; set; }

        public bool IsSizeApplicable { get; set; } = true;

        [ForeignKey("ProductSizeMasterID")]
        public virtual ProductSizeMaster? ProductSizeMaster { get; set; }

        public virtual ICollection<ProductMasterMetalClassification> MetalClassifications { get; set; } = new List<ProductMasterMetalClassification>();
        public virtual ICollection<ProductMasterVersion> Versions { get; set; } = new List<ProductMasterVersion>();
    }
}
