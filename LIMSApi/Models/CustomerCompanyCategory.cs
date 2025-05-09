using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class CustomerCompanyCategory
    {
        [Key]
        public long ID { get; set; }

        public long CustomerID { get; set; }
        public long CompanyCategoryID { get; set; }

        [ForeignKey("CustomerID")]
        [JsonIgnore]
        public virtual Customer? Customer { get; set; } = null!;

        [ForeignKey("CompanyCategoryID")]
        public virtual CompanyCategoryMaster? CompanyCategory { get; set; } = null!;
    }
}
