using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablInternalAuditors")]
    public class NablInternalAuditor : NablFormBase
    {
        public long? EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual EmployeeMaster? Employee { get; set; }

        [MaxLength(200)]
        public string? EmployeeName { get; set; }

        [MaxLength(500)]
        public string? Qualification { get; set; }

        [MaxLength(500)]
        public string? LeadAuditorCourse { get; set; }

        public DateTime? LeadAuditorCertDate { get; set; }

        [MaxLength(500)]
        public string? InternalAuditorCourse { get; set; }

        public DateTime? InternalAuditorCertDate { get; set; }

        [MaxLength(500)]
        public string? ISOClauses { get; set; }

        public string? AuditExperience { get; set; }

        public string? AuthorizedAreas { get; set; }

        public DateTime? AuthorizationDate { get; set; }

        public DateTime? AuthorizationValidUpto { get; set; }

        [MaxLength(200)]
        public string? AuthorizedBy { get; set; }
    }
}
