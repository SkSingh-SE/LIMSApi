using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class MetalClassificationMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(20)]
    public string? Code { get; set; }

    public long? ParentID { get; set; }

    public bool HasChemicalParams { get; set; } = false;
    public bool HasMechanicalParams { get; set; } = false;
    public int SortOrder { get; set; } = 0;

    [StringLength(20)]
    public string? MetalType { get; set; }

    // ---- Chemical billing config (set on base/parent; children inherit if null). L3 / pricing ----
    /// e.g. FE_SPECTRO_GROUP, CU_SPECTRO_GROUP — pricing group this metal base bills under.
    [StringLength(50)]
    public string? ChemicalBillingGroup { get; set; }

    /// Element-count breakpoint for spectro count pricing (NI_SPECTRO / ELEM_COUNT).
    public int? SpectroElementThreshold { get; set; }

    /// Special-element surcharge (N/B/Ca, Ag) applies only when element count >= this (Fe = 3).
    public int? SurchargeAppliesFromElement { get; set; }

    /// Enables N/B/Ca (Fe) / Ag (Cu) spectro surcharge for this metal base.
    public bool HasSpectroSpecialSurcharge { get; set; } = false;

    [ForeignKey("ParentID")]
    public virtual MetalClassificationMaster? Parent { get; set; }
    public virtual ICollection<MetalClassificationMaster> Children { get; set; } = new List<MetalClassificationMaster>();
    public virtual ICollection<MetalClassificationParameter> Parameters { get; set; } = new List<MetalClassificationParameter>();

    /// Analysis techniques (OES/WET/ICP/LECO/WDXRF/EDXRF) valid for this metal base. L3.
    public virtual ICollection<MetalClassificationAnalysisTechnique> CompatibleTechniques { get; set; } = new List<MetalClassificationAnalysisTechnique>();
}
