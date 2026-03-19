using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public class HardnessEquivalence : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [Required, MaxLength(50)]
    public string HardnessScale { get; set; } = string.Empty;  // "BHN", "HV", "HRC", "HRB"

    [MaxLength(50)]
    public string? IndenterSize { get; set; }  // "1mm", "2.5mm", "5mm", "10mm"

    [Column(TypeName = "decimal(18,2)")]
    public decimal? LoadKgf { get; set; }  // Applied load

    [Column(TypeName = "decimal(18,2)")]
    public decimal FromValue { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ToValue { get; set; }

    [MaxLength(50)]
    public string? EquivalentScale { get; set; }  // Target scale

    [Column(TypeName = "decimal(18,2)")]
    public decimal? EquivalentFromValue { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? EquivalentToValue { get; set; }

    [MaxLength(100)]
    public string? Standard { get; set; }  // "ASTM E10", "ASTM E140"
}
