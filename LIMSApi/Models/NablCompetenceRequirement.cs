using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablCompetenceRequirements")]
    public class NablCompetenceRequirement : NablFormBase
    {
        // FK to existing Designation master (position)
        public long? PositionId { get; set; }

        [ForeignKey("PositionId")]
        public virtual DesignationMaster? Position { get; set; }

        [MaxLength(200)]
        public string? PositionName { get; set; }

        [MaxLength(500)]
        public string? MinimumEducation { get; set; }

        [MaxLength(500)]
        public string? MinimumExperience { get; set; }

        public bool IsExternal { get; set; }

        [MaxLength(500)]
        public string? RelatedActivity { get; set; }
    }
}
