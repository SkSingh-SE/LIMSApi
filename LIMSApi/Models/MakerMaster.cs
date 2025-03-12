using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class MakerMaster :AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [StringLength(100)]
        public required string Name { get; set; }
    }
}
