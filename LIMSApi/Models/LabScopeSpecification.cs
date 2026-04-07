using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class LabScopeSpecification : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long LabScopeID { get; set; }
        public long TestMethodSpecificationID { get; set; }
        public long? TestMethodSpecificationVersionID { get; set; }

        [ForeignKey("LabScopeID"),JsonIgnore]
        public virtual LabScopeMaster? LabScope { get; set; }

        [ForeignKey("TestMethodSpecificationID"), JsonIgnore]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }

        [NotMapped]
        public string? TestMethodSpecificationName => TestMethodSpecification?.Name;

        [ForeignKey("TestMethodSpecificationVersionID"), JsonIgnore]
        public virtual TestMethodSpecificationVersion? TestMethodSpecificationVersion { get; set; }


        public ICollection<LabScopeSpecificationParameter> Parameters { get; set; } = new List<LabScopeSpecificationParameter>();
    }
}
