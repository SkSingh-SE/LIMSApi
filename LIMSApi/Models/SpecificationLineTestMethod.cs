using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LIMSApi.Models;

/// <summary>
/// MS-E: maps a material-specification parameter line to the Laboratory Test +
/// Test Method Specification that performs it, with the number of test specimens required.
/// </summary>
public class SpecificationLineTestMethod
{
    [Key]
    public long ID { get; set; }

    public long SpecificationLineID { get; set; }

    public long? LaboratoryTestID { get; set; }

    public long? TestMethodSpecificationID { get; set; }

    public int? NumberOfTestSpecimen { get; set; }

    public int? DisplayOrder { get; set; }

    [ForeignKey("SpecificationLineID"), JsonIgnore]
    public virtual SpecificationLine? SpecificationLine { get; set; }

    [ForeignKey("LaboratoryTestID")]
    public virtual LaboratoryTest? LaboratoryTest { get; set; }

    [ForeignKey("TestMethodSpecificationID")]
    public virtual TestMethodSpecification? TestMethodSpecification { get; set; }
}
