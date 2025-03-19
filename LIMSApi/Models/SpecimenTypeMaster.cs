using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace LIMSApi.Models
{
    public class SpecimenTypeMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [StringLength(100)]
        public required string Name { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? NormalCharge { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? HardCharge { get; set; }
    }
}
