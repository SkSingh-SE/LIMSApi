using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class StateMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    public long? CountryID { get; set; }

    public string? Code { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }

    public string? Gstcode { get; set; }

    [ForeignKey("CountryID")]
    public virtual CountryMaster? Country { get; set; }
}
