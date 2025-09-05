using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class SampleTestPlan
    {
        [Key]
        public long ID { get; set; }
        public long SampleID { get; set; }
        public string SampleNo { get; set; }
        public List<GeneralTest> GeneralTests { get; set; } = new List<GeneralTest>();
        public List<ChemicalTest> ChemicalTests { get; set; } = new List<ChemicalTest>();

        [ForeignKey("SampleID"),JsonIgnore]
        public virtual SampleDetail? SampleDetail { get; set; }
    }
}
