using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class NablEmployeeLaborartyTestAuthorization
    {
        [Key]
        public long Id { get; set; }
        public long EmployeeAuthorizationId { get; set; }
        public long LabTestId { get; set; }
        public string LabTestName { get; set; }
        [ForeignKey("EmployeeAuthorizationId")]
        [JsonIgnore]
        public NablEmployeeAuthorization? EmployeeAuthorization { get; set; }
    }
}
