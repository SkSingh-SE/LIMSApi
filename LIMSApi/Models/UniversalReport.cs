using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// Phase 9 Universal Report — controlled DOCUMENT history, NOT a configuration layer.
    /// One row per (Execution, Approved ResultRevision, ReportRevision).
    /// Released content is reproduced from ReportDataJson, never by re-resolving live masters.
    /// </summary>
    [Table("UniversalReports")]
    public class UniversalReport : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long TestExecutionID { get; set; }

        public long UniversalTestResultID { get; set; }

        /// <summary>Phase 7 approved result revision this report was built from.</summary>
        public int ResultRevisionNo { get; set; } = 1;

        /// <summary>Phase 9 own counter per (Execution, ResultRevision): 1,2,3...</summary>
        public int ReportRevisionNo { get; set; } = 1;

        [Required]
        [StringLength(100)]
        public string ReportNo { get; set; } = string.Empty;

        [StringLength(256)]
        public string? SnapshotHash { get; set; }

        [StringLength(256)]
        public string? ResultRevisionHash { get; set; }

        /// <summary>Canonical frozen presentation payload (lab/branch/customer/signatory/NABL-display frozen at Generate).</summary>
        public string? ReportDataJson { get; set; }

        [StringLength(256)]
        public string? ReportDataHash { get; set; }

        public string? PdfPath { get; set; }

        [StringLength(256)]
        public string? PdfHash { get; set; }

        /// <summary>GENERATED | RELEASED | SUPERSEDED | VOID</summary>
        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "GENERATED";

        public long BranchID { get; set; }

        public long OrganizationID { get; set; }

        public DateTime? GeneratedOn { get; set; }

        public long? GeneratedBy { get; set; }

        public DateTime? ReleasedOn { get; set; }

        public long? ReleasedBy { get; set; }

        [ForeignKey("TestExecutionID")]
        public virtual TestExecution? TestExecution { get; set; }

        [ForeignKey("UniversalTestResultID")]
        public virtual UniversalTestResult? UniversalTestResult { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch? Branch { get; set; }
    }
}
