using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class ChemicalSampleCategory : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public int SortOrder { get; set; } = 0;
    }
}
