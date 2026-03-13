using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablFormAttachments")]
    public class NablFormAttachment : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long FormDataId { get; set; }

        [Required, MaxLength(50)]
        public string FormType { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FileType { get; set; }

        public long FileSize { get; set; }
    }
}
