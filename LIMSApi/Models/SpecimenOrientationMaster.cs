using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class SpecimenOrientationMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(10)]
    public string? Code { get; set; }

    public long? SpecimenOrientationCategoryID { get; set; }

    [StringLength(50)]
    public string? NotationType { get; set; } = "Two-letter";

    [StringLength(10)]
    public string? LengthAxis { get; set; }

    [StringLength(10)]
    public string? CrackOrNotchDirection { get; set; }

    [StringLength(20)]
    public string? StandardNotation { get; set; }

    public long? TestMethodSpecificationID { get; set; }

    [StringLength(100)]
    public string? TestStandardName { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [ForeignKey("SpecimenOrientationCategoryID")]
    public virtual SpecimenOrientationCategoryMaster? SpecimenOrientationCategory { get; set; }

    public virtual ICollection<SpecimenOrientationProductForm> ApplicableForms { get; set; } = new List<SpecimenOrientationProductForm>();
    public virtual ICollection<SpecimenOrientationMetalClassification> ApplicableClassifications { get; set; } = new List<SpecimenOrientationMetalClassification>();
}
