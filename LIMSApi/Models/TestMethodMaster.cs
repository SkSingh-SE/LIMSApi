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
        public long? LabDepartmentID { get; set; }
        [ForeignKey("LabDepartmentID")]
        public virtual DepartmentMaster? LabDepartment { get; set; }
        public ICollection<TestMethodSubGroup> SubGroups { get; set; } = new List<TestMethodSubGroup>();

    }
}
