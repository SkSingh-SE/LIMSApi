using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class NablEmployeeEquipmentAuthrization
    {
        [Key]
        public long Id { get; set; }
        public long EmployeeAuthorazitionId { get; set; }
        public string UID { get; set; }
        public long EquipmentId { get; set; }
        public string EquipmentName { get; set; }

        [ForeignKey("EmployeeAuthorazitionId")]
        [JsonIgnore]
        public NablEmployeeAuthorization? EmployeeAuthorization { get; set; }
    }
}
