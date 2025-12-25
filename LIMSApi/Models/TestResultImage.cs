using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class TestResultImage : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long TestResultHeaderID { get; set; }

        [Required]
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public int SortOrder { get; set; }
        public long? UploadReferenceID { get; set; }

        [ForeignKey(nameof(TestResultHeaderID))]
        public TestResultHeader? TestResultHeader { get; set; }
    }

}
