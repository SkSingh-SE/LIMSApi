using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class CompanyMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [Required, StringLength(100)]
    public string CompanyName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string CompanyType { get; set; } = string.Empty;

    public string? PhoneNo { get; set; }

    public string? MobileNo { get; set; }

    public string? EmailId { get; set; }

    public string? Website { get; set; }

    public long? CountryID { get; set; }

    public long? StateID { get; set; }

    public long? CityID { get; set; }

    public long? AreaID { get; set; }

    public long? CurrencyID { get; set; }

    public string? Vatno { get; set; }

    public string? RegistrationNo { get; set; }

    public long? ResponsiblePerson { get; set; }

    public byte[]? Logo { get; set; }

    public byte[]? Gstno { get; set; }

    // Navigation Property
    [ForeignKey("CountryID")]
    public virtual CountryMaster? Country { get; set; }

    [ForeignKey("StateID")]
    public virtual StateMaster? State { get; set; }
    [ForeignKey("CityID")]
    public virtual CityMaster? City { get; set; }

    [ForeignKey("AreaID")]
    public virtual AreaMaster? Area { get; set; }

    [ForeignKey("CurrencyID")]
    public virtual CurrencyMaster? Currency { get; set; }

}
