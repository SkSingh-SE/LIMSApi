using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class ChemicalTest
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long SampleTestPlanID { get; set; }

        public string ReportNo { get; set; } = string.Empty;

        public string UlrNo { get; set; } = string.Empty;

        public long MetalClassificationID { get; set; }

        public long? Specification1 { get; set; }

        public long? Specification2 { get; set; }

        public long? TestMethod { get; set; } // not in use

        public long? LaboratoryTestAnalysisTypeID { get; set; }

        [ForeignKey(nameof(LaboratoryTestAnalysisTypeID))]
        public virtual LaboratoryTestAnalysisType? AnalysisType { get; set; }

        public virtual List<ChemicalTestType> TestTypes { get; set; } = new();

        public virtual ICollection<ChemicalTestElement> Elements { get; set; } = new List<ChemicalTestElement>();

        [ForeignKey("SampleTestPlanID"), JsonIgnore]
        public virtual SampleTestPlan? SampleTestPlan { get; set; }
    }
}
