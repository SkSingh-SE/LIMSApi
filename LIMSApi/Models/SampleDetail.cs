using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    [Index(nameof(SampleNo), IsUnique = true)]
    public class SampleDetail : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required]
        public string SampleNo { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public long? MetalClassificationID { get; set; }
        public long? ProductConditionID { get; set; }
        public long? ProductMasterID { get; set; }
        public long? ProductSizeMasterID { get; set; }
        public long? SpecificationGradeID { get; set; }
        public bool IsUnknownSample { get; set; } = false;
        public long? AssignedGradeID { get; set; }
        [MaxLength(500)]
        public string? AssignedGradeNote { get; set; }
        // UI hidden per client requirement — field retained for data integrity
        public long? SpecimenOrientationID { get; set; } = null;
        // UI hidden per client requirement — field retained for data integrity
        public long? ProductFormID { get; set; }
        public string? Remarks { get; set; }
        public int Quantity { get; set; }
        public bool IsCancelled { get; set; }

        // Cancellation tracking (populated only when IsCancelled = true)
        public DateTime? CancelledOn { get; set; }
        public long? CancelledBy { get; set; }
        [MaxLength(500)]
        public string? CancellationReason { get; set; }

      
        public bool TpiRequired { get; set; }
        public long? TpiAgencyID { get; set; }
        [MaxLength(2000)]
        public string? TpiInspectorsJson { get; set; }
        public string? Specimen { get; set; }
        public string? TestInstructions { get; set; }
        public string SampleStatus { get; set; } = string.Empty;
        public long? UploadReferenceID { get; set; }
        [StringLength(255)]
        public string? SampleFilePath { get; set; }
        public string? FileName { get; set; }
        
        public bool IsReportUnlocked { get; set; }
        public long InwardID { get; set; }
        [ForeignKey("InwardID"), JsonIgnore]
        public virtual SampleInward? SampleInward { get; set; } = null!;

        public virtual ICollection<SampleAdditionalDetail> AdditionalDetails { get; set; } = new List<SampleAdditionalDetail>();
        public virtual ICollection<SampleTestPlan> TestPlans { get; set; } = new List<SampleTestPlan>();

        [ForeignKey("MetalClassificationID")]
        public virtual MetalClassificationMaster? MetalClassification { get; set; }

        [ForeignKey("ProductConditionID")]
        public virtual ProductConditionMaster? ProductCondition { get; set; }

        [ForeignKey("ProductMasterID")]
        public virtual ProductMaster? ProductMaster { get; set; }

        [ForeignKey("ProductSizeMasterID")]
        public virtual ProductSizeMaster? ProductSizeMaster { get; set; }

        [ForeignKey("SpecificationGradeID")]
        public virtual SpecificationGrade? SpecificationGrade { get; set; }

        [ForeignKey("AssignedGradeID")]
        public virtual SpecificationGrade? AssignedGrade { get; set; }

        // UI hidden per client requirement — navigation property retained for DB integrity
        [ForeignKey("SpecimenOrientationID")]
        public virtual SpecimenOrientationMaster? SpecimenOrientation { get; set; }

        [ForeignKey("ProductFormID")]
        public virtual ProductFormMaster? ProductForm { get; set; }

        [NotMapped]
        public IFormFile File { get; set; } = null!;

        public bool IsTestingCompleted { get; set; }
        public DateTime? TestingCompletedOn { get; set; }

        // Physical Dimensions (captured at Inward time, used for pricing)
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Thickness { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Diameter { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Width { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Length { get; set; }
    }
}
