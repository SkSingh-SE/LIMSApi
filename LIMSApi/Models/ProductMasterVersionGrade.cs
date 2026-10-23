using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class ProductMasterVersionGrade : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long ProductMasterVersionID { get; set; }

        public long SpecificationGradeID { get; set; }

        public int SortOrder { get; set; } = 1;

        [ForeignKey("ProductMasterVersionID"), JsonIgnore]
        public virtual ProductMasterVersion? ProductMasterVersion { get; set; }

        [ForeignKey("SpecificationGradeID")]
        public virtual SpecificationGrade? SpecificationGrade { get; set; }

        public virtual ICollection<ProductMasterVersionGradeCondition> Conditions { get; set; } = new List<ProductMasterVersionGradeCondition>();
    }
}
