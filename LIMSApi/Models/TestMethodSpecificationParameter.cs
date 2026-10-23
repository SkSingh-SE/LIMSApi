using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    // A parameter listed inside a Test Method Specification VERSION, with an optional comment
    // capturing any special testing condition (e.g. for %El -> "GL = 5.65√A").
    // Version-level: each edition keeps its own parameter set so an old version can be reused later.
    public class TestMethodSpecificationParameter
    {
        [Key]
        public long ID { get; set; }

        // Parameters belong to a specific version (edition), not the spec as a whole.
        public long TestMethodSpecificationVersionID { get; set; }
        [ForeignKey("TestMethodSpecificationVersionID"), JsonIgnore]
        public virtual TestMethodSpecificationVersion? TestMethodSpecificationVersion { get; set; }

        public long ParameterID { get; set; }
        [ForeignKey("ParameterID")]
        public virtual ParameterMaster? Parameter { get; set; }

        // Chosen unit: base unit (default = parameter's default unit) + optional equivalent unit selection.
        public long? ParameterUnitID { get; set; }
        [ForeignKey("ParameterUnitID")]
        public virtual ParameterUnitMaster? ParameterUnit { get; set; }

        public long? ParameterUnitEquivalentID { get; set; }

        // Special testing conditions / remarks for this parameter under this specification.
        [MaxLength(1000)]
        public string? Comment { get; set; }

        public int SortOrder { get; set; } = 0;
    }
}
