using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("UniversalTestResults")]
    public class UniversalTestResult : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long TestExecutionID { get; set; }

        public long UniversalTestGroupID { get; set; }

        public long BranchID { get; set; }

        public long OrganizationID { get; set; }

        public int RevisionNo { get; set; } = 1;

        [Required]
        [StringLength(30)]
        public string ResultStatus { get; set; } = "Draft";

        [Required]
        [StringLength(30)]
        public string OverallDecision { get; set; } = "NOT_EVALUATED";

        [StringLength(256)]
        public string? SnapshotHash { get; set; }

        public long? ExecutionConfigSnapshotID { get; set; }

        [StringLength(50)]
        public string? DecisionRule { get; set; }

        [StringLength(50)]
        public string? AcceptanceCriteriaCode { get; set; }

        public string? CalculationTraceJson { get; set; }

        public string? ComplianceSummaryJson { get; set; }

        public long? FinalizedBy { get; set; }
        public DateTime? FinalizedOn { get; set; }

        public long? ReviewerID { get; set; }
        public DateTime? ReviewedOn { get; set; }

        public long? VerifiedBy { get; set; }
        public DateTime? VerifiedOn { get; set; }

        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }

        [StringLength(1000)]
        public string? ReviewRemarks { get; set; }

        [StringLength(1000)]
        public string? ApprovalRemarks { get; set; }

        [Required]
        [StringLength(64)]
        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString("N");

        [ForeignKey("TestExecutionID")]
        public virtual TestExecution? TestExecution { get; set; }

        [ForeignKey("UniversalTestGroupID")]
        public virtual UniversalTestGroup? UniversalTestGroup { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch? Branch { get; set; }

        public virtual ICollection<UniversalTestResultParameter> Parameters { get; set; } = new List<UniversalTestResultParameter>();
        public virtual ICollection<UniversalReviewFinding> Findings { get; set; } = new List<UniversalReviewFinding>();
        public virtual ICollection<UniversalResultAudit> Audits { get; set; } = new List<UniversalResultAudit>();
    }
}
