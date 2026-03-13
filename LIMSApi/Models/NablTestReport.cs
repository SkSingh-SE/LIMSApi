using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablTestReports")]
    public class NablTestReport : NablFormBase
    {
        [MaxLength(100)]
        public string? SampleCode { get; set; }

        [MaxLength(200)]
        public string? CustomerName { get; set; }

        public DateTime? ReportDate { get; set; }

        public string? TestResultsJson { get; set; } // JSON array

        [MaxLength(500)]
        public string? SamplingDetails { get; set; }

        [MaxLength(500)]
        public string? MethodReference { get; set; }

        [MaxLength(500)]
        public string? Conclusion { get; set; }

        [MaxLength(500)]
        public string? Disclaimer { get; set; }

        [MaxLength(200)]
        public string? IssuedBy { get; set; }

        public DateTime? IssuedDate { get; set; }

        [MaxLength(20)]
        public string? ReportVersion { get; set; }
    }
}
