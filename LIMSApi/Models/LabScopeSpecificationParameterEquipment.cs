using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class LabScopeSpecificationParameterEquipment
    {
        [Key]
        public long ID { get; set; }

        public long LabScopeSpecificationParameterID { get; set; }

        public long EquipmentID { get; set; }
    }
}
