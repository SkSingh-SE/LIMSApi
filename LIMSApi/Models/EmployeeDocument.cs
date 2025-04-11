using System.ComponentModel.DataAnnotations;
using LIMSApi.Dtos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class EmployeeDocument:AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long EmployeeID { get; set; }  

        public long UploadReferenceID { get; set; }

        public required string DocumentType { get; set; } 
        public bool IsAdditional { get; set; }

        [StringLength(500)]
        public string? FileName { get; set; } = string.Empty; 

        [StringLength(500)]
        public string? FilePath { get; set; } = string.Empty;
        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;

        
        [ForeignKey("UploadReferenceID")]
        public UploadFile? UploadFile { get; set; }

        [ForeignKey("EmployeeID")]
        [JsonIgnore]
        public EmployeeMaster? Employee { get; set; }

        [NotMapped]
        public IFormFile? file { get; set; }

    }
}
