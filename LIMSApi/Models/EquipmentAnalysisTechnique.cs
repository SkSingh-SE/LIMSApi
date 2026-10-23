using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class EquipmentAnalysisTechnique : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long EquipmentID { get; set; }
        public long AnalysisTechniqueID { get; set; }

        [ForeignKey(nameof(EquipmentID))]
        public virtual EquipmentMaster? Equipment { get; set; }

        [ForeignKey(nameof(AnalysisTechniqueID))]
        public virtual AnalysisTechniqueMaster? AnalysisTechnique { get; set; }
    }
}
