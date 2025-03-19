using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class TestGroup :AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Sample { get; set; } = string.Empty;

        public virtual ICollection<TestGroupMapping> TestGroupMappings { get; set; } = new List<TestGroupMapping>();
    }
}
