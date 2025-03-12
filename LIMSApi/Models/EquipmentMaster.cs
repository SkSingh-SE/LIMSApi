using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models;

public partial class EquipmentMaster : AuditProperty
{
    [Key]
    public long ID { get; set; }

    [StringLength(100)]
    public required string Name { get; set; }
    [StringLength(20)]
    public required string IdentificationNo { get; set; }

    public long? TestTypeID { get; set; }
    public long? MakerID { get; set; }

    public string? Remark { get; set; }

    public long? EquipmentTypeID { get; set; }
    public string? Capacity { get; set; }


    [ForeignKey("TestTypeID")]
    public virtual TestTypeMaster? TestType { get; set; }

    [ForeignKey("MakerID")]
    public virtual MakerMaster? Maker { get; set; }

    [ForeignKey("EquipmentTypeID")]
    public virtual EquipmentTypeMaster? EquipmentType { get; set; }


}
