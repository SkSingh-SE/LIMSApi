using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class Customer : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        [Required, StringLength(100)]
        public required string Name { get; set; }

        public string? LegalName { get; set; }

        [Required, StringLength(500)]
        public required string Address { get; set; }

        public long CountryID { get; set; }
        public long StateID { get; set; }
        public long CityID { get; set; }

        public long AreaID { get; set; }
        [StringLength(10)]
        public required string PinCode { get; set; }
        public long CurrencyID { get; set; }
        public required string  CustomerType { get; set; }

        public bool IsBlock { get; set; }


        [StringLength(50)]
        public required string GSTNo { get; set; }
        [StringLength(50)]
        public string? PANNo { get; set; }

        public bool GSTNA { get; set; }

        [MaxLength(255)]
        public required string TallyLedgerName { get; set; }


        public bool SampleReturn { get; set; }

        public bool BillingEvery { get; set; }

        public int? BillingEveryDays { get; set; }
        [StringLength(100)]
        public string? SpecialAccountingCase { get; set; }

        public bool WeeklyBillingCustomer { get; set; }

        public bool MonthlyBillingCustomer { get; set; }

        public bool DirectTaxInvoiceNoPerforma { get; set; }

        public bool PerformaInvoiceRequiredBeforeTesting { get; set; }

        public bool ConstantDiscount { get; set; }

        public decimal? ConstantDiscountPercentage { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CreditLimitAmount { get; set; }
        public int? CreditLimitTime { get; set; } // in days

        [MaxLength(500)]
        public string? Remark { get; set; }
        public string? DTestoLoginId { get; set; }
        public string? DTestoPassword { get; set; }
        public bool DTestoActive { get; set; }
        public bool BlockDTestoUser { get; set; }

        [StringLength(250)]
        public string? BlockReason { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime? VerifiedOn { get; set; }
        public long? VerifiedBy { get; set; }

        public virtual ICollection<ContactPerson> ContactPersons { get; set; } = new List<ContactPerson>();
        public virtual ICollection<CustomerCompanyCategory> CustomerCompanyCategories { get; set; } = new List<CustomerCompanyCategory>();

        public virtual ICollection<CustomerDispatchMode> CustomerDispatchModes { get; set; } = new List<CustomerDispatchMode>();

    }
}
