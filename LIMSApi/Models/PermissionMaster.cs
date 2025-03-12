using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models;

public partial class PermissionMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    public long? RoleID { get; set; }

    public long? MenuID { get; set; }

    public bool? Viewp { get; set; }

    public bool? Addp { get; set; }

    public bool? Editp { get; set; }

    public bool? Deletep { get; set; }

    public bool? ExportP { get; set; }
}
