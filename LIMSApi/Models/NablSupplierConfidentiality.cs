using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablSupplierConfidentialities")]
    public class NablSupplierConfidentiality : NablFormBase
    {
        public long? SupplierId { get; set; } // FK to Supplier (not yet in codebase)

        [MaxLength(200)]
        public string? SupplierName { get; set; }

        [MaxLength(200)]
        public string? ContactPerson { get; set; }

        public DateTime? AgreementDate { get; set; }

        public DateTime? AgreementValidUpto { get; set; }

        public string? ConfidentialItems { get; set; } // long text

        public string? PenaltyClause { get; set; } // long text

        [MaxLength(200)]
        public string? SignedBy { get; set; }

        [MaxLength(500)]
        public string? SupplierSignature { get; set; }
    }
}
