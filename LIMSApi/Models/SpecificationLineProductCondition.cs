using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class SpecificationLineProductCondition
    {
        public long SpecificationLineID { get; set; }
        [JsonIgnore, ForeignKey("SpecificationLineID")]
        public virtual SpecificationLine? ProductSpecificationLine { get; set; }

        public long ProductConditionID { get; set; }
    }
}
