using System;
using System.Collections.Generic;

namespace LIMSApi.Models;

public partial class SiteError
{
    public long ID { get; set; }

    public string? ErrorCode { get; set; }

    public string? ExceptionMessage { get; set; }

    public string? ExceptionStackTrace { get; set; }

    public string? Source { get; set; }

    public string? Ipaddress { get; set; }

    public string? Browser { get; set; }

    public string? Description { get; set; }

    public string? WebUrl { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? CompanyCode { get; set; }
}
