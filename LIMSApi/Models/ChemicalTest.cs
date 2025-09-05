using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class ChemicalTest
    {
        [Key]
        public long ID { get; set; }
        public long SamplePlanID { get; set; }
        public string ReportNo { get; set; }
        public string UlrNo { get; set; }
        //public Dictionary<string, bool> TestTypes { get; set; }
        public long MetalClassificationID { get; set; }
        public long Specification1 { get; set; }
        public long? Specification2 { get; set; }
        public long TestMethod { get; set; }
        public ICollection<ChemicalTestElement> Elements { get; set; } = new List<ChemicalTestElement>();

        [ForeignKey("SamplePlanID"), JsonIgnore]
        public virtual SampleTestPlan? SampleTestPlan { get; set; }
    }
}
