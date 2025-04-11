using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class EmployeeQualification : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long EmployeeID { get; set; } 

        [Required]
        [StringLength(200)]
        public string Qualification { get; set; } = string.Empty; 

        [Required]
        [StringLength(200)]
        public string SchoolOrUniversity { get; set; } = string.Empty;

        [Required]
        public int PassingYear { get; set; }

        [ForeignKey("EmployeeID")]
        [JsonIgnore]
        public EmployeeMaster? Employee { get; set; }
    }
}
