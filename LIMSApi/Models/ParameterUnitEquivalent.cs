using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models;

/// <summary>
/// Normalized equivalent unit of a base <see cref="ParameterUnitMaster"/>.
/// Own stable ID so renames don't break references; soft-deletable for add/remove anytime.
/// Replaces the denormalized inline SimilarUnit1-7 / ConversionFactor1-7 columns.
/// </summary>
public class ParameterUnitEquivalent
{
    [Key]
    public long ID { get; set; }

    public long BaseParameterUnitID { get; set; }

    [Required, StringLength(50)]
    public required string Name { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? ConversionFactor { get; set; }

    public int? DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    [ForeignKey("BaseParameterUnitID"), JsonIgnore]
    public virtual ParameterUnitMaster? BaseParameterUnit { get; set; }
}
