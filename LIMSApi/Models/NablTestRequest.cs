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
        public string? SampleQuantity { get; set; }

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
    }
}
