using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class TestMaster : AuditProperty
    {
        [Key] 
        public long ID { get; set; }

        [Required]
        [MaxLength(250)] 
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)] 
        public string TestCaption { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string InvoiceCaption { get; set; } = string.Empty;

        [Required] 
        public long LabDepartmentID { get; set; }

        [Required]
        [Range(1, 365)] 
        public int TestDuration { get; set; }

    }
}
