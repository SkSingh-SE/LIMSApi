using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models;

/// <summary>
/// Dropdown / MultiSelect options for a ParameterMaster.
/// Only populated when ParameterMaster.InputType = "Dropdown" or "MultiSelect".
/// Example: Parameter = "Surface Finish" → Options: Smooth, Rough, Pitted
/// </summary>
public class ParameterDropdownOption
{
    public long ID { get; set; }

    [Required]
    public long ParameterID { get; set; }

    [Required]
    [MaxLength(200)]
    public string DisplayText { get; set; } = string.Empty;   // e.g. "Smooth"

    [MaxLength(100)]
    public string Value { get; set; } = string.Empty;         // stored value (can differ from display)

    public int DisplayOrder { get; set; } = 0;

    public bool IsDefault { get; set; } = false;

    public bool IsActive { get; set; } = true;

    // Navigation
    [ForeignKey("ParameterID"), JsonIgnore]
    public virtual ParameterMaster? Parameter { get; set; }
}
