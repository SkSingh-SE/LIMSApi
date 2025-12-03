using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class SampleDetail : AuditProperty
    {
        
        public long ID { get; set; }
        public string SampleNo { get; set; }
        public string Details { get; set; }
        public long? MetalClassificationID { get; set; }
        public long? ProductConditionID { get; set; }
        public string Remarks { get; set; }
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
        public long InwardID { get; set; }
        [ForeignKey("InwardID"), JsonIgnore]
        public virtual SampleInward? SampleInward { get; set; } = null!;

        public virtual ICollection<SampleAdditionalDetail> AdditionalDetails { get; set; } = new List<SampleAdditionalDetail>();
        public virtual ICollection<SampleTestPlan> TestPlans { get; set; } = new List<SampleTestPlan>();


        [NotMapped]
        public IFormFile File { get; set; } = null!;
    }
}
