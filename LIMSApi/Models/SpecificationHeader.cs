using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class SpecificationHeader : AuditProperty
{
    [Key]
    public long ID { get; set; }
    [StringLength(100),Required]
    public required string AliasName { get; set; }

    public long? StandardOrganizationID { get; set; }

    public string? Standard { get; set; }

    public string? Part { get; set; }

    public string? StandardYear { get; set; }
    public bool IsCustom { get; set; }


    [ForeignKey("StandardOrganizationID")]
    public virtual StandardOrganizationMaster? StandardOrganization { get; set; }
    public virtual ICollection<SpecificationGrade> Grades { get; set; } = new List<SpecificationGrade>();

}