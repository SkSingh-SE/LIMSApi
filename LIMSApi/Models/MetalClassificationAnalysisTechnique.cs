using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    /// <summary>
    /// Junction: which Analysis Techniques (OES/WET/ICP/LECO/WDXRF/EDXRF) are valid for a metal base.
    /// Drives the technique cascade at inward/plan and the "no spectro possible" gate (L3).
    /// </summary>
    public class MetalClassificationAnalysisTechnique : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long MetalClassificationID { get; set; }
        public long AnalysisTechniqueID { get; set; }

        [ForeignKey(nameof(MetalClassificationID))]
        public virtual MetalClassificationMaster? MetalClassification { get; set; }

        [ForeignKey(nameof(AnalysisTechniqueID))]
        public virtual AnalysisTechniqueMaster? AnalysisTechnique { get; set; }
    }
}
