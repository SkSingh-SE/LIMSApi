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
        public long? SpecimenOrientationID { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool Disabled { get; set; }

        public bool PreparationRequired { get; set; }
        public bool MachiningRequired { get; set; }
        public decimal MachiningAmount { get; set; }
        public bool OtherPreparation { get; set; }
        public decimal OtherPreparationCharge { get; set; }
        public bool TpiRequired { get; set; }
        public long? TpiAgencyID { get; set; }
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

        [ForeignKey("SpecimenOrientationID")]
        public virtual SpecimenOrientationMaster? SpecimenOrientation { get; set; }

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
