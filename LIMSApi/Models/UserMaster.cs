using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class UserMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    public string? UserCode { get; set; }
    [StringLength(100)]
    public required string UserName { get; set; }

    public long? EmployeeID { get; set; }

    public string? EmailId { get; set; }

    public string? Password { get; set; }

    public long? RoleID { get; set; }
    public string? RoleName { get; set; }

    public bool? RemotLogin { get; set; }

    public bool? DeviceUser { get; set; }

    public bool? SamplePrepare { get; set; }
    [ForeignKey("RoleID")]
    public virtual RoleMaster? Role {  get; set; }
    [ForeignKey("EmployeeID")]
    public virtual EmployeeMaster? Employee { get; set; }

}
