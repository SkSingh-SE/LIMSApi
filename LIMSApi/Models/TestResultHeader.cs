using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    [Table("TestResultHeaders")]
    public class TestResultHeader : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long SampleID { get; set; }

        [Required]
        public long LaboratoryTestID { get; set; }

        [Required]
        public long TestPlanID { get; set; }

        /// <summary>
        /// Specimen number within same (SampleID, LaboratoryTestID, TestPlanID).
        /// When GeneralTestMethod.Quantity = 3, creates headers with SequenceNo 1, 2, 3.
        /// </summary>
        public int SequenceNo { get; set; } = 1;

        public long? TestID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        // Pass/Fail for the entire test
        public bool? IsOverallPass { get; set; }

        // Is this test NABL accredited or not?
        public bool IsNabl { get; set; } = false;
        public string? LabNo { get; set; }
        [MaxLength(128)]
        public string? CertificateNo { get; set; }
        public long? EquipmentID { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public long? StartedBy { get; set; }

        // ----- Lab/Room tracking -----
        public long? LabRoomId { get; set; }
        [Column(TypeName = "decimal(6,2)")]
        public decimal? RoomTemperature { get; set; }
        [Column(TypeName = "decimal(6,2)")]
        public decimal? RoomHumidity { get; set; }
        public string? EquipmentIdsJson { get; set; }

        // ----- Enhanced time tracking -----
        public DateTime? TestStartTime { get; set; }
        public DateTime? TestEndTime { get; set; }
        [MaxLength(200)]
        public string? PerformedByName { get; set; }
        public long? PerformedById { get; set; }

        // ----- Price fields -----
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CalculatedPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OverridePrice { get; set; }
        [MaxLength(500)]
        public string? OverrideReason { get; set; }
        public long? OverrideById { get; set; }
        public bool PriceOverridden { get; set; } = false;

        [MaxLength(50)]
        public string? SelectedPricingType { get; set; }

        [MaxLength(200)]
        public string? PricingDimensionValue { get; set; }

        // ----- Verification Workflow (Phase 5) -----
        public bool PreparationDataMissing { get; set; }

        // ----- Orientation Deviation -----
        public bool OrientationDeviationAcknowledged { get; set; } = false;

        // ----- Additional Test Information (ISO 17025) -----
        [MaxLength(100)]
        public string? TestPurpose { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        // ----- Layout Versioning & Historical Snapshot -----
        public long? LayoutTemplateId { get; set; }
        public int? LayoutVersion { get; set; }
        public string? LayoutSnapshotJson { get; set; }

        // ----- Retest Linkage (ISO 17025 Clause 7.7) -----
        public long? RetestOfHeaderId { get; set; }
        [MaxLength(50)]
        public string? RetestReasonCode { get; set; }

        // ----- Approval Lock & NABL Tamper-Evident Seal -----
        [MaxLength(64)]
        public string? ApprovalHash { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public long? ApprovedById { get; set; }
        public string? ReportSnapshotJson { get; set; }

        // ----- UI State & Transient Extras -----
        public string? ExecutionExtraJson { get; set; }

        // ----- Optimistic Concurrency Token -----
        [Timestamp]
        public byte[]? RowVersion { get; set; }

        // Navigation Properties
        public ICollection<TestResultParameter> Parameters { get; set; } = new List<TestResultParameter>();
        public ICollection<TestResultImage> Images { get; set; } = new List<TestResultImage>();
        public ICollection<LongTermTest> LongTermTests { get; set; } = new List<LongTermTest>();
        public ICollection<TestResultReading> Readings { get; set; } = new List<TestResultReading>();
        public ICollection<TestResultLocation> Locations { get; set; } = new List<TestResultLocation>();
        public ICollection<TestResultTraversePoint> TraversePoints { get; set; } = new List<TestResultTraversePoint>();
        public ICollection<TestResultChecklistItem> ChecklistItems { get; set; } = new List<TestResultChecklistItem>();
        public ICollection<TestResultObservation> Observations { get; set; } = new List<TestResultObservation>();
        public ICollection<TestResultAuditLog> AuditLogs { get; set; } = new List<TestResultAuditLog>();

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }
        [ForeignKey(nameof(SampleID))]
        public virtual SampleDetail? Sample { get; set; }
        [ForeignKey(nameof(RetestOfHeaderId))]
        public virtual TestResultHeader? RetestOfHeader { get; set; }
        [ForeignKey(nameof(LayoutTemplateId))]
        public virtual TestLayoutTemplate? LayoutTemplate { get; set; }
    }

}
