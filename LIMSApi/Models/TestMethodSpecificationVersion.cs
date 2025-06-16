using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [MaxLength(255)]
        public string? StandardFile { get; set; }

        [MaxLength(500)]
        public string? StandardFilePath { get; set; }

        public bool Default { get; set; }
        public long? UploadReferenceID { get; set; }

        [NotMapped]
        public IFormFile? file { get; set; } 
    }

}
