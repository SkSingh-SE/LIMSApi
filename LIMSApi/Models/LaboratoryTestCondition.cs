using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestCondition : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestID { get; set; }

        [Required]
        public long ConditionMasterID { get; set; }

        public bool IsMandatory { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey(nameof(ConditionMasterID))]
        public virtual ConditionMaster? ConditionMaster { get; set; }
    }
}
