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

        public bool? LeadAuditorCourse { get; set; }

        public DateTime? LeadAuditorCertDate { get; set; }

        public bool? InternalAuditorCourse { get; set; }

        public DateTime? InternalAuditorCertDate { get; set; }

        [MaxLength(500)]
        public string? ISOClaus { get; set; }

        public int? AuditExperience { get; set; }

        public string? AuthorizedAreas { get; set; }

        public DateTime? AuthorizationDate { get; set; }

        public DateTime? AuthorizationValidUpto { get; set; }

        [MaxLength(200)]
        public string? AuthorizedBy { get; set; }
        public long? DepartmentId{ get; set; }
        public long? AuthorizedById { get; set; }
        public string? AuthorizedByName { get; set; }
        public string? DepartmentName { get; set; }
        public string? Designation { get; set; }
        public string? CertificateNo { get; set; }
        public string? TrainingOrganization { get; set; }
        public string? Remarks { get; set; }
        public DateTime? CertificateIssueDate { get; set; }
        public DateTime? CertificateExpiryDate { get; set; }
        public string? ISOClausesJson { get; set; }
        public string? DepartmentListJson { get; set; }
        [NotMapped]
        public List<DepartmentList>? DepartmentList { get; set; }
        [NotMapped]
        public List<IsoClauses>? IsoClauses{ get; set; }
    }
    [NotMapped]
    public class IsoClauses
    {
        public int? ClauseId { get; set; }
        public string? ClauseName { get; set; }
    }
    [NotMapped]
    public class DepartmentList
    {
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}
