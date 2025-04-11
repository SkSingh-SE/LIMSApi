using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class LabScopeMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Description {  get; set; }
        
        public long TestMethodID { get; set; }

        [ForeignKey("TestMethodID")]
        public virtual TestMethodMaster? TestMethod { get; set; }
    }
}
