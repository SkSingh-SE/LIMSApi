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

        // ----- Verification Workflow (Phase 5) -----
        public bool PreparationDataMissing { get; set; }

        // Navigation Properties
        public ICollection<TestResultParameter> Parameters { get; set; } = new List<TestResultParameter>();
        public ICollection<TestResultImage> Images { get; set; } = new List<TestResultImage>();
        public ICollection<LongTermTest> LongTermTests { get; set; } = new List<LongTermTest>();

        [ForeignKey(nameof(LaboratoryTestID))]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }
        [ForeignKey(nameof(SampleID))]
        public virtual SampleDetail? Sample { get; set; }
    }

}
