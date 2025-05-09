using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class ContactPerson
    {
        [Key]
        public long ID { get; set; }

        [MaxLength(10)]
        public string? Salutation { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        public long DepartmentID { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? EmailId { get; set; }

        [Phone]
        [MaxLength(15)]
        public string? MobileNo { get; set; }

        public bool IsWhatsappNo { get; set; }

        
        [MaxLength(15)]
        public string? TelephoneNo { get; set; }

       
        public bool SendBill { get; set; }

        public bool SendReport { get; set; }

        [MaxLength(255)]
        public string? BillReportDeliveryAddress { get; set; }

        [Required,StringLength(50)]
        public required string Type { get; set; }

        // Foreign key relationship with CustomerMaster
       
        public long CustomerID { get; set; }
        [ForeignKey("DepartmentID")]
        public virtual DepartmentMaster? Department {  get; set; }


    }
}
