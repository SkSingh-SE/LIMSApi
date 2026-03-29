using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class LabScopeSpecificationParameter
    {
        [Key]
        public long ID { get; set; }

        public long LabScopeSpecificationID { get; set; }

        public long ParameterID { get; set; }
        public long ParameterUnitID { get; set; }
        public string QualitativeQuantitative { get; set; } = string.Empty;
        public bool IsUnderISO { get; set; }
        public string? LowerLimit { get; set; }       // Operator: >, ≥, =
        public decimal? LowerLimitValue { get; set; }  // Actual numeric lower limit
        public string? UpperLimit { get; set; }        // Operator: <, ≤, =
        public decimal? UpperLimitValue { get; set; }  // Actual numeric upper limit
        public long? DisciplineID { get; set; }
        public long? GroupID { get; set; }
        public long? SubGroupID { get; set; }
        public List<LabScopeSpecificationParameterEquipment> Equipments { get; set; } = new List<LabScopeSpecificationParameterEquipment>();
    }
}
