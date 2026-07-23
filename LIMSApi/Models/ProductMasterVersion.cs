using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class ProductMasterVersion : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long ProductMasterID { get; set; }

        [StringLength(50)]
        public string VersionNumber { get; set; } = "1";

        [StringLength(10)]
        public string? Year { get; set; }

        [StringLength(300)]
        public string? SpecificationFilePath { get; set; }

        public long? StandardOrganizationID { get; set; }

        [StringLength(100)]
        public string? SpecStdNo { get; set; }

        [StringLength(100)]
        public string? PartSection { get; set; }

        [StringLength(300)]
        public string? Title { get; set; }

        [StringLength(300)]
        public string? ProductCaption { get; set; }

        public bool IsActiveVersion { get; set; } = true;

        [ForeignKey("ProductMasterID"), JsonIgnore]
        public virtual ProductMaster? ProductMaster { get; set; }

        [ForeignKey("StandardOrganizationID")]
        public virtual StandardOrganizationMaster? StandardOrganization { get; set; }

        public virtual ICollection<ProductMasterVersionGrade> Grades { get; set; } = new List<ProductMasterVersionGrade>();
    }
}
