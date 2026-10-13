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

        [NotMapped]
        public string? TestMethodSpecificationVersionName => TestMethodSpecificationVersion?.Version;

        [NotMapped]
        public string? FullDisplayName
        {
            get
            {
                if (TestMethodSpecification == null) return null;
                var baseTitle = TestMethodSpecification.DisplayTitle ?? TestMethodSpecification.Name;
                if (TestMethodSpecificationVersion != null)
                {
                    var verStr = TestMethodSpecificationVersion.Version;
                    var yearStr = TestMethodSpecificationVersion.Year;
                    var caption = $"{baseTitle} : {verStr}";
                    if (!string.IsNullOrWhiteSpace(yearStr) && !caption.Contains(yearStr))
                        caption += $" {yearStr}";
                    if (TestMethodSpecificationVersion.IsDefault)
                        caption += " ★";
                    if (TestMethodSpecificationVersion.Status == LIMSApi.Helpers.Enums.VersionStatus.Superseded)
                        caption += " [Superseded]";
                    return caption;
                }
                return baseTitle;
            }
        }

        public ICollection<LabScopeSpecificationParameter> Parameters { get; set; } = new List<LabScopeSpecificationParameter>();
    }
}

