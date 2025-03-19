using System.ComponentModel.DataAnnotations;
using LIMSApi.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        [StringLength(500)]
        public string FileName { get; set; } = string.Empty; 

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;

        [ForeignKey("EmployeeID")]
        public EmployeeMaster? Employee { get; set; }

        [NotMapped]
        [ForeignKey("UploadReferenceID")]
        public UploadFile? UploadFile { get; set; }


    }
}
