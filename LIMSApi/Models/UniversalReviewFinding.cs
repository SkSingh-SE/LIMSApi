using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("UniversalReviewFindings")]
    public class UniversalReviewFinding : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long UniversalTestResultID { get; set; }

        public long TestExecutionID { get; set; }

        [Required]
        [StringLength(50)]
        public string FindingType { get; set; } = "Observation";

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Severity { get; set; } = "Major";

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Open";

        public bool IsBlocking { get; set; } = true;

        [StringLength(1000)]
        public string? Resolution { get; set; }

        public long? ResolvedBy { get; set; }
        public DateTime? ResolvedOn { get; set; }

        [ForeignKey("UniversalTestResultID")]
        public virtual UniversalTestResult? UniversalTestResult { get; set; }
    }
}
