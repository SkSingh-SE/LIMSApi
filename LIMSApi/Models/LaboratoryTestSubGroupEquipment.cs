using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LaboratoryTestSubGroupEquipment
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long LaboratoryTestSubGroupID { get; set; }

        [Required]
        public long EquipmentID { get; set; }

        public bool IsDefault { get; set; } = false;

        [ForeignKey(nameof(LaboratoryTestSubGroupID))]
        public virtual LaboratoryTestSubGroup? SubGroup { get; set; }

        [ForeignKey(nameof(EquipmentID))]
        public virtual EquipmentMaster? Equipment { get; set; }
    }
}
