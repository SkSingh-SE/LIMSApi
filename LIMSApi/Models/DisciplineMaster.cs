using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class DisciplineMaster: AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required, StringLength(100)] 
        public string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Description { get; set; }

    }
}
