using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models;

public class ProductConditionPropertyType
{
    public long ProductConditionID { get; set; }
    [ForeignKey("ProductConditionID"), JsonIgnore]
    public virtual ProductConditionMaster? ProductCondition { get; set; }

    public long PropertyTypeID { get; set; }
    [ForeignKey("PropertyTypeID")]
    public virtual PropertyTypeMaster? PropertyType { get; set; }
}
