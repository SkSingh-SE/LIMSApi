using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class NablEmployeeTestMethodAuthorization
    {
        [Key]
        public long Id { get; set; }
        public long EmployeeAuthorizationId { get; set; }
        public long TestMethodId { get; set; }
        public string TestMethodName { get; set; }

        [ForeignKey("EmployeeAuthorizationId")]
        [JsonIgnore]
        public NablEmployeeAuthorization? EmployeeAuthorization { get; set; }
    }
}