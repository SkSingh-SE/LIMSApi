using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Models
{
    [Index(nameof(SubGroup), IsUnique = true)]
    public class LaboratoryTest : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        public long? LabDepartmentID { get; set; }
        [StringLength(100),]
        public required string SubGroup { get; set; }
        public string? Equation { get; set; }

        [ForeignKey("LabDepartmentID")]
        public virtual DepartmentMaster? LabDepartment { get; set; }
        public ICollection<LaboratoryTestInvoiceCase> InvoiceCases { get; set; } = new List<LaboratoryTestInvoiceCase>();
        //public ICollection<TestMethodSubGroup> SubGroups { get; set; } = new List<TestMethodSubGroup>();
        public ICollection<LaboratoryTestParameter> Parameters { get; set; } = new List<LaboratoryTestParameter>();

    }
}
