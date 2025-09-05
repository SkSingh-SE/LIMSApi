using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class ChemicalTestElement
    {
        [Key]
        public long ID { get; set; }
        public long ChemicalTestID { get; set; }
        public long ParameterID { get; set; }
        public bool Selected { get; set; }
        [ForeignKey("ChemicalTestID"), JsonIgnore]
        public virtual ChemicalTest? ChemicalTest { get; set; }
        [ForeignKey("ParameterID")]
        public virtual ParameterMaster? Parameter { get; set; }
    }
}
