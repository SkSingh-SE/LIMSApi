using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    [Table("NablMasterDocuments")]
    public class NablMasterDocument : NablFormBase
    {
        [MaxLength(200)]
        public string? DocumentCode { get; set; }

        [MaxLength(500)]
        public string? DocumentTitle { get; set; }

        [MaxLength(50)]
        public string? DocumentType { get; set; } // SOP/Form/Policy/WorkInstruction/Other

        [MaxLength(50)]
        public string? CurrentIssue { get; set; }

        [MaxLength(50)]
        public string? CurrentRevision { get; set; }

        public DateTime? EffectiveDate { get; set; }

        [MaxLength(200)]
        public string? ReviewFrequency { get; set; }

        [MaxLength(200)]
        public string? DocumentOwner { get; set; }

        [MaxLength(500)]
        public string? StorageLocation { get; set; }

        public string? ControlledCopiesJson { get; set; } // JSON array of {copyNo, holder, location, dateIssued}

        public DateTime? ObsoleteDate { get; set; }

        public string? ObsoleteReason { get; set; }
        [StringLength(500)]
        public string? FileName { get; set; }
        [StringLength(500)]
        public string? FilePath { get; set; }
        public long? DocumentOwnerId { get; set; }
        public long DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? NextReviewDate { get; set; }
        [NotMapped]
        public List<ControlledCopies>? ControlledCopies { get; set; }
        public long? UploadReferenceID { get; set; }
        public DateTime? UploadedOn { get; set; }

        [ForeignKey("UploadReferenceID")]
        [JsonIgnore]
        public UploadFile? UploadFile { get; set; }
        [NotMapped]
        public bool HasReview { get; set; }

        [NotMapped]
        public long? ReviewId { get; set; }

        [NotMapped]
        public string? ReviewStatus { get; set; }
    }

    [NotMapped]
    public class ControlledCopies
    {
        public string? HolderName { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? Location { get; set; }
        public DateTime? DateIssued { get; set; }
    }

}
