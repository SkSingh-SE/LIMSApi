using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class SpecificationGrade
{
    [Key]
    public long ID { get; set; }
    public long SpecificationHeaderID { get; set; }
    [StringLength(100),Required]
    public required string Grade { get; set; }
    public bool? IsUNS { get; set; }

    public string? UNSSteelNumber { get; set; }

    public long? MetalClassificationID { get; set; }

    public long? TestMethodSpecificationID { get; set; }

    [ForeignKey("MetalCalssificationID")]
    public virtual MetalClassificationMaster? MetalClassification { get; set; }
    [ForeignKey("TestMethodSpecificationID")]
    public virtual TestMethodSpecification? TestMethodSpecification { get; set; }

    public virtual ICollection<SpecificationLine> SpecificationLines { get; set; } = new List<SpecificationLine>();

}