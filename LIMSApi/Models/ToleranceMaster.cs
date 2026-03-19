using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public class ToleranceMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    public long? SpecificationHeaderID { get; set; }  // Optional: specific to a specification
    public long? ParameterID { get; set; }  // Optional: specific to a parameter

    [MaxLength(200)]
    public string? StandardName { get; set; }  // e.g., "BS 970"

    [Column(TypeName = "decimal(18,4)")]
    public decimal? ValueRangeStart { get; set; }  // e.g., 0

    [Column(TypeName = "decimal(18,4)")]
    public decimal? ValueRangeEnd { get; set; }  // e.g., 100

    [Column(TypeName = "decimal(18,4)")]
    public decimal Tolerance { get; set; }  // e.g., ±0.5

    [MaxLength(50)]
    public string? ToleranceType { get; set; }  // "Absolute", "Percentage"

    [MaxLength(500)]
    public string? Remark { get; set; }

    [ForeignKey("SpecificationHeaderID")]
    public virtual SpecificationHeader? SpecificationHeader { get; set; }

    [ForeignKey("ParameterID")]
    public virtual ParameterMaster? Parameter { get; set; }
}
