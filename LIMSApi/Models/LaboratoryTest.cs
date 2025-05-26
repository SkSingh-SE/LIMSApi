using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTest : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        public long? LabDepartmentID { get; set; }
        [StringLength(100)]
        public required string SubGroup { get; set; }

        public string InvoiceCase { get; set; }

        [ForeignKey("LabDepartmentID")]
        public virtual DepartmentMaster? LabDepartment { get; set; }
        //public ICollection<TestMethodSubGroup> SubGroups { get; set; } = new List<TestMethodSubGroup>();

    }
}
