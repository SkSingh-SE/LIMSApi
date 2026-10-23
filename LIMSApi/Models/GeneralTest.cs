using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class GeneralTest
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long SampleTestPlanID { get; set; }

        public long? LaboratoryTestSubGroupID { get; set; }

        public long? Specification1 { get; set; }

        public long? Specification2 { get; set; }

        [ForeignKey(nameof(LaboratoryTestSubGroupID))]
        public virtual LaboratoryTestSubGroup? SubGroup { get; set; }

        public virtual ICollection<GeneralTestMethod> Methods { get; set; } = new List<GeneralTestMethod>();

        [ForeignKey("SampleTestPlanID"), JsonIgnore]
        public virtual SampleTestPlan? SampleTestPlan { get; set; }
    }
}
