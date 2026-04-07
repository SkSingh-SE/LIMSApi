using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// Coordination + NABL tracking layer for sample preparation.
    /// Wraps existing CuttingCharge*, MachiningChargeItem, and SampleDetail.OtherPreparation
    /// into a single trackable entity per sample.
    /// </summary>
    public class SamplePreparation : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long SampleID { get; set; }
        public long InwardID { get; set; }

        // ── Workflow Status ──
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";
        // Values: Pending, Assigned, InProgress, Completed, QCVerified, Rejected

        // ── Operator Assignment ──
        public long? AssignedToEmployeeID { get; set; }
        public long? PreparedByEmployeeID { get; set; }
        public long? VerifiedByEmployeeID { get; set; }

        // ── Timestamps ──
        public DateTime? AssignedOn { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime? VerifiedOn { get; set; }

        // ── NABL: Equipment & Method ──
        public long? EquipmentID { get; set; }

        [MaxLength(200)]
        public string? PreparationMethod { get; set; }

        // ── NABL: Environment at Preparation Time ──
        [Column(TypeName = "decimal(5,1)")]
        public decimal? Temperature { get; set; }

        [Column(TypeName = "decimal(5,1)")]
        public decimal? Humidity { get; set; }

        // ── Notes & Instructions ──
        [MaxLength(1000)]
        public string? PreparationInstructions { get; set; }

        [MaxLength(1000)]
        public string? PreConditionNotes { get; set; }

        [MaxLength(1000)]
        public string? PostConditionNotes { get; set; }

        [MaxLength(500)]
        public string? VerificationRemarks { get; set; }

        // ── Denormalized Charge Totals (updated on save) ──
        [Column(TypeName = "decimal(18,2)")]
        public decimal CuttingChargesTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MachiningChargesTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OtherChargesTotal { get; set; }

        // ── Navigation ──
        [ForeignKey("SampleID")]
        public virtual SampleDetail? Sample { get; set; }

        [ForeignKey("InwardID")]
        public virtual SampleInward? SampleInward { get; set; }

        [ForeignKey("AssignedToEmployeeID")]
        public virtual EmployeeMaster? AssignedTo { get; set; }

        [ForeignKey("PreparedByEmployeeID")]
        public virtual EmployeeMaster? PreparedBy { get; set; }

        [ForeignKey("VerifiedByEmployeeID")]
        public virtual EmployeeMaster? VerifiedBy { get; set; }

        [ForeignKey("EquipmentID")]
        public virtual EquipmentMaster? Equipment { get; set; }
    }
}
