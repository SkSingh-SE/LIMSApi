using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class SampleInward:AuditProperty
    {
        [Key]
        public long ID { get; set; }
        public long CustomerID { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string GstNo { get; set; } = string.Empty;
        public decimal AdvancePayment { get; set; }
        public bool BillRequired { get; set; }
        public bool AdvancePIRequired { get; set; }
        public bool HoldTesting { get; set; } = false;
        public bool HoldTestingUntilPiApproved { get; set; } = false ;
        public bool Urgent { get; set; }
        public bool ReturnSample { get; set; }
        public bool NotDestroyed { get; set; }
        public bool SameAsAbove { get; set; }

        public string Status { get; set; } = "Pending";

        public ICollection<SampleDispatchMode> DispatchModes { get; set; }
        public ICollection<SampleInwardContactPerson> Contacts { get; set; }
        public SampleInwardAddressInfo ReportingTo { get; set; }
        public SampleInwardAddressInfo BillingTo { get; set; }
        public ICollection<SampleDetail> SampleDetails { get; set; }
        public ICollection<SampleAdditionalDetail> SampleAdditionalDetails { get; set; }
        public ICollection<SampleTestPlan> SampleTestPlans { get; set; }
    }


}
