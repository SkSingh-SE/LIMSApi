using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models;

public class ParameterSpecimenOrientation
{
    public long ParameterID { get; set; }
    [ForeignKey("ParameterID"), JsonIgnore]
    public virtual ParameterMaster? Parameter { get; set; }

    public long SpecimenOrientationID { get; set; }
    [ForeignKey("SpecimenOrientationID")]
    public virtual SpecimenOrientationMaster? SpecimenOrientation { get; set; }
}
