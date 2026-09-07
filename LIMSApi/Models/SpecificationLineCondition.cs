using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class SpecificationLineCondition : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long SpecificationLineID { get; set; }

        public long ConditionMasterID { get; set; }

        [StringLength(20)]
        public string Operator { get; set; } = "=";

        [StringLength(100)]
        public string Value1 { get; set; } = null!;

        [StringLength(100)]
        public string? Value2 { get; set; }

        [ForeignKey("SpecificationLineID")]
        public virtual SpecificationLine SpecificationLine { get; set; } = null!;

        [ForeignKey("ConditionMasterID")]
        public virtual ConditionMaster ConditionMaster { get; set; } = null!;
    }
}
