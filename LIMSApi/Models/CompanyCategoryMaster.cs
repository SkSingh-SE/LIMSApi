using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class CompanyCategoryMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [StringLength(250)]
        public string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Description { get; set; }

        public virtual ICollection<CustomerCompanyCategory> CustomerCompanyCategories { get; set; } = new List<CustomerCompanyCategory>();

    }
}
