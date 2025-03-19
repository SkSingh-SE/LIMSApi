namespace LIMSApi.Models
{
    public class AuditProperty
    {
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CompanyCode { get; set; } = "LIMS";
        public bool IsActive { get; set; } = true;
    }

}
