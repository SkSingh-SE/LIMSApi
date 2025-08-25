using System.ComponentModel.DataAnnotations;
using LIMSApi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Dtos
{
    public class SampleInwardDto
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [MaxLength(20)]
        public string CaseNo { get; set; } = string.Empty;

        [Required]
        public long CustomerID { get; set; }

        [Required, StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Area { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, RegularExpression(@"^\d{6}$")]
        public string PinCode { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required, StringLength(15)]
        public string GstNo { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdvancePayment { get; set; }

        public bool BillRequired { get; set; }
        public bool AdvancePIRequired { get; set; }
        public bool HoldTesting { get; set; }
        public bool HoldTestingUntilPIApproved { get; set; }
        public bool Urgent { get; set; }
        public bool ReturnSample { get; set; }
        public bool NotDestroyed { get; set; }

        [StringLength(500)]
        public string? SampleReceiptNote { get; set; }

        [StringLength(300)]
        public string? RequestFilePath { get; set; }

        [StringLength(200)]
        public string? RequestFileName { get; set; }

        public long? UploadReferenceID { get; set; } = null;
        public string Status { get; set; } = "Inward Initiated";

        // Navigation Properties
        public virtual ICollection<DispatchModeDto> DispatchModes { get; set; } = new List<DispatchModeDto>();
        public virtual ICollection<ContactDto> Contacts { get; set; } = new List<ContactDto>();
        //public virtual ICollection<SampleInwardAddressInfo> Addresses { get; set; }
        //= new List<SampleInwardAddressInfo>();
        public required PartyAddressDto ReportingTo { get; set; }
        public required PartyAddressDto BillingTo { get; set; }
        public virtual ICollection<SampleDetailDto> SampleDetails { get; set; } = new List<SampleDetailDto>();
        public virtual ICollection<SampleAdditionalDetailDto> SampleAdditionalDetails { get; set; } = new List<SampleAdditionalDetailDto>();
        //public virtual ICollection<SampleTestPlan> SampleTestPlans { get; set; } = new List<SampleTestPlan>();

        [NotMapped]
        public IFormFile File { get; set; } = null!;
    }

    public class DispatchModeDto
    {
        public long ID { get; set; }

        public long InwardID { get; set; }
        public long DispatchModeID { get; set; }
    }

    public class ContactDto
    {
        public long ID { get; set; }
        public bool Selected { get; set; }
        public long ContactID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public bool SendBill { get; set; }
        public bool SendReport { get; set; }
    }

    public class PartyAddressDto
    {
        public long ID { get; set; }
        public long ContactPersonID { get; set; }
        public string ContactPersonName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Type { get; set; }
    }

    public class SampleDetailDto
    {
        public long ID { get; set; }
        public string SampleNo { get; set; }
        public string Details { get; set; }
        public string Nature { get; set; }
        public string Category { get; set; }
        public string Remarks { get; set; }
        public int Quantity { get; set; }
        public bool Disabled { get; set; }

        public long? UploadReferenceID { get; set; }
        [StringLength(255)]
        public string? SampleFilePath { get; set; }
        public string? FileName { get; set; }

        public IFormFile? File { get; set; }
    }

    public class SampleAdditionalDetailDto
    {
        public long ID { get; set; }
        public string SampleNo { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class SampleTestPlanDto
    {
        public string SampleNo { get; set; } = string.Empty;
        public List<GeneralTestDto> GeneralTests { get; set; } = new();
        public List<ChemicalTestDto> ChemicalTests { get; set; } = new();

        public IFormFile? File { get; set; }
    }

    public class GeneralTestDto
    {
        public string? Specification1 { get; set; }
        public string? Specification2 { get; set; }
        public string? Parameter { get; set; }

        public List<TestMethodDto> Methods { get; set; } = new();

        public IFormFile? File { get; set; }
    }

    public class ChemicalTestDto
    {
        public IFormFile? File { get; set; }
    }

    public class TestMethodDto
    {
        public string? TestMethodID { get; set; }
        public string? StandardID { get; set; }
        public int Quantity { get; set; }
        public string ReportNo { get; set; } = string.Empty;
        public string UlrNo { get; set; } = string.Empty;
        public bool Cancel { get; set; }

        public IFormFile? File { get; set; }
    }
}
