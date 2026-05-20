using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Models
{
    /// <summary>
    /// Date-effective rate for a CuttingPriceMaster. A new row is added every time the rate
    /// changes (yearly or mid-year). Resolution: the row with the greatest EffectiveFrom
    /// not after the sample inward date.
    /// </summary>
    public class CuttingPriceVersion : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long CuttingPriceMasterID { get; set; }

        // The date this rate takes effect — the resolution key.
        public DateTime EffectiveFrom { get; set; }

        // Annual price-list this row belongs to — derived from EffectiveFrom on save.
        // Used for copy-forward + reminders; not used for resolution.
        public long? FinancialYearId { get; set; }

        [Precision(10, 2)]
        public decimal RatePerUnit { get; set; }       // Normal-metal rate

        [Precision(10, 2)]
        public decimal RatePerUnitHard { get; set; }   // Hard-metal rate (Duplex/Ni-Base/Ti-Base etc.)

        [ForeignKey(nameof(CuttingPriceMasterID))]
        [JsonIgnore]
        public virtual CuttingPriceMaster? CuttingPriceMaster { get; set; }

        [ForeignKey(nameof(FinancialYearId))]
        public virtual FinancialYear? FinancialYear { get; set; }
    }
}
