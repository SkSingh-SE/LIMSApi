using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models;

public class SpecimenOrientationProductForm
{
    public long SpecimenOrientationID { get; set; }
    [ForeignKey("SpecimenOrientationID"), JsonIgnore]
    public virtual SpecimenOrientationMaster? SpecimenOrientation { get; set; }

    public long ProductFormID { get; set; }
    [ForeignKey("ProductFormID")]
    public virtual ProductFormMaster? ProductForm { get; set; }
}
