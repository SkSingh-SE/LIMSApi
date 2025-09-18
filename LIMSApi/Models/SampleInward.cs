using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class SampleInward:AuditProperty
    {
        
        [Key]
        public long ID { get; set; }
        [Required]
        [MaxLength(20)]
        public string CaseNo { get; set; } = string.Empty;

        [Required]
        public long CustomerID { get; set; }

        [Required, StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Area { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, RegularExpression(@"^\d{6}$")]
        public string PinCode { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required, StringLength(15)]
        public string GstNo { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdvancePayment { get; set; }

        public bool BillRequired { get; set; }
        public bool AdvancePIRequired { get; set; }
        public bool HoldTesting { get; set; }
        public bool HoldTestingUntilPIApproved { get; set; }
        public bool Urgent { get; set; }
        public bool ReturnSample { get; set; }
        public bool NotDestroyed { get; set; }

        [StringLength(500)]
        public string? SampleReceiptNote { get; set; }
        public string? StatementOfConformity { get; set; }
        public string? DecisionRule { get; set; }

        [StringLength(300)]
        public string? RequestFilePath { get; set; }

        [StringLength(200)]
        public string? RequestFileName { get; set; }

        public long? UploadReferenceID { get; set; } = null;
        public string Status { get; set; } = "Sample Received";

        public DateTime CollectionTime { get; set; } = DateTime.UtcNow;
        public string ReviewStatus { get; set; } = "Pending";
        public long? ReviewedBy { get; set; }     
        public DateTime? ReviewedOn { get; set; }

        // Navigation Properties
        public virtual ICollection<SampleDispatchMode> DispatchModes { get; set; } = new List<SampleDispatchMode>();
        public virtual ICollection<SampleInwardContactPerson> Contacts { get; set; } = new List<SampleInwardContactPerson>();
        public virtual ICollection<SampleInwardAddressInfo> Addresses { get; set; }
        = new List<SampleInwardAddressInfo>();
        public virtual ICollection<SampleDetail> SampleDetails { get; set; } = new List<SampleDetail>();
        //public virtual ICollection<SampleTestPlan> SampleTestPlans { get; set; } = new List<SampleTestPlan>();

        [ForeignKey("CustomerID")]
        public virtual Customer? Customer { get; set; } = null!;

        [NotMapped]
       public IFormFile File { get; set; } = null!;
    }


}
