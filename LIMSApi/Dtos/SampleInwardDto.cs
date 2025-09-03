using System.ComponentModel.DataAnnotations;
using LIMSApi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Dtos
{
    public class SampleInwardDto
    {
       
        public long ID { get; set; }
       
        public string CaseNo { get; set; } = string.Empty;

       
        public long CustomerID { get; set; }

       
        public string Address { get; set; } = string.Empty;

       
        public string Area { get; set; } = string.Empty;

        
        public string State { get; set; } = string.Empty;

        
        public string City { get; set; } = string.Empty;

       
        public string PinCode { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        
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

       
        public string? SampleReceiptNote { get; set; }

        
        public string? RequestFilePath { get; set; }

       
        public string? RequestFileName { get; set; }

        public long? UploadReferenceID { get; set; } = null;
        public string Status { get; set; } = "Inward Initiated";
        public DateTime CollectionTime { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<DispatchModeDto> DispatchModes { get; set; } = new List<DispatchModeDto>();
        public virtual ICollection<ContactDto> Contacts { get; set; } = new List<ContactDto>();
        //public virtual ICollection<SampleInwardAddressInfo> Addresses { get; set; }
        //= new List<SampleInwardAddressInfo>();
        public required PartyAddressDto ReportingTo { get; set; }
        public required PartyAddressDto BillingTo { get; set; }
        public virtual ICollection<SampleDetailDto> SampleDetails { get; set; } = new List<SampleDetailDto>();
        public virtual ICollection<SampleAdditionalDetailDto> SampleAdditionalDetails { get; set; } = new List<SampleAdditionalDetailDto>();
        public ICollection<SampleTestPlanDto> SampleTestPlans { get; set; } = new List<SampleTestPlanDto>();

        [NotMapped]
        public IFormFile? File { get; set; } = null!;
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
        public long InwardID { get; set; }
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
        public long InwardID { get; set; }
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
        public string? SampleFilePath { get; set; }
        public string? FileName { get; set; }
        public long InwardID { get; set; }

        public IFormFile? File { get; set; }
    }

    public class SampleAdditionalDetailDto
    {
        public long ID { get; set; }
        public string SampleNo { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public long SampleID { get; set; }
    }

    public class SampleTestPlanDto
    {
        public string? SampleNo { get; set; }
        public List<GeneralTestDto> GeneralTests { get; set; } = new();
        public List<ChemicalTestDto> ChemicalTests { get; set; } = new();
    }

    public class GeneralTestDto
    {
        public string? SampleNo { get; set; }
        public string? Specification1 { get; set; }
        public string? Specification2 { get; set; }
        public string? Parameter { get; set; }
        public List<TestMethodDto> Methods { get; set; } = new();
    }

    public class TestMethodDto
    {
        public long? TestMethodID { get; set; }
        public long? StandardID { get; set; }
        public int Quantity { get; set; }
        public string? ReportNo { get; set; }
        public string? UlrNo { get; set; }
        public bool Cancel { get; set; }
    }

    public class ChemicalTestDto
    {
        public string? SampleNo { get; set; }
        public string? ReportNo { get; set; }
        public string? UrlNo { get; set; }
        public List<ElementDto> Elements { get; set; } = new();
    }

    public class ElementDto
    {
        public long? ElementID { get; set; }
        public string? ElementName { get; set; }
        public int Quantity { get; set; }
    }
}
