using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Models
{
    [Index(nameof(Name), nameof(SelectionType), IsUnique = true)]
    public class InvoiceCaseConfiguration : AuditProperty

    {
        [Key]
        public long ID { get; set; }
        [MaxLength(100)]
        public string SelectionType { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string AliasName { get; set; } = string.Empty; // comma-separated

        [MaxLength(100)]
        public string Value { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Start { get; set; } = string.Empty;

        [MaxLength(100)]
        public string End { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;

        // Comma-separated ParameterMaster IDs this config dimension reads from.
        // ParameterLinked: engine reads value from first matching billable param.
        // SpectroCombination: engine checks which of these param IDs are in billable params.
        [MaxLength(1000)]
        public string? SourceParameterIDs { get; set; }

        // SpectroCombination only: marks this as the base "Full" config whose standard elements
        // must ALL be present before extra-tier configs ("Full + N") are evaluated.
        // Extra-tier configs (IsBaseConfig = false) are checked first; this is the fallback tier.
        public bool IsBaseConfig { get; set; } = false;

        // When ValueSource = "ParameterLinked" and no linked param is found in billable params,
        // fall back to asking user for manual input at test result entry.
        public bool FallbackToUserInput { get; set; } = false;

        // Dynamic Pricing Dimension (Phase 3)
        public long? PriceDimensionTypeId { get; set; }
        public virtual PriceDimensionType? DimensionType { get; set; }

        // Navigation property
        public ICollection<InvoiceCaseAliasName> AliasNames { get; set; } = new List<InvoiceCaseAliasName>();
    }
}
