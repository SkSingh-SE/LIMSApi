using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class SpecificationVersionParameter
    {
        [Key]
        public long ID { get; set; }

        public long SpecificationVersionID { get; set; }

        [ForeignKey("SpecificationVersionID"), JsonIgnore]
        public virtual SpecificationVersion? SpecificationVersion { get; set; }

        public long ParameterID { get; set; }

        [ForeignKey("ParameterID")]
        public virtual ParameterMaster? Parameter { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public int SortOrder { get; set; } = 1;
    }
}
