using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("FinancialYears")]
    public class FinancialYear : AuditProperty
    {
        [Key]
        public long Id { get; set; }

        [Required, StringLength(50)]
        public string Year { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public long OrganizationId { get; set; }
    }
}
