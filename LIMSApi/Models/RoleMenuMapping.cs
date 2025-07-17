using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class RoleMenuMapping
    {
        [Key]
        public long ID { get; set; }
        public long RoleID { get; set; }
        public long MenuID { get; set; }
        [JsonIgnore]
        [ForeignKey("RoleID")]
        public virtual RoleMaster? Role { get; set; }
        [ForeignKey("MenuID")]
        public virtual MenuMaster? Menu { get; set; }
    }
}
