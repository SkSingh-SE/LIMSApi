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

        [StringLength(500)]
        public string? TestCaption { get; set; }

        [StringLength(500)]
        public string? InvoiceCaption { get; set; }

        [Range(1, 365)]
        public int? TestDuration { get; set; }

        public int GlobalUsageCount { get; set; }
        public int RecentUsageCount { get; set; }
        public DateTime? LastPerformedDate { get; set; }

        [ForeignKey("LabDepartmentID")]
        public virtual DepartmentMaster? LabDepartment { get; set; }
        public ICollection<LaboratoryTestInvoiceCase> InvoiceCases { get; set; } = new List<LaboratoryTestInvoiceCase>();

    }
}
