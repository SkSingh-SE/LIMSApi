using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class SamplePreparationTestItem : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long SamplePreparationID { get; set; }

        public long SampleID { get; set; }

        public long? TestPlanID { get; set; }

        /// <summary>
        /// "General" or "Chemical"
        /// </summary>
        [Required, MaxLength(20)]
        public string PlannedTestType { get; set; } = "General";

        /// <summary>
        /// Exact PlannedTestID (FK to GeneralTestMethod.ID or ChemicalTestMethod.ID)
        /// </summary>
        public long PlannedTestMethodID { get; set; }

        public long LaboratoryTestID { get; set; }

        public long? TestMethodSpecificationID { get; set; }

        /// <summary>
        /// Snapshot link to the SpecimenPreparationMaster (MachiningChargeMaster)
        /// </summary>
        public long? SpecimenPreparationMasterID { get; set; }

        [Required, MaxLength(300)]
        public string SpecimenSize { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? SpecimenRawMaterialSize { get; set; }

        [MaxLength(500)]
        public string? DrawingFilePath { get; set; }

        [MaxLength(255)]
        public string? FileName { get; set; }

        public int Quantity { get; set; } = 1;

        public bool CuttingRequired { get; set; } = true;

        public bool MachiningRequired { get; set; } = true;

        /// <summary>
        /// If true, specimen is prepared/machined only; no testing execution in laboratory.
        /// </summary>
        public bool NoTesting { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ResolvedCuttingRate { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ResolvedMachiningRate { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CuttingTotal { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MachiningTotal { get; set; } = 0;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        /// <summary>
        /// Item-level preparation status: "Pending", "CuttingCompleted", "MachiningCompleted", "Completed", "Cancelled"
        /// </summary>
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public DateTime? CompletedOn { get; set; }

        public long? CompletedByEmployeeID { get; set; }

        // Navigation
        [ForeignKey("SamplePreparationID")]
        public virtual SamplePreparation? SamplePreparation { get; set; }

        [ForeignKey("SampleID")]
        public virtual SampleDetail? Sample { get; set; }

        [ForeignKey("LaboratoryTestID")]
        public virtual LaboratoryTest? LaboratoryTest { get; set; }

        [ForeignKey("SpecimenPreparationMasterID")]
        public virtual MachiningChargeMaster? SpecimenPreparationMaster { get; set; }
    }
}
