using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class ChemicalTestElement
    {
        [Key]
        public long ID { get; set; }
        public long ParameterID { get; set; }
        public bool Selected { get; set; }
    }
}
