using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class SpecificationLineLaboratoryTest
    {
        public long SpecificationLineID { get; set; }
        [JsonIgnore, ForeignKey("SpecificationLineID")]
        public virtual SpecificationLine? TestSpecificationLine { get; set; }

        public long LaboratoryTestID { get; set; }
    }
}
