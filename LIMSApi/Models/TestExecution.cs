using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class TestExecution : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long UniversalTestGroupID { get; set; }

        public long BranchID { get; set; }

        public long OrganizationID { get; set; }

        public long? ExecutionAnalystID { get; set; }

        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }

        public long? VerifiedBy { get; set; }
        public DateTime? VerifiedOn { get; set; }

        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }

        [StringLength(500)]
        public string? ReviewRemarks { get; set; }

        public int ExecutionNo { get; set; } = 1;

        public bool IsRetest { get; set; } = false;

        public long? PreviousExecutionID { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "InProgress";

        public long? ExecutionConfigSnapshotID { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch? Branch { get; set; }

        [ForeignKey("UniversalTestGroupID")]
        public virtual UniversalTestGroup UniversalTestGroup { get; set; } = null!;

        [ForeignKey("OrganizationID")]
        public virtual Organization Organization { get; set; } = null!;

        [ForeignKey("ExecutionConfigSnapshotID")]
        public virtual ExecutionConfigSnapshot? ExecutionConfigSnapshot { get; set; }

        [ForeignKey("PreviousExecutionID")]
        public virtual TestExecution? PreviousExecution { get; set; }
        
        public virtual ICollection<TestSpecimen> TestSpecimens { get; set; } = new List<TestSpecimen>();
    }
}
