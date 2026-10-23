using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LIMSApi.Helpers.Enums;

namespace LIMSApi.Models
{
    public class TestMethodSpecificationVersion
    {
        [Key]
        public long ID { get; set; }

        public long TestMethodSpecificationID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Version { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Year { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        [EnumDataType(typeof(VersionStatus))]
        public VersionStatus Status { get; set; } = VersionStatus.Draft;

        public DateTime? EffectiveDate { get; set; }

        public DateTime? SupersededDate { get; set; }

        public DateTime? ReviewDate { get; set; }

        [MaxLength(500)]
        public string? ChangeReason { get; set; }

        [MaxLength(255)]
        public string? StandardFile { get; set; }

        [MaxLength(500)]
        public string? StandardFilePath { get; set; }

        public long? UploadReferenceID { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public IFormFile? file { get; set; }

        // True if this version is the spec's default (auto-selected in inward/plan dropdowns).
        public bool IsDefault { get; set; }

        [ForeignKey("TestMethodSpecificationID")]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual TestMethodSpecification? TestMethodSpecification { get; set; }

        // Parameters this edition reports (version-level so old editions keep their own set).
        public ICollection<TestMethodSpecificationParameter> Parameters { get; set; } = new List<TestMethodSpecificationParameter>();
    }
}
