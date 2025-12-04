using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class LaboratoryTestParameter
    {
        [Key]
        public long ID { get; set; }

        public long LaboratoryTestID { get; set; }

        public long ParameterID { get; set; }
        [ForeignKey("LaboratoryTestID"),JsonIgnore]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }
        [ForeignKey("ParameterID")]
        public virtual ParameterMaster? Parameter { get; set; }

    }
}
