using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMSApi.Models;

public partial class SiteActivity
{
    [Key]
    public long ID { get; set; }

    public string? ModuleName { get; set; }
    public string? Action { get; set; }

    public string? TraceId { get; set; }

    public string? Ipaddress { get; set; }

    public string? Browser { get; set; }

    public string? Description { get; set; }

    public string? WebUrl { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? CompanyCode { get; set; }
}
