using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Models
{
    [Table("NablEmployeePerformanceRecords")]
    public class NablEmployeePerformanceRecord : NablFormBase
    {
        // FK to existing Employee master
        public long EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual EmployeeMaster? Employee { get; set; }

        [MaxLength(200)]
        public string? EmployeeName { get; set; }

        [MaxLength(100)]
        public string? DesignationName { get; set; }

        // Review Period (e.g., "Q1 2026", "H1 2026", "Annual 2025")
        [Required, MaxLength(100)]
        public string ReviewPeriod { get; set; } = string.Empty;

        // Ratings (1-5 scale)
        [Precision(4, 2)]
        public decimal TechnicalRating { get; set; }

        [Precision(4, 2)]
        public decimal BehavioralRating { get; set; }

        [Precision(4, 2)]
        public decimal OverallRating { get; set; }

        // Reviewer
        [MaxLength(200)]
        public string? ReviewerName { get; set; }

        public long? ReviewerId { get; set; }

        public DateTime? ReviewDate { get; set; }

        // Remarks
        [MaxLength(1000)]
        public string? Remarks { get; set; }
    }
}
