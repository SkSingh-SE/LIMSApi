using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class NablAccreditation : AuditProperty
    {
        [Key]
        public long Id { get; set; }
        public string CertificateNumber { get; set; } = null!;
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? CertificatePath { get; set; }
        public string? LogoPath { get; set; }
        public long OrganizationId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("OrganizationId")]
        public virtual Organization? Organization { get; set; }

        public long? BranchID { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("BranchID")]
        public virtual Branch? Branch { get; set; }
    }
}
