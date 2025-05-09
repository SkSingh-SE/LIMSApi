using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class SpecificationHeader : AuditProperty
{
    [Key]
    public long ID { get; set; }

    public string? SpecificationCode { get; set; }

    public long? StandardOrganizationID { get; set; }

    public string? Standard { get; set; }

    public string? Part { get; set; }

    public string? StandardYear { get; set; }

    public string? Grade { get; set; }

    /// <summary>
    /// IsUNS(0=false=Steel Number, 1=true=UNS Number) 
    /// </summary>
    public bool? IsUNS { get; set; }

    public string? UNSSteelNumber { get; set; }

    public string? AliasName { get; set; }

    public long? MetalCalssificationID { get; set; }

    [ForeignKey("MetalCalssificationID")]
    public virtual MetalClassificationMaster? MetalClassification { get; set; }

    [ForeignKey("StandardOrganizationID")]
    public virtual StandardOrganizationMaster? StandardOrganization { get; set; }
    public virtual ICollection<SpecificationLine> SpecificationLines { get; set; } = new List<SpecificationLine>();

}
