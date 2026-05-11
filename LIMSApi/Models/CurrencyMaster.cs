using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models;

public partial class CurrencyMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }
    [StringLength(100)]
    public required string Name { get; set; }

    public string? Code { get; set; }

    [StringLength(10)]
    public string Symbol { get; set; } = "₹";

    public bool IsDefault { get; set; } = false;
}
