using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models
{
    /// <summary>
    /// Date-effective price for a MachiningChargeMaster. A new row is added every time the
    /// price changes (yearly or mid-year). Resolution: the row with the greatest
    /// EffectiveFrom not after the sample inward date.
    /// </summary>
    public class MachiningChargeVersion : AuditProperty
    {
        [Key]
        public long ID { get; set; }

        public long MachiningChargeMasterID { get; set; }

        // The date this price takes effect — the resolution key.
        public DateTime EffectiveFrom { get; set; }

        // Annual price-list this row belongs to — derived from EffectiveFrom on save.
        // Used for copy-forward + reminders; not used for resolution.
        public long? FinancialYearId { get; set; }

        // Machining rates
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceGeneralMetal { get; set; }   // Normal-metal machining rate

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceHardMetal { get; set; }      // Hard-metal machining rate

        // Cutting rates (same year-version)
        [Column(TypeName = "decimal(18,2)")]
        public decimal CuttingRateGeneralMetal { get; set; }   // Normal-metal cutting rate

        [Column(TypeName = "decimal(18,2)")]
        public decimal CuttingRateHardMetal { get; set; }      // Hard-metal cutting rate

        [ForeignKey(nameof(MachiningChargeMasterID))]
        [JsonIgnore]
        public virtual MachiningChargeMaster? MachiningChargeMaster { get; set; }

        [ForeignKey(nameof(FinancialYearId))]
        public virtual FinancialYear? FinancialYear { get; set; }
    }
}