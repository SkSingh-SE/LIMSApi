using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class RemarkMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }= string.Empty;
        [MaxLength(250)]
        public string? Description { get; set; }
    }
}
