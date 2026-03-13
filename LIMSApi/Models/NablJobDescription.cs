using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablJobDescriptions")]
    public class NablJobDescription : NablFormBase
    {
        // FK to existing Designation master
        public long DesignationId { get; set; }

        [ForeignKey("DesignationId")]
        public virtual DesignationMaster? Designation { get; set; }

        [MaxLength(100)]
        public string? DesignationName { get; set; }

        // FK to existing Department master
        public long DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster? Department { get; set; }

        [MaxLength(100)]
        public string? DepartmentName { get; set; }

        [MaxLength(200)]
        public string? ReportingTo { get; set; }

        // Section II - Education & Experience
        [MaxLength(500)]
        public string? MinimumQualification { get; set; }

        [MaxLength(500)]
        public string? TechnicalTraining { get; set; }

        [MaxLength(500)]
        public string? Experience { get; set; }

        // Section III - Principal Accountabilities (HTML content)
        public string? PrincipalAccountabilities { get; set; }

        // Section IV - Authorities
        public bool AuthorityStopTesting { get; set; }
        public bool AuthorityIssueReports { get; set; }
        public bool AuthorityAccessConfidential { get; set; }
        public bool AuthorityEquipmentCalibration { get; set; }

        // Section V - QMS Responsibilities (HTML content)
        public string? QmsResponsibilities { get; set; }

        // Section VI - Confidentiality Clause
        public string? ConfidentialityClause { get; set; }

        // Section VII - Approval
        [MaxLength(200)]
        public string? PreparedByName { get; set; }

        [MaxLength(200)]
        public string? ApprovedByName { get; set; }

        public bool EmployeeAccepted { get; set; }
    }
}
