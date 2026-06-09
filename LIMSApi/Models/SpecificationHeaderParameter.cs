using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    /// <summary>
    /// Header-level parameter+unit list for a material specification. Acts as the template:
    /// when a grade is added, these are copied into that grade's SpecificationLines, routed by Type.
    /// </summary>
    public class SpecificationHeaderParameter
    {
        [Key]
        public long ID { get; set; }

        public long SpecificationHeaderID { get; set; }

        public long ParameterID { get; set; }

        public long? ParameterUnitID { get; set; }

        /// 'chemical' | 'general'
        [MaxLength(20)]
        public string Type { get; set; } = "chemical";

        public int? DisplayOrder { get; set; }

        [ForeignKey(nameof(SpecificationHeaderID))]
        [JsonIgnore]
        public virtual SpecificationHeader? SpecificationHeader { get; set; }

        [ForeignKey(nameof(ParameterID))]
        public virtual ParameterMaster? Parameter { get; set; }

        [ForeignKey(nameof(ParameterUnitID))]
        public virtual ParameterUnitMaster? ParameterUnit { get; set; }
    }
}
