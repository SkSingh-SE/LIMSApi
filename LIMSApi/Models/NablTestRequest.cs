using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablTestRequests")]
    public class NablTestRequest : NablFormBase
    {
        public long? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [MaxLength(200)]
        public string? CustomerName { get; set; }

        [MaxLength(500)]
        public string? SampleDescription { get; set; }

        [MaxLength(200)]
        public int? SampleQuantity { get; set; }

        [MaxLength(200)]
        public string? SampleCondition { get; set; }

        public DateTime? RequestDate { get; set; }

        public DateTime? RequiredByDate { get; set; }

        public string? TestParametersJson { get; set; } // JSON array

        [MaxLength(500)]
        public string? SpecialRequirements { get; set; }

        [MaxLength(200)]
        public string? ContactPerson { get; set; }

        [MaxLength(50)]
        public string? ContactPhone { get; set; }

        [MaxLength(200)]
        public string? ContactEmail { get; set; }

        [MaxLength(500)]
        public string? ReferenceStandard { get; set; }

        [MaxLength(500)]
        public string? TestPurpose { get; set; }
        public string? Address { get; set; }
        public string? Remarks { get; set; }
        public string? GstNo { get; set; }
        public string? PoNumber { get; set; }
        public bool? Urgent { get; set; }
        public bool? ReturnSample { get; set; }
        public bool? HoldTesting { get; set; }
        public bool? BillRequired { get; set; }
        public bool? ConfirmityRequired { get; set; }
        public string? DispatchModeJson { get; set; }
        [NotMapped]
        public List<string>? DispatchModes { get; set; }
        [NotMapped]
        public List<samples>? Samples { get; set; }
        public string? Note { get; set; }

    }
    [NotMapped]
    public class samples
    {
        public string? SampleNo { get; set; }
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public string? MaterialSpecification { get; set; }
        public string? TestToPerform { get; set; }
        public string? MetalClassification { get; set; }

    }

}

