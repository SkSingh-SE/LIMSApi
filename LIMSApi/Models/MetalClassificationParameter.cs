using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class MetalClassificationParameter
    {
        public long MetalClassificationID { get; set; }
        [ForeignKey("MetalClassificationID"), JsonIgnore]
        public virtual MetalClassificationMaster? MetalClassification { get; set; }

        public long ParameterID { get; set; }
        [ForeignKey("ParameterID")]
        public virtual ParameterMaster? Parameter { get; set; }
    }
}
