using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablSupplierRegistrations")]
    public class NablSupplierRegistration : NablFormBase
    {
        public long? SupplierId { get; set; }

        [MaxLength(200)]
        public string? SupplierName { get; set; }

        [MaxLength(100)]
        public string? SupplierCode { get; set; }

        [MaxLength(200)]
        public string? ContactPerson { get; set; }

        [MaxLength(200)]
        public string? Designation { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(50)]
        public string? MobileNo { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        // Business details
        [MaxLength(100)]
        public string? NatureOfBusiness { get; set; } // Manufacturer, Trader, Service Provider

        public string? ProductsServicesOffered { get; set; }

        [MaxLength(100)]
        public string? SupplierCategory { get; set; }

        [MaxLength(500)]
        public string? ItemsSupplied { get; set; }

        // Compliance
        [MaxLength(20)]
        public string? GstNo { get; set; }

        [MaxLength(20)]
        public string? PanNo { get; set; }

        public bool IsoCertified { get; set; }

        [MaxLength(500)]
        public string? IsoDetails { get; set; }

        // Bank details stored as JSON: { bankName, accountNo, ifscCode, branch }
        public string? BankDetailsJson { get; set; }

        // Documents submitted stored as JSON: { gstCertificate, panCard, isoCertificate, cancelledCheque, msmeCertificate, otherDocs }
        public string? DocumentsSubmittedJson { get; set; }

        // Registration
        public DateTime? RegistrationDate { get; set; }

        public DateTime? RegistrationValidUpto { get; set; }

        public bool NablApproved { get; set; }

        [MaxLength(50)]
        public string? RegistrationStatus { get; set; } // Pending, Approved, Rejected

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [MaxLength(200)]
        public string? RecordedBy { get; set; }

        [MaxLength(200)]
        public string? VerifiedBy { get; set; }
        public string? RegisterNo { get; set; }
        [NotMapped]
        public BankDetail? BankDetail { get; set; }
        [NotMapped]
        public DocumentsSubmitted? DocumentsSubmitted { get; set; }
    }
    [NotMapped]
    public class BankDetail
    {
        public string BankName{ get; set; }
        public string AccountNo { get; set; }
        public string IfscCode { get; set; }
        public string Branch { get; set; }
    }
    [NotMapped]
    public class DocumentsSubmitted
    {
        public bool MonopolyCert { get; set; }
        public bool PopularBrandCert { get; set; }
        public bool IsoCertificate { get; set; }
        public bool WorkmanshipCert { get; set; }
        public bool DeliveryRecord { get; set; }
        public bool SupplierConfidentiality { get; set; }
        public bool SupplierApproved { get; set; }
        public string ReasonNotApproved { get; set; }
        public bool Price { get; set; }
        public bool EvaluationRequired { get; set; }

    }
}
