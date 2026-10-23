using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestAnalysisTypeEquipment
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestAnalysisTypeID { get; set; }

        [Required]
        public long EquipmentID { get; set; }

        public bool IsDefault { get; set; } = false;

        [ForeignKey(nameof(LaboratoryTestAnalysisTypeID))]
        public virtual LaboratoryTestAnalysisType? AnalysisType { get; set; }

        [ForeignKey(nameof(EquipmentID))]
        public virtual EquipmentMaster? Equipment { get; set; }
    }
}
