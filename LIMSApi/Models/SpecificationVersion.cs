using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using LIMSApi.Helpers.Enums;
using Microsoft.AspNetCore.Http;

namespace LIMSApi.Models
{
    public class SpecificationVersion
    {
        [Key]
        public long ID { get; set; }

        public long SpecificationHeaderID { get; set; }

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

        [NotMapped]
        public IFormFile? File { get; set; }

        // True if this version is the specification's default edition
        public bool IsDefault { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [MaxLength(50)]
        public string CompanyCode { get; set; } = "LIMS";

        [ForeignKey("SpecificationHeaderID")]
        [JsonIgnore]
        public virtual SpecificationHeader? SpecificationHeader { get; set; }

        public virtual ICollection<SpecificationVersionParameter> Parameters { get; set; } = new List<SpecificationVersionParameter>();

        public virtual ICollection<SpecificationLine> SpecificationLines { get; set; } = new List<SpecificationLine>();
    }
}
