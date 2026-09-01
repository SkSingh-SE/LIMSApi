using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    public class ProductMasterVersionGradeCondition : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long ProductMasterVersionGradeID { get; set; }

        public long? ProductConditionID1 { get; set; }

        public long? ProductConditionID2 { get; set; }

        public long? HeatTreatmentID { get; set; }

        public long? ProductSizeMasterID { get; set; }

        public int Priority { get; set; } = 1;

        [ForeignKey("ProductMasterVersionGradeID"), JsonIgnore]
        public virtual ProductMasterVersionGrade? ProductMasterVersionGrade { get; set; }

        [ForeignKey("ProductConditionID1")]
        public virtual ProductConditionMaster? ProductCondition1 { get; set; }

        [ForeignKey("ProductConditionID2")]
        public virtual ProductConditionMaster? ProductCondition2 { get; set; }

        [ForeignKey("HeatTreatmentID")]
        public virtual HeatTreatmentMaster? HeatTreatment { get; set; }

        [ForeignKey("ProductSizeMasterID")]
        public virtual ProductSizeMaster? ProductSizeMaster { get; set; }
    }
}
