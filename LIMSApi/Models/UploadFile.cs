using LIMSApi.Dtos;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("UploadFiles")]
    public class UploadFile : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public required string OriginalFileName { get; set; }

        [Required]
        public required string StoredFileName { get; set; }

        public FileType FileType { get; set; }
        public string? FileExtension { get; set; }
        public string? FilePath { get; set; }
        public long? FileSize { get; set; }
        public int? Year { get; set; }

    }
}
