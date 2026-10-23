using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestSubGroupParameter
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestSubGroupID { get; set; }

        [Required]
        public long ParameterID { get; set; }

        public bool IsMandatory { get; set; } = false;

        public bool IsReportable { get; set; } = true;

        public int Sequence { get; set; } = 0;

        [ForeignKey(nameof(LaboratoryTestSubGroupID))]
        public virtual LaboratoryTestSubGroup? SubGroup { get; set; }

        [ForeignKey(nameof(ParameterID))]
        public virtual ParameterMaster? Parameter { get; set; }
    }
}
