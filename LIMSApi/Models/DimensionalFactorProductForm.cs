using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models;

public class DimensionalFactorProductForm
{
    public long DimensionalFactorID { get; set; }
    [ForeignKey("DimensionalFactorID"), JsonIgnore]
    public virtual DimensionalFactorMaster? DimensionalFactor { get; set; }

    public long ProductFormID { get; set; }
    [ForeignKey("ProductFormID")]
    public virtual ProductFormMaster? ProductForm { get; set; }
}
