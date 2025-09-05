using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class GeneralTest
    {
        [Key]
        public long ID { get; set; }
        public long SampleTestPlanID { get; set; }
        public long Specification1 { get; set; }
        public long? Specification2 { get; set; }
        public string Parameter { get; set; }
        public ICollection<GeneralTestMethod> Methods { get; set; }

        [ForeignKey("SampleTestPlanID"), JsonIgnore]
        public virtual SampleTestPlan? SampleTestPlan { get; set; }
    }
}
