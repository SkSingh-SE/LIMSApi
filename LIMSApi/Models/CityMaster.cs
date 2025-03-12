using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models
{
    public class CityMaster : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long StateID { get; set; }  // Foreign Key to State

        public string? Code { get; set; }

        public required string Name { get; set; }

        // Navigation Property
        [ForeignKey("StateID")]
        public virtual StateMaster? State { get; set; }
    }

}
