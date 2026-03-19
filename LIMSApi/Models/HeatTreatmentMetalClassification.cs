using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models;

public class HeatTreatmentMetalClassification
{
    public long HeatTreatmentID { get; set; }
    [ForeignKey("HeatTreatmentID"), JsonIgnore]
    public virtual HeatTreatmentMaster? HeatTreatment { get; set; }

    public long MetalClassificationID { get; set; }
    [ForeignKey("MetalClassificationID")]
    public virtual MetalClassificationMaster? MetalClassification { get; set; }
}
