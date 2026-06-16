using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    // Junction: links a Test Method Specification to the Metal Classifications it applies to.
    // Used to narrow the test-method selection in sample inward by the sample's metal classification.
    public class TestMethodSpecificationMetalClassification
    {
        public long TestMethodSpecificationID { get; set; }
        [ForeignKey("TestMethodSpecificationID"), JsonIgnore]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }

        public long MetalClassificationID { get; set; }
        [ForeignKey("MetalClassificationID")]
        public virtual MetalClassificationMaster? MetalClassification { get; set; }
    }
}
