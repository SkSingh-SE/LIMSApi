using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class PriceDimensionType : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Unit { get; set; }

        public bool IsRange { get; set; }

        // Where the engine reads the dimension value from.
        // "ParameterLinked" | "TestDuration" | "SampleDimension" | "ParameterCount" | "ParameterPresence" | "UserInputAtEntry" | "UserInput"
        [MaxLength(50)]
        public string ValueSource { get; set; } = "ParameterLinked";

        // Which SampleDetail field to read when ValueSource = "SampleDimension".
        // "Diameter" | "Thickness" | "Width" | "Length"
        [MaxLength(20)]
        public string? SampleField { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public int SortOrder { get; set; }
    }
}
