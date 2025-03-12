using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models;

public partial class ParameterUnitMaster : AuditProperty
{
    public long ID { get; set; }
    [StringLength(100)]
    public required string Name { get; set; }
}
