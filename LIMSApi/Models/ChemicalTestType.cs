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
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        [ForeignKey("ChemicalTestID"),JsonIgnore]
        public virtual ChemicalTest? ChemicalTest { get; set; }
    }
}
