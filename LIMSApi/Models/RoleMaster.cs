using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LIMSApi.Models;

public partial class RoleMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    public string? Description { get; set; }
    public ICollection<RoleMenuMapping> MenuItems { get; set; } = new List<RoleMenuMapping>();
}
