using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class ProductMasterMetalClassification
    {
        public long ProductMasterID { get; set; }
        public long MetalClassificationID { get; set; }

        [ForeignKey("ProductMasterID"), JsonIgnore]
        public virtual ProductMaster? ProductMaster { get; set; }

        [ForeignKey("MetalClassificationID")]
        public virtual MetalClassificationMaster? MetalClassification { get; set; }
    }
}
