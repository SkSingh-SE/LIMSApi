using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class SupplierMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductType { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContactPerson1 { get; set; }
        [StringLength(100)]
        public string? ContactPerson2 { get; set; }
        [StringLength(100)]
        public string? ContactPerson3 { get; set; }

        [Phone, StringLength(100)]
        public string? ContactNo1 { get; set; }
        [Phone, StringLength(100)]
        public string? ContactNo2 { get; set; }
        [Phone, StringLength(100)]
        public string? ContactNo3 { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? EmailId1 { get; set; }
        [StringLength(100)]
        [EmailAddress]
        public string? EmailId2 { get; set; }
        [StringLength(100)]
        [EmailAddress]
        public string? EmailId3 { get; set; }

        [StringLength(500)]
        public string? Address {  get; set; }
        [Required]
        [EnumDataType(typeof(SupplierStatus))]
        [Column(TypeName = "varchar(50)")]
        public SupplierStatus PresentStatus { get; set; }

        public long? UploadReferenceID { get; set; }
        [StringLength(255)]
        public string? AgreementFilePath { get; set; } 
        public string? FileName { get; set; }

        [Required]
        public bool SupplierApproved { get; set; }

        [Required]
        public bool IsBlacklisted { get; set; }

        [StringLength(255)]
        public string? ReasonForBlacklisting { get; set; }

        public long? BlacklistedBy { get; set; }

        public DateTime? BlacklistDate { get; set; }

        public long? EvaluatedBy { get; set; } // Evaluation by GM

        public long? ApprovedBy { get; set; } // Approval by MD / TD

        public DateTime? EvaluationDate { get; set; }
        [NotMapped]
        public IFormFile? file { get; set; }
    }
    public enum SupplierStatus
    {
        Enlisted = 1,
        Delisted = 2
    }
}
