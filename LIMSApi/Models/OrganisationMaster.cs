using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class OrganisationMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }
        [StringLength(100)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? YearSeparator { get; set; }
    }
}
