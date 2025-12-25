using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class ChemicalTestType
    {
        [Key]
        public long ID { get; set; }
        public long ChemicalTestID { get; set; }
        public long? LaboratoryTestID { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        [ForeignKey("ChemicalTestID"),JsonIgnore]
        public virtual ChemicalTest? ChemicalTest { get; set; }
        [ForeignKey("LaboratoryTestID")]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }
    }
}
