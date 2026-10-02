using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablNonConformingWorkVerifications")]
    public class NablNonConformingWorkVerification
    {
        [Key]
        public long Id { get; set; }
        public long NablNonConformingWorkId { get; set; }
        public DateTime? VerificationDate { get; set; }
        public long? VerifiedByEmployeeId { get; set; }

        [MaxLength(200)]
        public string? VerifiedByEmployeeName { get; set; }

        [MaxLength(200)]
        public string? VerificationMethod { get; set; }
        public string? Observation { get; set; }

        [MaxLength(50)]
        public string? Result { get; set; }

        public string? Remarks { get; set; }

        [ForeignKey("NablNonConformingWorkId")]
        public virtual NablNonConformingWork? NonConformingWork { get; set; }
    }
}
