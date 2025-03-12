using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class UOMMaster: AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [StringLength(50)]
        public required string Name { get; set; }
    }
}
