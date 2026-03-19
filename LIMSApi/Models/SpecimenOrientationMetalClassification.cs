using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models;

public class SpecimenOrientationMetalClassification
{
    public long SpecimenOrientationID { get; set; }
    [ForeignKey("SpecimenOrientationID"), JsonIgnore]
    public virtual SpecimenOrientationMaster? SpecimenOrientation { get; set; }

    public long MetalClassificationID { get; set; }
    [ForeignKey("MetalClassificationID")]
    public virtual MetalClassificationMaster? MetalClassification { get; set; }
}
