using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class AreaMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long CityID { get; set; }  // Foreign Key to City

        public string? Code { get; set; }

        public required string Name { get; set; }

        public string? Pincode { get; set; }

        // Navigation Property
        [ForeignKey("CityID")]
        public virtual CityMaster? City { get; set; }
    }

}
