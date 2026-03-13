using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablResponsibilityAuthorities")]
    public class NablResponsibilityAuthority : NablFormBase
    {
        // FK to existing Designation master
        public long DesignationId { get; set; }

        [ForeignKey("DesignationId")]
        public virtual DesignationMaster? Designation { get; set; }

        [MaxLength(100)]
        public string? DesignationName { get; set; }

        // Responsibility & Authority content (HTML)
        public string? Responsibilities { get; set; }
        public string? Authorities { get; set; }

        // Acceptance
        public bool EmployeeAccepted { get; set; }
        public DateTime? AcceptanceTimestamp { get; set; }

        [MaxLength(500)]
        public string? EmployeeSignature { get; set; }

        // Approval
        [MaxLength(200)]
        public string? IssuedBy { get; set; }

        [MaxLength(200)]
        public string? ReviewedApprovedBy { get; set; }
    }
}
