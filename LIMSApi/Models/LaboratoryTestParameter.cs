using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestParameter : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestID { get; set; }

        [Required]
        public long ParameterID { get; set; }

        public bool IsMandatory { get; set; } = true;

        public bool IsReportable { get; set; } = true;

        public int DisplayOrder { get; set; } = 0;

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey(nameof(ParameterID))]
        public virtual ParameterMaster? Parameter { get; set; }
    }
}
