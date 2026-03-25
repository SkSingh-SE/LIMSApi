using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// Audit trail for financial year changes — tracks who changed, when, and why.
    /// </summary>
    [Table("FinancialYearChangeLogs")]
    public class FinancialYearChangeLog
    {
        [Key]
        public long Id { get; set; }

        public long FinancialYearId { get; set; }

        [Required, StringLength(50)]
        public string OldYear { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string NewYear { get; set; } = string.Empty;

        public long ChangedById { get; set; }

        public DateTime ChangedOn { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Reason { get; set; }

        [ForeignKey(nameof(FinancialYearId))]
        public virtual FinancialYear? FinancialYear { get; set; }

        [ForeignKey(nameof(ChangedById))]
        public virtual UserMaster? ChangedBy { get; set; }
    }
}
