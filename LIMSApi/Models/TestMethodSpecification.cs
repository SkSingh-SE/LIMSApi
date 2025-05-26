using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class TestMethodSpecification : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        public long StandardOrganizationID { get; set; }
        [MaxLength(255)]
        [Required]
        public string TestMethodStandard { get; set; } = string.Empty;
        [MaxLength(500)]
        [Required]
        public required string Name { get; set; }
        public bool IsDisabled { get; set; } = false;
        public ICollection<TestMethodSpecificationVersion> Versions { get; set; } = new List<TestMethodSpecificationVersion>();

    }
}
